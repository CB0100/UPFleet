using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Xml;
using UPFleet.Models;

namespace UPFleet.Data
{
    public class ApplicationDbContext:DbContext
    {
        //DB Context Class
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Barge> Barges { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PeachtreeExportedArchive> peachtreeExportedArchives { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeachtreeExportedArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Peachtre__3214EC27067633C8");

                entity.ToTable("PeachtreeExportedArchive");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Account).HasMaxLength(255);
                entity.Property(e => e.Closed).HasMaxLength(255);
                entity.Property(e => e.CustomerPo)
                    .HasMaxLength(255)
                    .HasColumnName("\"Customer PO\"");
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Glaccount)
                    .HasMaxLength(255)
                    .HasColumnName("GLaccount");
                entity.Property(e => e.Invitem).HasMaxLength(255);
                entity.Property(e => e.Owner).HasMaxLength(255);
                entity.Property(e => e.Qnty).HasMaxLength(255);
                entity.Property(e => e.SalesOrder).HasMaxLength(255);
                entity.Property(e => e.SalesRepresentativeId)
                    .HasMaxLength(255)
                    .HasColumnName("\"Sales Representative ID\"");
                entity.Property(e => e.SalesTaxAuth).HasMaxLength(255);
                entity.Property(e => e.SalesTaxId)
                    .HasMaxLength(255)
                    .HasColumnName("\"Sales Tax ID\"");
            });
        }
    }
}