using System.Threading.Tasks;
using Api.Models;

namespace Api.Interfaces;

public interface IOptionSelectionRepository
{
    Task<dynamic> GetOptionSelectionById(int id);
    Task<dynamic> GetOptionSelectionByOptionKey(string optionKey);
    Task<dynamic> GetAllOptionSelections(int take, int skip, string name, string optionType, bool alls);
    Task<bool> InsertOptionSelection(DTOOptionSelection optionSelection);
    Task<bool> UpdateOptionSelection(DTOOptionSelection optionSelection);
    Task<bool> UpdateOptionSelectionDisplayOrder(int optionSelectionId, int displayOrder);
    Task<bool> DeleteOptionSelection(int id);
}
