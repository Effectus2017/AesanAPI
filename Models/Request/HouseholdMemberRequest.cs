using System;

namespace Api.Models.Request
{
    public class HouseholdMemberRequest
    {
        public int? Id { get; set; }
        public int HouseholdId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Relationship { get; set; }
        public string Gender { get; set; }
    }
}