using FirstBlazorApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RetailChannel.Models.Model;

public partial class IBRetailDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public IBRetailDbContext()
    {
    }
    public IBRetailDbContext(DbContextOptions<IBRetailDbContext> options) : base(options)
    {
    }

    public virtual DbSet<KillSwitchLog> KillSwitchLogs { get; set; }
    public virtual DbSet<UserActivityHistory> UserActivityHistory { get; set; }
    public virtual DbSet<PermissionTemplate> PermissionTemplate { get; set; }
    public virtual DbSet<PermissionTemplateDetail> PermissionTemplateDetail { get; set; }
    public virtual DbSet<FormDetail> FormDetail { get; set; }
    public virtual DbSet<MakerChecker> MakerChecker { get; set; }
    public virtual DbSet<UserAccess> UserAccess { get; set; }
    public virtual DbSet<ListItems> ListItems { get; set; }
    public virtual DbSet<ApplicationFunctionalities> ApplicationFunctionalities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        modelBuilder.Entity<KillSwitchLog>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<UserAccess>(entity =>
        {
            entity.Property(e => e.FormName).HasMaxLength(50);

            entity.Property(e => e.UserId).HasMaxLength(50);
        });

        modelBuilder.Entity<ListItems>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.Property(e => e.Text)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ApplicationFunctionalities>(entity =>
        {
            entity.Property(e => e.ActionMethodName).HasMaxLength(50);
            entity.Property(e => e.MenuReferenceName).HasMaxLength(150);

            entity.Property(e => e.FunctionalityName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserActivityHistory>(entity =>
        {
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.ActionMethod)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.NewValueJson).IsUnicode(false);

            entity.Property(e => e.OldValueJson).IsUnicode(false);
        });

        modelBuilder.Entity<PermissionTemplate>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.TemplateName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FormDetail>(entity =>
        {
            entity.Property(e => e.ActionName).HasMaxLength(50);

            entity.Property(e => e.ControllerName).HasMaxLength(50);

            entity.Property(e => e.DisplayName).HasMaxLength(50);

            entity.Property(e => e.FormName).HasMaxLength(50);

            entity.Property(e => e.IconCode).HasMaxLength(40);
        });

        modelBuilder.Entity<PermissionTemplateDetail>(entity =>
        {
            entity.Property(e => e.FormName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Template)
                .WithMany(p => p.PermissionTemplateDetails)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PermissionTemplateDetail_PermissionTemplate");
        });

        modelBuilder.Entity<MakerChecker>(entity =>
        {
            entity.ToTable("MakerChecker");

            entity.Property(e => e.CheckerDate).HasColumnType("datetime");

            entity.Property(e => e.MakerDate).HasColumnType("datetime");

            entity.Property(e => e.NewValueJson).IsUnicode(false);

            entity.Property(e => e.Notes).HasMaxLength(200);

            entity.Property(e => e.OldValueJson).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
