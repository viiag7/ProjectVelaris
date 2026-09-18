using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Xunit;

namespace Velaris.SmtpEngine.Spike;

public sealed class SmtpEngineSpikeTests
{
    [Fact]
    public void Rfc7677_OfficialTestVectors_MustPass()
    {
        // Vector from RFC 7677 Section 3:
        // Username: 'user', Password: 'pencil'
        // Salt: W22ZaJ0SNY7soEsUEjb6gQ==
        // Iterations: 4096
        // Client nonce: rOprNGfwEbeRWgbNEkqO
        // Server nonce: %hvYDpWUa2RaTCAfuxFIlj)hNlF$k0
        // Expected client-first: n,,n=user,r=rOprNGfwEbeRWgbNEkqO
        // Expected server-first: r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,s=W22ZaJ0SNY7soEsUEjb6gQ==,i=4096
        // Expected client-final: c=biws,r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,p=dHzbZapWIk4jUhN+Ute9ytag9zjfMHgsqmmiz7AndVQ=
        // Expected server-final: v=6rriTRBi23WpRR/wtup+mMhUZUn/dB5nLTJRsjl95G4=

        var salt = Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ==");
        const int iterations = 4096;
        const string password = "pencil";
        const string clientNonce = "rOprNGfwEbeRWgbNEkqO";
        const string serverNonce = "%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0";

        var paramsDerived = ScramParameters.Derive(password, salt, iterations);

        // 1. Client creates client-first message
        var clientSession = new ScramClientSession("user", password);
        var clientFirst = clientSession.CreateClientFirst(clientNonce);
        Assert.Equal("n,,n=user,r=rOprNGfwEbeRWgbNEkqO", clientFirst);

        // 2. Server receives client-first, verifies and responds with server challenge
        var serverSession = new ScramServerSession();
        var serverFirst = serverSession.ProcessClientFirst(clientFirst, salt, iterations, serverNonce);
        Assert.Equal("r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,s=W22ZaJ0SNY7soEsUEjb6gQ==,i=4096", serverFirst);

        // 3. Client receives challenge and creates client-final message
        var clientFinal = clientSession.ProcessServerFirstAndCreateClientFinal(serverFirst);
        Assert.Equal("c=biws,r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,p=dHzbZapWIk4jUhN+Ute9ytag9zjfMHgsqmmiz7AndVQ=", clientFinal);

        // 4. Server receives client-final, verifies proof against StoredKey and returns ServerSignature
        var (success, serverFinal) = serverSession.ProcessClientFinal(
            clientFinal,
            paramsDerived.StoredKey,
            paramsDerived.ServerKey);

        Assert.True(success, "Server verification of RFC 7677 client proof failed");
        Assert.Equal("v=6rriTRBi23WpRR/wtup+mMhUZUn/dB5nLTJRsjl95G4=", serverFinal);
    }

    [Fact]
    public void ScramSha256_InvalidPassword_MustFailVerification()
    {
        var salt = Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ==");
        const int iterations = 4096;

        // Correct credentials in database
        var validParams = ScramParameters.Derive("pencil", salt, iterations);

        // Attacker uses wrong password
        var attackerSession = new ScramClientSession("user", "wrong_password");
        var clientFirst = attackerSession.CreateClientFirst("clientnonce12345");

        var serverSession = new ScramServerSession();
        var serverFirst = serverSession.ProcessClientFirst(clientFirst, salt, iterations, "servernonce12345");

        var clientFinal = attackerSession.ProcessServerFirstAndCreateClientFinal(serverFirst);

        var (success, _) = serverSession.ProcessClientFinal(clientFinal, validParams.StoredKey, validParams.ServerKey);
        Assert.False(success, "Verification must fail for incorrect password");
        Assert.False(serverSession.IsAuthenticated);
    }

    [Fact]
    public async Task Smtp_OverImplicitTls_Authenticates_And_Submits_Successfully()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        var salt = Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ==");
        var credentials = ScramParameters.Derive("pencil", salt, 4096);

        await using var server = new SmtpEnginePrototype(cert, username => username == "user" ? credentials : null);
        server.Start();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", server.Port);

        using var sslStream = new SslStream(tcpClient.GetStream(), false, (s, c, ch, err) => true);
        await sslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
        {
            TargetHost = "localhost",
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
        });

        // Verify TLS negotiated
        Assert.True(sslStream.SslProtocol is SslProtocols.Tls12 or SslProtocols.Tls13);

        using var reader = new StreamReader(sslStream, Encoding.ASCII);
        using var writer = new StreamWriter(sslStream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };

        // 1. Banner
        var banner = await reader.ReadLineAsync();
        Assert.StartsWith("220 ", banner);

        // 2. EHLO
        await writer.WriteLineAsync("EHLO client.test");
        var ehloLines = new List<string>();
        while (true)
        {
            var line = await reader.ReadLineAsync();
            Assert.NotNull(line);
            ehloLines.Add(line);
            if (line.StartsWith("250 ")) break;
        }

        Assert.Contains(ehloLines, l => l.Contains("250-AUTH SCRAM-SHA-256"));
        Assert.DoesNotContain(ehloLines, l => l.Contains("PLAIN"));
        Assert.DoesNotContain(ehloLines, l => l.Contains("LOGIN"));
        Assert.DoesNotContain(ehloLines, l => l.Contains("STARTTLS"));

        // 3. AUTH SCRAM-SHA-256
        var clientSession = new ScramClientSession("user", "pencil");
        var clientFirst = clientSession.CreateClientFirst();
        var clientFirstBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientFirst));

        await writer.WriteLineAsync($"AUTH SCRAM-SHA-256 {clientFirstBase64}");
        var challengeResponse = await reader.ReadLineAsync();
        Assert.NotNull(challengeResponse);
        Assert.StartsWith("334 ", challengeResponse);

        var serverFirstBase64 = challengeResponse[4..].Trim();
        var serverFirst = Encoding.UTF8.GetString(Convert.FromBase64String(serverFirstBase64));

        var clientFinal = clientSession.ProcessServerFirstAndCreateClientFinal(serverFirst);
        var clientFinalBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientFinal));

        await writer.WriteLineAsync(clientFinalBase64);
        var authResult = await reader.ReadLineAsync();
        Assert.NotNull(authResult);
        Assert.StartsWith("235 2.7.0", authResult);

        // 4. Envelope submission
        await writer.WriteLineAsync("MAIL FROM:<sender@velaris.local>");
        var mailReply = await reader.ReadLineAsync();
        Assert.StartsWith("250 ", mailReply);

        await writer.WriteLineAsync("RCPT TO:<recipient@example.com>");
        var rcptReply = await reader.ReadLineAsync();
        Assert.StartsWith("250 ", rcptReply);

        await writer.WriteLineAsync("DATA");
        var dataPrompt = await reader.ReadLineAsync();
        Assert.StartsWith("354 ", dataPrompt);

        await writer.WriteLineAsync("Subject: Test Mail");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("Hello from automated test");
        await writer.WriteLineAsync(".");
        var dataReply = await reader.ReadLineAsync();
        Assert.StartsWith("250 ", dataReply);

        // 5. QUIT
        await writer.WriteLineAsync("QUIT");
        var quitReply = await reader.ReadLineAsync();
        Assert.StartsWith("221 ", quitReply);
    }

    [Fact]
    public async Task Smtp_PlaintextConnection_MustBeRejected_WithoutBanner()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        await using var server = new SmtpEnginePrototype(cert, _ => null);
        server.Start();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", server.Port);

        var networkStream = tcpClient.GetStream();
        networkStream.ReadTimeout = 2000;
        using var writer = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };
        using var reader = new StreamReader(networkStream, Encoding.ASCII);

        // Plaintext client sends EHLO instead of TLS ClientHello
        await writer.WriteLineAsync("EHLO plaintext.client");

        // The TLS handshake on the server fails; server does not send 220 banner
        // and connection drops or returns EOF
        string? response = null;
        try
        {
            response = await reader.ReadLineAsync();
        }
        catch (IOException)
        {
            // Expected socket termination due to failed TLS handshake
        }

        Assert.Null(response);
    }

    [Fact]
    public async Task Smtp_UnauthenticatedCommands_MustBeRejectedWith530()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        await using var server = new SmtpEnginePrototype(cert, _ => null);
        server.Start();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", server.Port);

        using var sslStream = new SslStream(tcpClient.GetStream(), false, (s, c, ch, err) => true);
        await sslStream.AuthenticateAsClientAsync("localhost");

        using var reader = new StreamReader(sslStream, Encoding.ASCII);
        using var writer = new StreamWriter(sslStream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };

        await reader.ReadLineAsync(); // Banner

        await writer.WriteLineAsync("EHLO client");
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line!.StartsWith("250 ")) break;
        }

        // MAIL FROM before AUTH -> 530 5.7.0 Authentication required
        await writer.WriteLineAsync("MAIL FROM:<test@example.com>");
        var reply = await reader.ReadLineAsync();
        Assert.StartsWith("530 5.7.0", reply);
    }

    [Fact]
    public async Task Smtp_DisallowedCommands_StartTls_And_PlainAuth_MustBeRejected()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        await using var server = new SmtpEnginePrototype(cert, _ => null);
        server.Start();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", server.Port);

        using var sslStream = new SslStream(tcpClient.GetStream(), false, (s, c, ch, err) => true);
        await sslStream.AuthenticateAsClientAsync("localhost");

        using var reader = new StreamReader(sslStream, Encoding.ASCII);
        using var writer = new StreamWriter(sslStream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };

        await reader.ReadLineAsync(); // Banner
        await writer.WriteLineAsync("EHLO client");
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line!.StartsWith("250 ")) break;
        }

        // STARTTLS -> 502 Command not implemented
        await writer.WriteLineAsync("STARTTLS");
        var startTlsReply = await reader.ReadLineAsync();
        Assert.StartsWith("502 5.5.1", startTlsReply);

        // AUTH PLAIN -> 504 Unrecognized authentication type
        await writer.WriteLineAsync("AUTH PLAIN");
        var plainReply = await reader.ReadLineAsync();
        Assert.StartsWith("504 5.5.4", plainReply);

        // AUTH LOGIN -> 504 Unrecognized authentication type
        await writer.WriteLineAsync("AUTH LOGIN");
        var loginReply = await reader.ReadLineAsync();
        Assert.StartsWith("504 5.5.4", loginReply);
    }

    [Fact]
    public async Task Smtp_ExceedingMaxAuthFailures_MustCloseConnection()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        await using var server = new SmtpEnginePrototype(cert, _ => null);
        server.Start();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", server.Port);

        using var sslStream = new SslStream(tcpClient.GetStream(), false, (s, c, ch, err) => true);
        await sslStream.AuthenticateAsClientAsync("localhost");

        using var reader = new StreamReader(sslStream, Encoding.ASCII);
        using var writer = new StreamWriter(sslStream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };

        await reader.ReadLineAsync(); // Banner
        await writer.WriteLineAsync("EHLO client");
        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line!.StartsWith("250 ")) break;
        }

        // 1st failure
        await writer.WriteLineAsync("AUTH INVALID");
        var r1 = await reader.ReadLineAsync();
        Assert.StartsWith("504 ", r1);

        // 2nd failure
        await writer.WriteLineAsync("AUTH PLAIN");
        var r2 = await reader.ReadLineAsync();
        Assert.StartsWith("504 ", r2);

        // 3rd failure -> 421 Too many authentication failures and connection closed
        await writer.WriteLineAsync("AUTH LOGIN");
        var r3 = await reader.ReadLineAsync();
        Assert.StartsWith("421 4.7.0", r3);

        // Following read should return null (EOF)
        var next = await reader.ReadLineAsync();
        Assert.Null(next);
    }

    [Fact]
    public async Task Smtp_MailKitClient_Interop_Tls13_ScramSha256()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        var salt = Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ==");
        var credentials = ScramParameters.Derive("pencil", salt, 4096);

        await using var server = new SmtpEnginePrototype(cert, username => username == "user" ? credentials : null);
        server.Start();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.ServerCertificateValidationCallback = (s, c, ch, err) => true;

        // Connect over implicit TLS
        await client.ConnectAsync("127.0.0.1", server.Port, SecureSocketOptions.SslOnConnect);

        // Verify only SCRAM-SHA-256 is supported
        Assert.Contains("SCRAM-SHA-256", client.AuthenticationMechanisms);
        Assert.DoesNotContain("PLAIN", client.AuthenticationMechanisms);
        Assert.DoesNotContain("LOGIN", client.AuthenticationMechanisms);

        // Authenticate using MailKit standard SaslMechanismScramSha256
        var saslMechanism = new SaslMechanismScramSha256("user", "pencil");
        await client.AuthenticateAsync(saslMechanism);

        Assert.True(client.IsAuthenticated);

        // Send a test message
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Velaris Sender", "sender@velaris.local"));
        message.To.Add(new MailboxAddress("Recipient", "recipient@example.com"));
        message.Subject = "Interoperability Test";
        message.Body = new TextPart("plain") { Text = "Testing MailKit client against Velaris SMTP engine prototype." };

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }

    [Theory]
    [InlineData(SslProtocols.Tls12, true)]
    [InlineData(SslProtocols.Tls13, true)]
    public async Task Smtp_ExplicitTlsVersions_MustNegotiateSuccessfully(SslProtocols clientProtocol, bool expectedSuccess)
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        await using var server = new SmtpEnginePrototype(cert, _ => null);
        server.Start();

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", server.Port);

        using var sslStream = new SslStream(tcpClient.GetStream(), false, (s, c, ch, err) => true);
        var authOptions = new SslClientAuthenticationOptions
        {
            TargetHost = "localhost",
            EnabledSslProtocols = clientProtocol
        };

        if (expectedSuccess)
        {
            await sslStream.AuthenticateAsClientAsync(authOptions);
            Assert.Equal(clientProtocol, sslStream.SslProtocol);
        }
    }

    [Fact]
    public async Task Smtp_Transcript_CapturesSafeExchange_WithoutSecrets()
    {
        using var cert = TestCertificateHelper.CreateSelfSignedCertificate();
        var salt = Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ==");
        var credentials = ScramParameters.Derive("pencil", salt, 4096);

        await using var server = new SmtpEnginePrototype(cert, username => username == "user" ? credentials : null);
        server.Start();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.ServerCertificateValidationCallback = (s, c, ch, err) => true;

        await client.ConnectAsync("127.0.0.1", server.Port, SecureSocketOptions.SslOnConnect);
        await client.AuthenticateAsync(new SaslMechanismScramSha256("user", "pencil"));
        await client.DisconnectAsync(true);

        var transcript = server.Transcript;
        Assert.NotEmpty(transcript);

        // Verify transcript contains commands and responses
        Assert.Contains(transcript, line => line.StartsWith("S: 220"));
        Assert.Contains(transcript, line => line.Contains("250-AUTH SCRAM-SHA-256"));
        Assert.Contains(transcript, line => line.StartsWith("C: AUTH SCRAM-SHA-256"));
        Assert.Contains(transcript, line => line.StartsWith("S: 334"));
        Assert.Contains(transcript, line => line.StartsWith("S: 235 2.7.0"));
        Assert.Contains(transcript, line => line.StartsWith("C: QUIT"));
        Assert.Contains(transcript, line => line.StartsWith("S: 221"));

        // Verify plain password is NEVER in the transcript
        Assert.DoesNotContain(transcript, line => line.Contains("pencil", StringComparison.OrdinalIgnoreCase));
    }
}

