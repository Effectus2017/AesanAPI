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
        Task<dynamic> GetAllHouseholds(int take, int skip, bool alls);
        Task<bool> InsertHousehold(HouseholdRequest request);
        Task<bool> UpdateHousehold(HouseholdRequest request);
        Task<bool> DeleteHousehold(int id);

    }
}