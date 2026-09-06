using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
        });
        builder.HasIndex(x => new { x.ContractId, x.ReferenceMonth }).IsUnique();
        builder.Property(i => i.Status).HasConversion<int>();

        builder.HasOne<Contract>()
       .WithMany()
       .HasForeignKey(x => x.ContractId)
       .OnDelete(DeleteBehavior.Restrict);
    }
}
