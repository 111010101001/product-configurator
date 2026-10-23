using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Data
{
    public class ConfiguratorDbContext(DbContextOptions<ConfiguratorDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<OptionGroup> OptionGroups => Set<OptionGroup>();
        public DbSet<ConfigurationOption> ConfigurationOptions => Set<ConfigurationOption>();
        public DbSet<CompatibilityRule> CompatibilityRules => Set<CompatibilityRule>();
        public DbSet<ConfigurationSelection> ConfigurationSelections => Set<ConfigurationSelection>();
        public DbSet<SavedConfiguration> SavedConfigurations => Set<SavedConfiguration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Sku).HasMaxLength(50).IsUnicode(false);
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<OptionGroup>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);

            });

            modelBuilder.Entity<ConfigurationOption>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.PriceDelta).HasPrecision(18, 2);


            });

            modelBuilder.Entity<ConfigurationSelection>(entity =>
            {
                entity.HasKey(e => new { e.SavedConfigurationId, e.ConfigurationOptionId });
                entity.HasOne(e => e.ConfigurationOption)
                  .WithMany()                              // empty: ConfigurationOption has no collection of selections
                  .HasForeignKey(e => e.ConfigurationOptionId)
                  .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<SavedConfiguration>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<CompatibilityRule>(entity =>
            {
                entity.HasIndex(e => new { e.SourceOptionId, e.TargetOptionId, e.RuleType }).IsUnique();
                entity.HasOne(e => e.SourceOption)
                  .WithMany()                              // empty: ConfigurationOption has no collection of selections
                  .HasForeignKey(e => e.SourceOptionId)
                  .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.TargetOption)
                  .WithMany()                              // empty: ConfigurationOption has no collection of selections
                  .HasForeignKey(e => e.TargetOptionId)
                  .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_CompatibilityRule_ExcludesOrdered", "[RuleType] <> 2 OR [SourceOptionId] < [TargetOptionId]");
                    t.HasCheckConstraint("CK_CompatibilityRule_NotSelf", "[SourceOptionId] <> [TargetOptionId]");
                });

            });
        }
    }
}
