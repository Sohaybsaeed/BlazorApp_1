using FirstBlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Data;

public partial class EnterpriseOperationsContext : DbContext
{
    public EnterpriseOperationsContext()
    {
    }

    public EnterpriseOperationsContext(DbContextOptions<EnterpriseOperationsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LicenseDetail> LicenseDetails { get; set; }

    public virtual DbSet<MasterBankDetail> MasterBankDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=EnterpriseOperations;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LicenseDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LicenseD__3214EC07C7E8A16D");

            entity.HasIndex(e => e.BankId, "IX_LicenseDetails_BankId");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.XmlFile).HasColumnType("xml");
        });

        modelBuilder.Entity<MasterBankDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Master.B__3214EC07422B61FE");

            entity.ToTable("Master.BankDetails");

            entity.Property(e => e.BankName).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
