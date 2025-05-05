using System;

namespace Api.Models.Request
{
    public class HouseholdRequest
    {
        public int? Id { get; set; }
        public string Street { get; set; }
        public string Apartment { get; set; }
        public int CityId { get; set; }
        public int RegionId { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompletedBy { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}