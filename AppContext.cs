using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Telerik.Barcode;

namespace UrbanHelp
{
    public class AppContext : DbContext
    {
        
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<OrganizationChange> OrganizationChanges { get; set; } = null!;
        public DbSet<GovProcurement> GovProcurements { get; set; } = null!;
        public DbSet<CourtCase> CourtCases { get; set; } = null!;
        public DbSet<PersonFinCondition> PersonFinConditions { get; set; } = null!;
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<PersonChange> PersonChanges { get; set; } = null!;
        public DbSet<Relative> Relatives { get; set; } = null!;
        public AppContext() 
        {

            Database.EnsureDeleted();           

            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=UrbanHelp.sqlite");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Person>(PersonConfigure);
            modelBuilder.Entity<CourtCase>(CourtCaseConfigure);
            modelBuilder.Entity<Organization>(OrganizationConfigure);
            modelBuilder.Entity<OrganizationChange>(OrganizationChangeConfigure);
            modelBuilder.Entity<PersonFinCondition>(PersonFinConditionConfigure);            
            modelBuilder.Entity<PersonChange>(PersonChangeConfigure);
            modelBuilder.Entity<Relative>(RelativeConfigure);
            modelBuilder.Entity<GovProcurement>(GovProcurementConfigure);
            
        }


        private void OrganizationChangeConfigure(EntityTypeBuilder<OrganizationChange> builder)
        {
            builder.ToTable("OrganizationChanges").HasKey(p => p.Id);
            builder.Property(o => o.AddDate).HasDefaultValueSql("DATETIME('now')");
            builder.HasOne(o => o.Organization)
                .WithMany(c => c.OrganizationChanges)
                .HasForeignKey(o => o.Organization_Id);
        }

        private void GovProcurementConfigure(EntityTypeBuilder<GovProcurement> builder)
        {
            builder.ToTable("GovProcurements").HasKey(p => p.Id);
            builder.HasOne(o => o.Organization)
                .WithMany(g => g.GovProcurements)
                .HasForeignKey(o => o.Organization_Id);
        }

        private void RelativeConfigure(EntityTypeBuilder<Relative> builder)
        {
            builder.ToTable("Relatives").HasKey(p => p.Id);
            builder.HasOne(p => p.Person)
                .WithMany(c => c.Relatives)
                .HasForeignKey(p => p.Person_Id);
        }

        private void PersonChangeConfigure(EntityTypeBuilder<PersonChange> builder)
        {
            builder.ToTable("PersonChanges").HasKey(p => p.Id);
            builder.Property(p => p.AddDate).HasDefaultValueSql("DATETIME('now')");
            builder.HasOne(p => p.Person)
                .WithMany(c => c.PersonChanges)
                .HasForeignKey(p => p.Person_Id);
        }

       

        private void PersonFinConditionConfigure(EntityTypeBuilder<PersonFinCondition> builder)
        {
            builder.ToTable("FinConditions").HasKey(p => p.Id);
            builder.HasOne(p => p.Person)
                .WithMany(c => c.FinConditions)
                .HasForeignKey(p => p.Person_Id);
        }

        private void OrganizationConfigure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organizations").HasKey(p => p.Id);
            builder.Property(o => o.AddDate).HasDefaultValueSql("DATETIME('now')");
            builder.Property(o => o.Latitude).HasDefaultValue(0.0);
            builder.Property(o => o.Longitude).HasDefaultValue(0.0);

        }

        private void CourtCaseConfigure(EntityTypeBuilder<CourtCase> builder)
        {
            builder.ToTable("CourtCases").HasKey(p => p.Id);
            builder.HasOne(o => o.Organization)
                .WithMany(c => c.CourtCases)
                .HasForeignKey(o => o.Organization_Id);
        }

        private void PersonConfigure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons").HasKey(p=>p.Id);
            
            builder.Property(p=>p.AddDate).HasDefaultValueSql("DATETIME('now')");
            builder.Property(p => p.Latitude).HasDefaultValue(0.0);
            builder.Property(p => p.Longitude).HasDefaultValue(0.0);


        }

    }
}
