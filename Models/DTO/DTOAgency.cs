using System;
using System.Collections.Generic;

namespace Api.Models.DTO
{
    public class DTOAgency
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int AgencyStatusId { get; set; }
        public string AgencyStatusName { get; set; } = "";
        public int SdrNumber { get; set; }
        public int UieNumber { get; set; }
        public int EinNumber { get; set; }
        public string Address { get; set; } = "";
        public int ZipCode { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; } = "";
        public int RegionId { get; set; }
        public string RegionName { get; set; } = "";
        public string PostalAddress { get; set; } = "";
        public int PostalZipCode { get; set; }
        public int? PostalCityId { get; set; }
        public string PostalCityName { get; set; } = "";
        public int? PostalRegionId { get; set; }
        public string PostalRegionName { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public bool NonProfit { get; set; }
        public bool FederalFundsDenied { get; set; }
        public bool StateFundsDenied { get; set; }
        public bool OrganizedAthleticPrograms { get; set; }
        public bool AtRiskService { get; set; }
        public DateTime ServiceTime { get; set; }
        public int TaxExemptionStatus { get; set; }
        public int TaxExemptionType { get; set; }
        public string RejectionJustification { get; set; } = "";
        public string Comment { get; set; } = "";
        public bool? AppointmentCoordinated { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsListable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AgencyCode { get; set; } = "";
        public List<DTOProgram> Programs { get; set; } = new List<DTOProgram>();
        public DTOUser User { get; set; } = new DTOUser();
        public DTOUser Monitor { get; set; } = new DTOUser();
    }
}