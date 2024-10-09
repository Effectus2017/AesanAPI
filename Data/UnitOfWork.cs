using Api.Interfaces;

namespace Api.Data;

public class UnitOfWork(ApplicationDbContext context, IAuthRepository authRepository) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public IAuthRepository AuthRepository { get; private set; } = authRepository; // Asignar repositorio

    public void Save()
    {
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
        }
    }
}