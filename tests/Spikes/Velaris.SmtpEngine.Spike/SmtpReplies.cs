namespace Velaris.SmtpEngine.Spike;

public static class SmtpReplies
{
    public const string ServiceReady = "220 velaris.local Velaris SMTP Submission Service Ready\r\n";
    public const string Ok = "250 2.0.0 OK\r\n";
    public const string StartMailInput = "354 Start mail input; end with <CRLF>.<CRLF>\r\n";
    public const string Bye = "221 2.0.0 Service closing transmission channel\r\n";

    public const string LineTooLong = "500 5.5.2 Line too long\r\n";
    public const string SyntaxError = "500 5.5.2 Syntax error, command unrecognized\r\n";
    public const string SyntaxErrorInParameters = "501 5.5.2 Syntax error in parameters or arguments\r\n";
    public const string CommandNotImplemented = "502 5.5.1 Command not implemented\r\n";
    public const string BadSequenceOfCommands = "503 5.5.1 Bad sequence of commands\r\n";
    public const string UnrecognizedAuthType = "504 5.5.4 Unrecognized authentication type\r\n";
    public const string AuthenticationRequired = "530 5.7.0 Authentication required\r\n";
    public const string AuthenticationCredentialsInvalid = "535 5.7.8 Authentication credentials invalid\r\n";
    public const string TooManyAuthFailures = "421 4.7.0 Too many authentication failures, closing transmission channel\r\n";

    public static string EhloResponse(string domain) =>
        $"250-{domain}\r\n250-AUTH SCRAM-SHA-256\r\n250-8BITMIME\r\n250-SIZE 26214400\r\n250 OK\r\n";

    public static string AuthChallenge(string base64Data) =>
        $"334 {base64Data}\r\n";

    public static string AuthSuccess(string base64Data) =>
        $"235 2.7.0 Authentication successful; {base64Data}\r\n";
}
