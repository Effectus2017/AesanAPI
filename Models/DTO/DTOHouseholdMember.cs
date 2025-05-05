using System;

namespace Api.Models.DTO
{
    public class DTOHouseholdMember
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Relationship { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}