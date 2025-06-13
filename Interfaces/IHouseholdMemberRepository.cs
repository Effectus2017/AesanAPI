using Api.Models.Request;

namespace Api.Interfaces;

public interface IHouseholdMemberRepository
{
    Task<dynamic> GetHouseholdMemberById(int id);
    Task<dynamic> GetAllHouseholdMembers(int take, int skip, bool alls);
    Task<bool> InsertHouseholdMember(HouseholdMemberRequest request);
    Task<bool> UpdateHouseholdMember(HouseholdMemberRequest request);
    Task<bool> DeleteHouseholdMember(int id);
}
