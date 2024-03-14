using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using portal_backend.Entities;

namespace portal_backend.EntityConfiguration;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder
            .Property(s => s.Price)
            .HasColumnType("decimal(18, 2)");
    }
}