using Api.Interfaces;

namespace Api.Data;

public interface IUnitOfWork
{
    IAuthRepository AuthRepository { get; } // Agregar repositorio de autenticación
    // Agregar otros repositorios según sea necesario
    void Save();
}