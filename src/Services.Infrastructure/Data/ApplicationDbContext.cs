using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;

namespace Services.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Preserving exact table names from database
    public DbSet<Appointment> tbl_Appointment { get; set; }
    public DbSet<Customer> tbl_Customer { get; set; }
    public DbSet<Resource> tbl_Resources { get; set; }
    public DbSet<Status> tbl_Status { get; set; }
    public DbSet<TicketStatus> tbl_TicketStatus { get; set; }
    public DbSet<ServiceType> tbl_ServiceType { get; set; }
    public DbSet<Tax> Taxes { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<FormTemplate> FormTemplates { get; set; }
    public DbSet<Items> Items { get; set; }
    public DbSet<User> tbl_User { get; set; }
    public DbSet<Company> tbl_Company { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to match existing database
        modelBuilder.Entity<Appointment>().ToTable("tbl_Appointment", "msSchedulerV3");
        modelBuilder.Entity<Customer>().ToTable("tbl_Customer", "msSchedulerV3");
        modelBuilder.Entity<Resource>().ToTable("tbl_Resources", "msSchedulerV3");
        modelBuilder.Entity<Status>().ToTable("tbl_Status", "msSchedulerV3");
        modelBuilder.Entity<TicketStatus>().ToTable("tbl_TicketStatus", "msSchedulerV3");
        modelBuilder.Entity<ServiceType>().ToTable("tbl_ServiceType", "msSchedulerV3");
        modelBuilder.Entity<User>().ToTable("tbl_User", "XinatorCentral");
        modelBuilder.Entity<Company>().ToTable("tbl_Company", "msSchedulerV3");

        // Configure primary keys to match existing structure
        modelBuilder.Entity<Appointment>().HasKey(a => a.ApptID);
        modelBuilder.Entity<Customer>().HasKey(c => new { c.CustomerID, c.CompanyID });
        modelBuilder.Entity<Status>().HasKey(s => new { s.StatusId, s.CompanyId });
        modelBuilder.Entity<TicketStatus>().HasKey(ts => new { ts.StatusId, ts.CompanyId });
        modelBuilder.Entity<ServiceType>().HasKey(st => new { st.ServiceTypeID, st.CompanyID });
        modelBuilder.Entity<Resource>().HasKey(r => r.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Company>().HasKey(c => c.CompanyID);
    }
}

