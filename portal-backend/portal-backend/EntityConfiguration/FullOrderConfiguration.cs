using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using portal_backend.Entities;

namespace portal_backend.EntityConfiguration;

public class FullOrderConfiguration : IEntityTypeConfiguration<FullOrder>
{
    public void Configure(EntityTypeBuilder<FullOrder> builder)
    {
    }
}