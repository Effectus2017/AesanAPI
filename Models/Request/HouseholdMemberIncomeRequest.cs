using System;

namespace Api.Models.Request
{
    public class HouseholdMemberIncomeRequest
    {
        public int? Id { get; set; }
        public int MemberId { get; set; }
        public string IncomeType { get; set; }
        public decimal Amount { get; set; }
        public string Frequency { get; set; }
    }
}