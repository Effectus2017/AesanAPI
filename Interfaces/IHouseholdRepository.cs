using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models.DTO;
using Api.Models.Request;

namespace Api.Interfaces
{
    public interface IHouseholdRepository
    {
        // Household
        Task<dynamic> GetHouseholdById(int id);
        Task<dynamic> GetHouseholds(int take, int skip);
        Task<int> InsertHousehold(HouseholdRequest request);
        Task<bool> UpdateHousehold(HouseholdRequest request);
        Task<bool> DeleteHousehold(int id);

        // HouseholdMember
        Task<dynamic> GetHouseholdMemberById(int id);
        Task<dynamic> GetHouseholdMembers(int take, int skip, int memberId);
        Task<int> InsertHouseholdMember(HouseholdMemberRequest request);
        Task<bool> UpdateHouseholdMember(HouseholdMemberRequest request);
        Task<bool> DeleteHouseholdMember(int id);
    }
}