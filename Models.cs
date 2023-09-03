using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace UrbanHelp
{
    public class Organization
    {
        public int Id { get; set; }
        public int ChiefId { get; set; }
        public Person Chief { get; set; }
        public string Title { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; } = null;
        public string City { get; set; } = null;
        public string ZIPCode { get; set; } = null;
        public string Adress { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public string EMail { get; set; } = null;
        //широта
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        [JsonIgnore]
        public List<OrganizationChange> OrganizationChanges { get; set; } = new();
        [JsonIgnore]
        public List<CourtCase> CourtCases { get; set; }
        [JsonIgnore]
        public List<GovProcurement> GovProcurements { get; set; }
        public DateTime AddDate { get; set; }
    }
    public class OrganizationChange
    {
        public int Id { get; set; }
        public int Organization_Id { get; set; }
        public Organization Organization { get; set; }
        public string Data { get; set; }
        public DateTime AddDate { get; set; }
    }
    public class GovProcurement
    {
        public int Id { get; set; }
        public int Organization_Id { get; set; }
        public Organization Organization { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
    }

    //судебные дела
    public class CourtCase
    {
        public string Id { get; set; }
        public int Organization_Id { get; set; }
        public Organization Organization { get; set; }
        public string Num { get; set; }
        
        //Об интеллектуальной собственности, Об банкротстве и т.д.
        public string Category { get; set; }
        //кем выступаеи истец/ответчик/третье лицо
        public string Type { get; set; }
        //завершенно, разбирается...
        public string Status { get; set; }
        //истец
        public string Plaintiff { get; set; }
        //ответчик
        public string Defendant { get; set; }
    }
 

    public class Relative
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public Person Person { get; set; }
        //степень родства
        public string Degree { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Surname { get; set; } = null;
        public DateTime? DateOfBirth { get; set; } = null;
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
        public string Name { get; set; }
        public string Family { get; set; }
        public string Surname { get; set; } = null;
        public bool Male { get; set; } = true;
        [NotMapped]
        [JsonIgnore]
        public string FIO => $"{Family} {Name?.ToUpper()[..1]}." +
            $"{(string.IsNullOrEmpty(Surname) ? string.Empty:$" {Surname?.ToUpper()?[..1]}.")}" ;
        [NotMapped]
        [JsonIgnore]
        public string FullFIO => $"{Family} {Name}{(string.IsNullOrEmpty(Surname) ? string.Empty : $" {Surname?.ToUpper()}")}";
        public string Country { get; set; } = null;
        public string City { get; set; } = null;
        public string ZIPCode { get; set; } = null;
        public string Adress { get; set; } = null;
        //широта
        public double Latitude { get;  set; }
        public double Longitude { get;  set; }
        public DateTime? DateOfBirth { get; set; } = null;
        public string PhoneNumber { get;  set; } = null;
        public string EMail { get;  set; } = null;               
        [JsonIgnore]
        public List<PersonChange> PersonChanges { get; set; } = new();
        [JsonIgnore]
        public List<PersonFinCondition> FinConditions { get; set; } = new();       
        [JsonIgnore]
        public List<Relative> Relatives { get; set; } = new();
        public DateTime AddDate { get; set; }

    }

    public class PhoneNumber
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public Person Person { get; set; }
        public string Number { get; set; }
    }

    public class EMail
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public Person Person { get; set; }
        public string Value { get; set; }
    }
}
