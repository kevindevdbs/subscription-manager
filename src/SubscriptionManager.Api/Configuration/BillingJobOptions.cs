namespace SubscriptionManager.Api.Configuration;

public class BillingJobOptions
{
    public const string SectionName = "BillingJob";

    public bool Enabled { get; set; } = true;

    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);
}
