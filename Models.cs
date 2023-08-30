using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace UrbanHelp
{
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
    //    public string Id { get; set; }
    //    public string ChiefId { get; set; }
    //    public Person Chief { get; set; }
    //    public string Title { get; set; }
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
    //    public DateTime AddDate { get; private set; }

    //}

    
    public class Relative
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public Person Person { get; set; }
        public string Degree { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Surname { get; set; } = null;
        public DateTime DateOfBirth { get; set; }
    }
    public class PersonFinCondition
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public Person Person { get; set; }
        public double Condition { get; set; }
        public int Year { get; set; }
    }
    public class PersonChange
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public Person Person { get; set; }
        public string Data { get; set; }
        public DateTime AddDate { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        //public int Relativ_Id { get; set; }
        //public string Degree { get; set; } = null;
        public string Name { get; set; }
        public string Family { get; set; }
        public string Surname { get; set; } = null;

        [NotMapped]
        [JsonIgnore]
        public string FIO => $"{Family} {Name.ToUpper().Substring(0,1)}.{(string.IsNullOrEmpty(Surname) ? string.Empty:$" {Surname?.ToUpper()?.Substring(0, 1)}.")}" ;

        public string Country { get; set; } = null;
        public string City { get; set; } = null;
        public string ZIPCode { get; set; } = null;
        public string Address { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public string EMail { get; set; } = null;
        public double Latitude { get; private set; } 
        public double Longitude { get; private set; }
        
        public DateTime DateOfBirth { get; set; }
        [JsonIgnore]
        public List<PersonChange> PersonChanges { get; set; } = new();
        [JsonIgnore]
        public List<PersonFinCondition> FinConditions { get; set; } = new();
        [JsonIgnore]
        public List<Relative> Relatives { get; set; } = new();
        public DateTime AddDate { get; set; }

    }
}
