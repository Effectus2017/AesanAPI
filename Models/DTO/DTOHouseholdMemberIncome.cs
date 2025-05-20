using System;

namespace Api.Models.DTO
{
    public class DTOHouseholdMemberIncome
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int IncomeTypeId { get; set; }
        public decimal Amount { get; set; }
        public int FrequencyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}