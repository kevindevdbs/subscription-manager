using System.Net;

namespace SubscriptionManager.Domain.Exceptions;

public abstract class SubscriptionManagerException : System.Exception
{
    public abstract HttpStatusCode GetStatusCode();
    public abstract List<string> GetErrorMessages();
}
