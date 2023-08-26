using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanHelp
{
    public class GovProcurement
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
    }
    public class CourtCase
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Num { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Plaintiff { get; set; }
        public string Defendant { get; set; }
    }
    public class Organization
    {
        public string Id { get; set; }
        public string ChiefId { get; set; }
        public Person Chief { get; set; }
        public string Title { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<CourtCase> CourtCases { get; set; }
        public List<GovProcurement> GovProcurements { get; set; }
        public string Coordinates { get; set; }
        public string OGRN { get; set; } = null;
        public DateTime OGRNDate { get; set; }
        public string INN { get; set; } = null;
        public string KPP { get; set; } = null;
        public DateTime Date { get; set; }
        public string OKPO { get; set; } = null;
        public string OKATO { get; set; } = null;
        public string OKTMO { get; set; } = null;
        public string OKFS { get; set; } = null;
        public string OKOGU { get; set; } = null;
        public DateTime AddDate { get; private set; }

    }
    public class PersonFinCondition
    {
        public string Id { get; set; }
        public string PersonId { get; set; }
        public double Condition { get; set; }
        public int Year { get; set; }
    }
    public class Contact
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; } = null;
        public string Address { get; set; }
        public string PhoneNumber { get; set; } = null;
        public string EMail { get; set; } = null;
        public DateTime AddDate { get; private set; }
    }
    public class PersonChange
    {
        public string Id { get; set; }
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Surname { get; set; } = null;

        public Contact Contact { get; set; } = null;
        public DateTime AddDate { get; private set; }
    }
    public class Relative
    {
        public string Id { get; set; }
        public string PersonId { get; set; }
        public string Degree { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Surname { get; set; } = null;
        public DateTime DateOfBirth { get; set; }
    }
    public class Person
    {
        public string Id { get; set; }
        public string PersonId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<PersonChange> ListChange { get; set; }
        public List<PersonFinCondition> FinConditions { get; set; } = null;
        public List<Relative> Relatives { get; set; } = null;
        public DateTime AddDate { get; private set; }

    }
}
