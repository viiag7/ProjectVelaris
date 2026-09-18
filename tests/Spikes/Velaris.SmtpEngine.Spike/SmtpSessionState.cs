namespace Velaris.SmtpEngine.Spike;

public enum SmtpSessionState
{
    Connected,
    Greeted,
    Authenticating,
    Authenticated,
    EnvelopeStarted,
    RecipientAccepted,
    Data,
    Closed
}
