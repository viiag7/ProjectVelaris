using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Velaris.SmtpEngine.Spike;

public sealed class SmtpEnginePrototype : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly X509Certificate2 _certificate;
    private readonly Func<string, ScramParameters?> _credentialLookup;
    private readonly CancellationTokenSource _cts = new();
    private readonly List<string> _transcript = new();
    private Task? _listenerTask;

    public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;
    public IReadOnlyList<string> Transcript
    {
        get
        {
            lock (_transcript)
            {
                return _transcript.ToList();
            }
        }
    }

    public SmtpEnginePrototype(
        X509Certificate2 certificate,
        Func<string, ScramParameters?> credentialLookup,
        int port = 0)
    {
        _certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
        _credentialLookup = credentialLookup ?? throw new ArgumentNullException(nameof(credentialLookup));
        _listener = new TcpListener(IPAddress.Loopback, port);
    }

    public void Start()
    {
        _listener.Start();
        _listenerTask = Task.Run(() => AcceptConnectionsAsync(_cts.Token));
    }

    public async Task StopAsync()
    {
        _cts.Cancel();
        _listener.Stop();
        if (_listenerTask is not null)
        {
            try
            {
                await _listenerTask;
            }
            catch (OperationCanceledException)
            {
                // Expected on stop
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _cts.Dispose();
    }

    private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var socket = await _listener.AcceptSocketAsync(cancellationToken);
                _ = Task.Run(() => HandleClientAsync(socket, cancellationToken), cancellationToken);
            }
            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task HandleClientAsync(Socket socket, CancellationToken cancellationToken)
    {
        using var networkStream = new NetworkStream(socket, ownsSocket: true);
        using var sslStream = new SslStream(networkStream, leaveInnerStreamOpen: false);

        try
        {
            // Enforce implicit TLS with TLS 1.2 or TLS 1.3
            var sslOptions = new SslServerAuthenticationOptions
            {
                ServerCertificate = _certificate,
                ClientCertificateRequired = false,
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck
            };

            await sslStream.AuthenticateAsServerAsync(sslOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            LogTranscript($"TLS Handshake failed: {ex.Message}");
            return;
        }

        using var reader = new StreamReader(sslStream, Encoding.ASCII, leaveOpen: true);
        using var writer = new StreamWriter(sslStream, Encoding.ASCII, leaveOpen: true) { AutoFlush = true, NewLine = "\r\n" };

        var state = SmtpSessionState.Connected;
        var authFailures = 0;
        ScramServerSession? scramSession = null;
        ScramParameters? activeParameters = null;

        await writer.WriteAsync(SmtpReplies.ServiceReady);
        LogTranscript($"S: {SmtpReplies.ServiceReady.TrimEnd()}");

        while (!cancellationToken.IsCancellationRequested && state != SmtpSessionState.Closed)
        {
            string? line;
            try
            {
                line = await reader.ReadLineAsync(cancellationToken);
            }
            catch (Exception)
            {
                break;
            }

            if (line is null)
            {
                break;
            }

            LogTranscript($"C: {line}");

            if (line.Length > 1000)
            {
                await writer.WriteAsync(SmtpReplies.LineTooLong);
                LogTranscript($"S: {SmtpReplies.LineTooLong.TrimEnd()}");
                continue;
            }

            // In Authenticating state, handle client-final response
            if (state == SmtpSessionState.Authenticating)
            {
                if (line.Equals("*", StringComparison.Ordinal))
                {
                    // Client abort
                    state = SmtpSessionState.Greeted;
                    scramSession = null;
                    activeParameters = null;
                    await writer.WriteAsync(SmtpReplies.BadSequenceOfCommands);
                    LogTranscript($"S: {SmtpReplies.BadSequenceOfCommands.TrimEnd()}");
                    continue;
                }

                string clientFinal;
                try
                {
                    clientFinal = Encoding.UTF8.GetString(Convert.FromBase64String(line));
                }
                catch (FormatException)
                {
                    authFailures++;
                    state = SmtpSessionState.Greeted;
                    scramSession = null;
                    activeParameters = null;
                    if (authFailures >= 3)
                    {
                        await writer.WriteAsync(SmtpReplies.TooManyAuthFailures);
                        LogTranscript($"S: {SmtpReplies.TooManyAuthFailures.TrimEnd()}");
                        break;
                    }
                    await writer.WriteAsync(SmtpReplies.SyntaxErrorInParameters);
                    LogTranscript($"S: {SmtpReplies.SyntaxErrorInParameters.TrimEnd()}");
                    continue;
                }

                if (scramSession is null || activeParameters is null)
                {
                    authFailures++;
                    state = SmtpSessionState.Greeted;
                    await writer.WriteAsync(SmtpReplies.AuthenticationCredentialsInvalid);
                    LogTranscript($"S: {SmtpReplies.AuthenticationCredentialsInvalid.TrimEnd()}");
                    continue;
                }

                var (success, serverFinal) = scramSession.ProcessClientFinal(
                    clientFinal,
                    activeParameters.StoredKey,
                    activeParameters.ServerKey);

                if (success)
                {
                    state = SmtpSessionState.Authenticated;
                    authFailures = 0;
                    var serverFinalBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(serverFinal));
                    var reply = SmtpReplies.AuthSuccess(serverFinalBase64);
                    await writer.WriteAsync(reply);
                    LogTranscript($"S: {reply.TrimEnd()}");
                }
                else
                {
                    authFailures++;
                    state = SmtpSessionState.Greeted;
                    scramSession = null;
                    activeParameters = null;
                    if (authFailures >= 3)
                    {
                        await writer.WriteAsync(SmtpReplies.TooManyAuthFailures);
                        LogTranscript($"S: {SmtpReplies.TooManyAuthFailures.TrimEnd()}");
                        break;
                    }
                    await writer.WriteAsync(SmtpReplies.AuthenticationCredentialsInvalid);
                    LogTranscript($"S: {SmtpReplies.AuthenticationCredentialsInvalid.TrimEnd()}");
                }
                continue;
            }

            var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var command = parts.Length > 0 ? parts[0].ToUpperInvariant() : string.Empty;
            var argument = parts.Length > 1 ? parts[1].Trim() : string.Empty;

            switch (command)
            {
                case "EHLO":
                    state = SmtpSessionState.Greeted;
                    var domain = string.IsNullOrWhiteSpace(argument) ? "client" : argument;
                    var ehloReply = SmtpReplies.EhloResponse("velaris.local");
                    await writer.WriteAsync(ehloReply);
                    LogTranscript($"S: {ehloReply.TrimEnd()}");
                    break;

                case "HELO":
                    state = SmtpSessionState.Greeted;
                    var heloReply = "250 velaris.local\r\n";
                    await writer.WriteAsync(heloReply);
                    LogTranscript($"S: {heloReply.TrimEnd()}");
                    break;

                case "AUTH":
                    if (state == SmtpSessionState.Connected)
                    {
                        await writer.WriteAsync(SmtpReplies.BadSequenceOfCommands);
                        LogTranscript($"S: {SmtpReplies.BadSequenceOfCommands.TrimEnd()}");
                        break;
                    }

                    if (state is SmtpSessionState.Authenticated or SmtpSessionState.EnvelopeStarted or SmtpSessionState.RecipientAccepted)
                    {
                        await writer.WriteAsync(SmtpReplies.BadSequenceOfCommands);
                        LogTranscript($"S: {SmtpReplies.BadSequenceOfCommands.TrimEnd()}");
                        break;
                    }

                    var authArgs = argument.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    var mechanism = authArgs.Length > 0 ? authArgs[0].ToUpperInvariant() : string.Empty;
                    var initialResponse = authArgs.Length > 1 ? authArgs[1] : null;

                    if (mechanism != "SCRAM-SHA-256")
                    {
                        authFailures++;
                        if (authFailures >= 3)
                        {
                            await writer.WriteAsync(SmtpReplies.TooManyAuthFailures);
                            LogTranscript($"S: {SmtpReplies.TooManyAuthFailures.TrimEnd()}");
                            state = SmtpSessionState.Closed;
                            break;
                        }
                        await writer.WriteAsync(SmtpReplies.UnrecognizedAuthType);
                        LogTranscript($"S: {SmtpReplies.UnrecognizedAuthType.TrimEnd()}");
                        break;
                    }

                    scramSession = new ScramServerSession();

                    string clientFirst;
                    if (initialResponse is not null)
                    {
                        try
                        {
                            clientFirst = Encoding.UTF8.GetString(Convert.FromBase64String(initialResponse));
                        }
                        catch (FormatException)
                        {
                            await writer.WriteAsync(SmtpReplies.SyntaxErrorInParameters);
                            LogTranscript($"S: {SmtpReplies.SyntaxErrorInParameters.TrimEnd()}");
                            break;
                        }
                    }
                    else
                    {
                        // Prompt client for initial response
                        await writer.WriteAsync(SmtpReplies.AuthChallenge(string.Empty));
                        LogTranscript($"S: {SmtpReplies.AuthChallenge(string.Empty).TrimEnd()}");
                        var rawClientFirst = await reader.ReadLineAsync(cancellationToken);
                        if (rawClientFirst is null || rawClientFirst == "*")
                        {
                            state = SmtpSessionState.Greeted;
                            await writer.WriteAsync(SmtpReplies.BadSequenceOfCommands);
                            LogTranscript($"S: {SmtpReplies.BadSequenceOfCommands.TrimEnd()}");
                            break;
                        }
                        LogTranscript($"C: {rawClientFirst}");
                        try
                        {
                            clientFirst = Encoding.UTF8.GetString(Convert.FromBase64String(rawClientFirst));
                        }
                        catch (FormatException)
                        {
                            await writer.WriteAsync(SmtpReplies.SyntaxErrorInParameters);
                            LogTranscript($"S: {SmtpReplies.SyntaxErrorInParameters.TrimEnd()}");
                            break;
                        }
                    }

                    try
                    {
                        // Parse client first to extract username
                        // For dummy/unknown users, supply a deterministic dummy salt to prevent timing attacks
                        var dummySalt = new byte[16];
                        var serverFirstPreview = scramSession.ProcessClientFirst(clientFirst, dummySalt, 4096);
                        var username = scramSession.Username!;

                        activeParameters = _credentialLookup(username);
                        if (activeParameters is null)
                        {
                            // Fail closed after full exchange to resist enumeration, or challenge with dummy
                            // For spike simplicity, use dummy parameters that fail verification
                            activeParameters = new ScramParameters(dummySalt, 4096, new byte[32], new byte[32]);
                        }

                        // Re-process with real salt and iterations
                        scramSession = new ScramServerSession();
                        var serverFirst = scramSession.ProcessClientFirst(
                            clientFirst,
                            activeParameters.Salt,
                            activeParameters.Iterations);

                        var serverFirstBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(serverFirst));
                        var challengeReply = SmtpReplies.AuthChallenge(serverFirstBase64);
                        await writer.WriteAsync(challengeReply);
                        LogTranscript($"S: {challengeReply.TrimEnd()}");
                        state = SmtpSessionState.Authenticating;
                    }
                    catch (FormatException)
                    {
                        authFailures++;
                        await writer.WriteAsync(SmtpReplies.SyntaxErrorInParameters);
                        LogTranscript($"S: {SmtpReplies.SyntaxErrorInParameters.TrimEnd()}");
                    }
                    break;

                case "STARTTLS":
                    await writer.WriteAsync(SmtpReplies.CommandNotImplemented);
                    LogTranscript($"S: {SmtpReplies.CommandNotImplemented.TrimEnd()}");
                    break;

                case "MAIL":
                    if (state < SmtpSessionState.Authenticated)
                    {
                        await writer.WriteAsync(SmtpReplies.AuthenticationRequired);
                        LogTranscript($"S: {SmtpReplies.AuthenticationRequired.TrimEnd()}");
                        break;
                    }
                    if (state != SmtpSessionState.Authenticated)
                    {
                        await writer.WriteAsync(SmtpReplies.BadSequenceOfCommands);
                        LogTranscript($"S: {SmtpReplies.BadSequenceOfCommands.TrimEnd()}");
                        break;
                    }
                    if (!argument.StartsWith("FROM:", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteAsync(SmtpReplies.SyntaxErrorInParameters);
                        LogTranscript($"S: {SmtpReplies.SyntaxErrorInParameters.TrimEnd()}");
                        break;
                    }
                    state = SmtpSessionState.EnvelopeStarted;
                    await writer.WriteAsync(SmtpReplies.Ok);
                    LogTranscript($"S: {SmtpReplies.Ok.TrimEnd()}");
                    break;

                case "RCPT":
                    if (state is not (SmtpSessionState.EnvelopeStarted or SmtpSessionState.RecipientAccepted))
                    {
                        await writer.WriteAsync(state < SmtpSessionState.Authenticated ? SmtpReplies.AuthenticationRequired : SmtpReplies.BadSequenceOfCommands);
                        LogTranscript($"S: {(state < SmtpSessionState.Authenticated ? SmtpReplies.AuthenticationRequired : SmtpReplies.BadSequenceOfCommands).TrimEnd()}");
                        break;
                    }
                    if (!argument.StartsWith("TO:", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteAsync(SmtpReplies.SyntaxErrorInParameters);
                        LogTranscript($"S: {SmtpReplies.SyntaxErrorInParameters.TrimEnd()}");
                        break;
                    }
                    state = SmtpSessionState.RecipientAccepted;
                    await writer.WriteAsync(SmtpReplies.Ok);
                    LogTranscript($"S: {SmtpReplies.Ok.TrimEnd()}");
                    break;

                case "DATA":
                    if (state != SmtpSessionState.RecipientAccepted)
                    {
                        await writer.WriteAsync(state < SmtpSessionState.Authenticated ? SmtpReplies.AuthenticationRequired : SmtpReplies.BadSequenceOfCommands);
                        LogTranscript($"S: {(state < SmtpSessionState.Authenticated ? SmtpReplies.AuthenticationRequired : SmtpReplies.BadSequenceOfCommands).TrimEnd()}");
                        break;
                    }
                    await writer.WriteAsync(SmtpReplies.StartMailInput);
                    LogTranscript($"S: {SmtpReplies.StartMailInput.TrimEnd()}");

                    state = SmtpSessionState.Data;
                    while (true)
                    {
                        var dataLine = await reader.ReadLineAsync(cancellationToken);
                        if (dataLine is null || dataLine == ".")
                        {
                            break;
                        }
                    }
                    state = SmtpSessionState.Authenticated;
                    await writer.WriteAsync("250 2.0.0 OK: message queued\r\n");
                    LogTranscript("S: 250 2.0.0 OK: message queued");
                    break;

                case "RSET":
                    if (state > SmtpSessionState.Authenticated)
                    {
                        state = SmtpSessionState.Authenticated;
                    }
                    await writer.WriteAsync(SmtpReplies.Ok);
                    LogTranscript($"S: {SmtpReplies.Ok.TrimEnd()}");
                    break;

                case "NOOP":
                    await writer.WriteAsync(SmtpReplies.Ok);
                    LogTranscript($"S: {SmtpReplies.Ok.TrimEnd()}");
                    break;

                case "QUIT":
                    await writer.WriteAsync(SmtpReplies.Bye);
                    LogTranscript($"S: {SmtpReplies.Bye.TrimEnd()}");
                    state = SmtpSessionState.Closed;
                    break;

                default:
                    await writer.WriteAsync(SmtpReplies.SyntaxError);
                    LogTranscript($"S: {SmtpReplies.SyntaxError.TrimEnd()}");
                    break;
            }
        }
    }

    private void LogTranscript(string message)
    {
        lock (_transcript)
        {
            _transcript.Add(message);
        }
    }
}
