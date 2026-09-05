using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Infrastructure.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.OwnsOne(x => x.MonthlyPrice, money =>
        {
            money.Property(x => x.Amount).HasColumnName("monthly_price").HasColumnType("decimal(18,2)");
        });
    }
}
