using System.Net;

namespace SubscriptionManager.Domain.Exceptions;

public class ConflictException : SubscriptionManagerException
{
    private readonly string _message;

    public ConflictException(string message)
    {
        _message = message;
    }

    public override List<string> GetErrorMessages() => [_message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Conflict;
}
