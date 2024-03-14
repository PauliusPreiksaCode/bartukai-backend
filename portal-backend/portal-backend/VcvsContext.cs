using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.EntityConfiguration;

namespace portal_backend;

public partial class VcvsContext : DbContext
{
    public VcvsContext()
    {
    }

    public VcvsContext(DbContextOptions<VcvsContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Admin> Admin { get; set; }
    public virtual DbSet<Customer> Customer { get; set; }
    public virtual DbSet<Equipment> Equipment { get; set; }
    public virtual DbSet<FullOrder> FullOrder { get; set; }
    public virtual DbSet<Order> Order { get; set; }
    public virtual DbSet<Room> Room { get; set; }
    public virtual DbSet<Service> Service { get; set; }
    public virtual DbSet<ServiceCategory> ServiceCategory { get; set; }
    public virtual DbSet<ServiceServiceCategory> ServiceServiceCategories { get; set; }
    public virtual DbSet<Specialist> Specialist { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration["SqlServer:ConnectionString"];
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        
        modelBuilder.Entity<ServiceServiceCategory>().HasKey(u => new 
        { 
            u.ServiceId, 
            u.ServiceCategoryId 
        });
        
        modelBuilder.HasSequence<int>("SequenceNumbers")
            .StartsAt(1)
            .IncrementsBy(1);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}