using System.Threading.Tasks;

namespace Api.Interfaces;

public interface IPasswordService
{
    Task<string> GetTemporaryPassword(string userId);
}
