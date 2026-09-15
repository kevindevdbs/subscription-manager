namespace SubscriptionManager.Api.Configuration;

public class OverdueInvoicesJobOptions
{
    public const string SectionName = "OverdueInvoicesJob";

    public bool Enabled { get; set; } = true;

    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);
}
