using System.Security.Cryptography;
using System.Text;

namespace Velaris.SmtpEngine.Spike;

public sealed record ScramParameters(
    byte[] Salt,
    int Iterations,
    byte[] StoredKey,
    byte[] ServerKey)
{
    public static ScramParameters Derive(string password, byte[] salt, int iterations)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(salt);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(iterations);

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltedPassword = Rfc2898DeriveBytes.Pbkdf2(
            passwordBytes,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            outputLength: 32);

        var clientKeyConstant = Encoding.UTF8.GetBytes("Client Key");
        var serverKeyConstant = Encoding.UTF8.GetBytes("Server Key");

        var clientKey = HMACSHA256.HashData(saltedPassword, clientKeyConstant);
        var storedKey = SHA256.HashData(clientKey);
        var serverKey = HMACSHA256.HashData(saltedPassword, serverKeyConstant);

        return new ScramParameters(salt, iterations, storedKey, serverKey);
    }
}

public sealed class ScramServerSession
{
    private string? _clientFirstBare;
    private string? _serverFirstMessage;
    private string? _combinedNonce;
    private byte[]? _authMessageBytes;

    public string? Username { get; private set; }
    public bool IsAuthenticated { get; private set; }

    public string ProcessClientFirst(string clientFirstMessage, byte[] salt, int iterations, string? serverNonce = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientFirstMessage);
        ArgumentNullException.ThrowIfNull(salt);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(iterations);

        // Client-first-message format: [gs2-header] client-first-message-bare
        // gs2-header for SCRAM without channel binding is "n,,"
        var parts = clientFirstMessage.Split(',', 3);
        if (parts.Length < 3)
        {
            throw new FormatException("Malformed client-first message: insufficient parts");
        }

        var gs2CbindFlag = parts[0];
        if (gs2CbindFlag != "n" && gs2CbindFlag != "y")
        {
            throw new FormatException($"Unsupported GS2 channel binding flag: '{gs2CbindFlag}'");
        }

        var clientFirstBare = parts[2];
        _clientFirstBare = clientFirstBare;

        // Parse bare message attributes
        string? username = null;
        string? clientNonce = null;

        var bareParts = clientFirstBare.Split(',');
        foreach (var part in bareParts)
        {
            if (part.StartsWith("n=", StringComparison.Ordinal))
            {
                username = part[2..];
            }
            else if (part.StartsWith("r=", StringComparison.Ordinal))
            {
                clientNonce = part[2..];
            }
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new FormatException("Missing username (n=) in client-first message");
        }

        if (string.IsNullOrWhiteSpace(clientNonce))
        {
            throw new FormatException("Missing nonce (r=) in client-first message");
        }

        Username = username;

        // Generate server nonce if not supplied
        var sNonce = serverNonce ?? Convert.ToBase64String(RandomNumberGenerator.GetBytes(18));
        _combinedNonce = clientNonce + sNonce;

        var saltBase64 = Convert.ToBase64String(salt);
        _serverFirstMessage = $"r={_combinedNonce},s={saltBase64},i={iterations}";

        return _serverFirstMessage;
    }

    public (bool Success, string ServerFinalMessage) ProcessClientFinal(
        string clientFinalMessage,
        byte[] storedKey,
        byte[] serverKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientFinalMessage);
        ArgumentNullException.ThrowIfNull(storedKey);
        ArgumentNullException.ThrowIfNull(serverKey);

        if (_clientFirstBare is null || _serverFirstMessage is null || _combinedNonce is null)
        {
            throw new InvalidOperationException("Cannot process client-final before client-first");
        }

        string? cbind = null;
        string? nonce = null;
        string? proofBase64 = null;
        var finalParts = clientFinalMessage.Split(',');

        var withoutProofParts = new List<string>();

        foreach (var part in finalParts)
        {
            if (part.StartsWith("c=", StringComparison.Ordinal))
            {
                cbind = part[2..];
                withoutProofParts.Add(part);
            }
            else if (part.StartsWith("r=", StringComparison.Ordinal))
            {
                nonce = part[2..];
                withoutProofParts.Add(part);
            }
            else if (part.StartsWith("p=", StringComparison.Ordinal))
            {
                proofBase64 = part[2..];
            }
            else
            {
                withoutProofParts.Add(part);
            }
        }

        if (cbind != "biws") // base64 for "n,,"
        {
            return (false, "Malformed or mismatched channel binding");
        }

        if (nonce != _combinedNonce)
        {
            return (false, "Mismatched nonce");
        }

        if (string.IsNullOrWhiteSpace(proofBase64))
        {
            return (false, "Missing client proof");
        }

        byte[] clientProof;
        try
        {
            clientProof = Convert.FromBase64String(proofBase64);
        }
        catch (FormatException)
        {
            return (false, "Invalid base64 in client proof");
        }

        var clientFinalWithoutProof = string.Join(",", withoutProofParts);
        var authMessage = $"{_clientFirstBare},{_serverFirstMessage},{clientFinalWithoutProof}";
        _authMessageBytes = Encoding.UTF8.GetBytes(authMessage);

        var clientSignature = HMACSHA256.HashData(storedKey, _authMessageBytes);

        if (clientProof.Length != clientSignature.Length)
        {
            return (false, "Invalid proof length");
        }

        // ClientKey = ClientProof XOR ClientSignature
        var clientKey = new byte[clientProof.Length];
        for (var i = 0; i < clientProof.Length; i++)
        {
            clientKey[i] = (byte)(clientProof[i] ^ clientSignature[i]);
        }

        var computedStoredKey = SHA256.HashData(clientKey);

        // Constant-time verification
        if (!CryptographicOperations.FixedTimeEquals(computedStoredKey, storedKey))
        {
            return (false, "Verification failed");
        }

        var serverSignature = HMACSHA256.HashData(serverKey, _authMessageBytes);
        var serverSignatureBase64 = Convert.ToBase64String(serverSignature);

        IsAuthenticated = true;
        return (true, $"v={serverSignatureBase64}");
    }
}

public sealed class ScramClientSession
{
    private readonly string _username;
    private readonly string _password;
    private string? _clientNonce;
    private string? _clientFirstBare;

    public ScramClientSession(string username, string password)
    {
        _username = username ?? throw new ArgumentNullException(nameof(username));
        _password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public string CreateClientFirst(string? clientNonce = null)
    {
        _clientNonce = clientNonce ?? Convert.ToBase64String(RandomNumberGenerator.GetBytes(18));
        _clientFirstBare = $"n={_username},r={_clientNonce}";
        return $"n,,{_clientFirstBare}";
    }

    public string ProcessServerFirstAndCreateClientFinal(string serverFirstMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverFirstMessage);

        if (_clientNonce is null || _clientFirstBare is null)
        {
            throw new InvalidOperationException("CreateClientFirst must be called first");
        }

        string? nonce = null;
        string? saltBase64 = null;
        var iterations = 0;

        foreach (var part in serverFirstMessage.Split(','))
        {
            if (part.StartsWith("r=", StringComparison.Ordinal))
            {
                nonce = part[2..];
            }
            else if (part.StartsWith("s=", StringComparison.Ordinal))
            {
                saltBase64 = part[2..];
            }
            else if (part.StartsWith("i=", StringComparison.Ordinal))
            {
                int.TryParse(part[2..], out iterations);
            }
        }

        if (string.IsNullOrWhiteSpace(nonce) || !nonce.StartsWith(_clientNonce, StringComparison.Ordinal))
        {
            throw new FormatException("Server nonce does not start with client nonce");
        }

        if (string.IsNullOrWhiteSpace(saltBase64) || iterations <= 0)
        {
            throw new FormatException("Invalid salt or iterations in server challenge");
        }

        var salt = Convert.FromBase64String(saltBase64);

        // Derive keys
        var passwordBytes = Encoding.UTF8.GetBytes(_password);
        var saltedPassword = Rfc2898DeriveBytes.Pbkdf2(
            passwordBytes,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            outputLength: 32);

        var clientKeyConstant = Encoding.UTF8.GetBytes("Client Key");
        var clientKey = HMACSHA256.HashData(saltedPassword, clientKeyConstant);
        var storedKey = SHA256.HashData(clientKey);

        var clientFinalWithoutProof = $"c=biws,r={nonce}";
        var authMessage = $"{_clientFirstBare},{serverFirstMessage},{clientFinalWithoutProof}";
        var authMessageBytes = Encoding.UTF8.GetBytes(authMessage);

        var clientSignature = HMACSHA256.HashData(storedKey, authMessageBytes);

        var clientProof = new byte[clientKey.Length];
        for (var i = 0; i < clientKey.Length; i++)
        {
            clientProof[i] = (byte)(clientKey[i] ^ clientSignature[i]);
        }

        var clientProofBase64 = Convert.ToBase64String(clientProof);
        return $"{clientFinalWithoutProof},p={clientProofBase64}";
    }
}
