using Api.Models;

namespace Api.Interfaces;

public interface IGroupTypeRepository
{
    Task<dynamic> GetGroupTypeById(int id);
    Task<dynamic> GetAllGroupTypes(int take, int skip, string name, bool alls);
    Task<bool> InsertGroupType(DTOGroupType groupType);
    Task<bool> UpdateGroupType(DTOGroupType groupType);
    Task<bool> UpdateGroupTypeDisplayOrder(int groupTypeId, int displayOrder);
    Task<bool> DeleteGroupType(int id);
}