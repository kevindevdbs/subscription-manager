namespace SubscriptionManager.Api.Configuration;

public abstract class RecurringJobOptions
{
    public bool Enabled { get; set; } = true;

    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);
}
