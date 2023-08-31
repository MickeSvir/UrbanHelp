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
        //public DbSet<PhoneNumber> PhoneNumbers { get; set; } = null!;
        //public DbSet<EMail> EMails { get; set; } = null!;
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
            //modelBuilder.Entity<PhoneNumber>(PhoneNumberConfigure);
            //modelBuilder.Entity<EMail>(EMailConfigure);
        }

        //private void EMailConfigure(EntityTypeBuilder<EMail> builder)
        //{
        //    builder.ToTable("Relatives").HasKey(p => p.Id);
        //    builder.HasOne(p => p.Person)
        //        .WithMany(c => c.Relatives)
        //        .HasForeignKey(p => p.Person_Id);
        //    ;
        //}

        //private void PhoneNumberConfigure(EntityTypeBuilder<PhoneNumber> builder)
        //{
        //    throw new NotImplementedException();
        //}

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

        //private void ContactConfigure(EntityTypeBuilder<Contact> builder)
        //{
        //    throw new NotImplementedException();
        //}

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
            builder.Property(o => o.DateOfBirth).HasDefaultValue("01.01.0001");
        }

        private void CourtCaseConfigure(EntityTypeBuilder<CourtCase> builder)
        {
            builder.ToTable("CourtCases").HasKey(p => p.Id);
            builder.HasOne(o => o.Organization)
                .WithMany(c => c.CourtCases)
                .HasForeignKey(o => o.Organization_Id);
        }

        // конфигурация для типа User
        private void PersonConfigure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons").HasKey(p=>p.Id);
            
            builder.Property(p=>p.AddDate).HasDefaultValueSql("DATETIME('now')");
            builder.Property(p => p.Latitude).HasDefaultValue(0.0);
            builder.Property(p => p.Longitude).HasDefaultValue(0.0);
            builder.Property(p => p.DateOfBirth).HasDefaultValue("01.01.0001");


        }
        // конфигурация для типа Company
        //public void CompanyConfigure(EntityTypeBuilder<Company> builder)
        //{
        //    builder.ToTable("Enterprises").Property(c => c.Name).IsRequired();
        //}

        //public class GovProcurement
        //{
        //    public string Id { get; set; }
        //    public string OrganizationId { get; set; }
        //    public string Name { get; set; }
        //    public long Price { get; set; }
        //}
        //public class CourtCase
        //{
        //    public string Id { get; set; }
        //    public string OrganizationId { get; set; }
        //    public string Num { get; set; }
        //    public string Category { get; set; }
        //    public string Type { get; set; }
        //    public string Status { get; set; }
        //    public string Plaintiff { get; set; }
        //    public string Defendant { get; set; }
        //}
        //public class Organization
        //{
        //    public string Id { get; set;}
        //    public string ChiefId { get; set;}
        //    public Person Chief { get; set;}
        //    public string Title { get; set;}
        //    public List<Contact> Contacts { get; set; }
        //    public List<CourtCase> CourtCases { get; set; }
        //    public List<GovProcurement> GovProcurements { get; set; }
        //    public string Coordinates { get; set; }
        //    public string OGRN { get; set; } = null;
        //    public DateTime OGRNDate { get; set; }
        //    public string INN { get; set; } = null;
        //    public string KPP { get; set; } = null;
        //    public DateTime Date { get; set; }
        //    public string OKPO { get; set; } = null;
        //    public string OKATO { get; set; } = null;
        //    public string OKTMO { get; set; } = null;
        //    public string OKFS { get; set; } = null;
        //    public string OKOGU { get; set; } = null;
        //    public DateTime AddDate { get;private set; }

        //}
        //public class PersonFinCondition
        //{
        //    public string Id { get; set; }
        //    public string PersonId { get; set; }
        //    public double Condition { get; set; }
        //    public int Year { get; set; }
        //}
        //public class Contact
        //{
        //    public string Id { get; set; }
        //    public string ExternalId { get; set; }
        //    public string Country { get; set; }
        //    public string City { get; set; }
        //    public string ZIPCode { get; set; } = null;
        //    public string Address { get; set; }
        //    public string PhoneNumber { get; set; } = null;
        //    public string EMail { get; set; } = null;
        //    public DateTime AddDate { get; private set; }
        //}
        //public class PersonChange
        //{
        //    public string Id { get; set; }
        //    public string PersonId { get; set; }
        //    public string Name { get; set; }
        //    public string Family { get; set; }
        //    public string Surname { get; set; } = null;
            
        //    public Contact Contact { get; set; } = null;
        //    public DateTime AddDate { get; private set; }
        //}
        //public class Relative
        //{
        //    public string Id { get; set; }
        //    public string PersonId { get; set; }
        //    public string Degree { get; set; }
        //    public string Name { get; set; }
        //    public string Family { get; set; }
        //    public string Surname { get; set; } = null;
        //    public DateTime DateOfBirth { get; set; }
        //}
        //public class Person
        //{
        //    public string Id { get; set; }
        //    public string PersonId { get; set; }
        //    public DateTime DateOfBirth { get; set; }
        //    public List<PersonChange> ListChange { get; set; }
        //    public List<PersonFinCondition> FinConditions { get; set; } = null;
        //    public List<Relative> Relatives { get; set; } = null;            
        //    public DateTime AddDate { get; private set; }

        //}
    }
}
