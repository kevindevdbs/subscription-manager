using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;


namespace SubscriptionManager.Infrastructure.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("contracts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>();
        // Um cliente só pode ter um contrato aberto por plano. Contratos cancelados
        // ficam de fora para que o mesmo plano possa ser recontratado depois.
        builder.HasIndex(x => new { x.CustomerId, x.PlanId })
            .IsUnique()
            .HasFilter($"[Status] <> {(int)ContractStatus.Cancelled}")
            .HasDatabaseName("IX_contracts_CustomerId_PlanId_Open");
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Plan>()
            .WithMany()
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
