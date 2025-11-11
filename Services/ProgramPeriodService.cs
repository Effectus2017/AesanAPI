using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.Extensions.Logging;

namespace Api.Services;

/// <summary>
/// Servicio para gestionar períodos de programas con lógica de cálculo automático de fechas
/// </summary>
public class ProgramPeriodService(
    IProgramPeriodRepository repository,
    ILogger<ProgramPeriodService> logger) : IProgramPeriodService
{
    private readonly IProgramPeriodRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ILogger<ProgramPeriodService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    // IDs de programas según la tabla Program
    private const int PROGRAM_ID_PAF = 7;  // PAF - FFDP
    private const int PROGRAM_ID_PDAM = 1; // PDAM - NSLBP
    private const int PROGRAM_ID_PSAV = 2; // PSAV - SFSP
    private const int PROGRAM_ID_PACNA = 3; // PACNA - CACFP

    /// <summary>
    /// Obtiene todos los períodos de un programa
    /// </summary>
    public async Task<IEnumerable<ProgramPeriodResponse>> GetProgramPeriodsByProgramId(int programId)
    {
        try
        {
            return await _repository.GetProgramPeriodsByProgramId(programId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener períodos del programa {ProgramId}", programId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene un período específico de un programa por año
    /// </summary>
    public async Task<ProgramPeriodResponse?> GetProgramPeriodByProgramIdAndYear(int programId, int year)
    {
        try
        {
            return await _repository.GetProgramPeriodByProgramIdAndYear(programId, year);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener período del programa {ProgramId} para el año {Year}", programId, year);
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo período de programa
    /// </summary>
    public async Task<ProgramPeriodResponse> InsertProgramPeriod(ProgramPeriodRequest request)
    {
        try
        {
            var id = await _repository.InsertProgramPeriod(request);
            var result = await _repository.GetProgramPeriodByProgramIdAndYear(request.ProgramId, request.Year);
            
            if (result == null)
            {
                throw new Exception("Error al recuperar el período insertado");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar período de programa");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un período de programa existente
    /// </summary>
    public async Task<bool> UpdateProgramPeriod(ProgramPeriodRequest request)
    {
        try
        {
            return await _repository.UpdateProgramPeriod(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar período de programa {Id}", request.Id);
            throw;
        }
    }

    /// <summary>
    /// Calcula automáticamente las fechas de inicio y fin para un programa y año
    /// </summary>
    public (DateTime StartDate, DateTime EndDate) CalculateProgramPeriodDates(int programId, int year)
    {
        try
        {
            // PAF y PDAM: Julio 1 - Junio 30 del año siguiente
            if (programId == PROGRAM_ID_PAF || programId == PROGRAM_ID_PDAM)
            {
                var startDate = new DateTime(year, 7, 1);
                var endDate = new DateTime(year + 1, 6, 30);
                return (startDate, endDate);
            }

            // PSAV: Últimas 2 semanas de Mayo - Primeras 2 semanas de Agosto
            if (programId == PROGRAM_ID_PSAV)
            {
                // Calcular último lunes de mayo
                var lastDayOfMay = new DateTime(year, 5, 31);
                var lastMondayOfMay = lastDayOfMay;
                while (lastMondayOfMay.DayOfWeek != DayOfWeek.Monday)
                {
                    lastMondayOfMay = lastMondayOfMay.AddDays(-1);
                }
                // Retroceder 1 semana (últimas 2 semanas = desde 2 semanas antes del último lunes)
                var startDate = lastMondayOfMay.AddDays(-7);

                // Calcular segundo viernes de agosto
                var firstDayOfAugust = new DateTime(year, 8, 1);
                var firstFridayOfAugust = firstDayOfAugust;
                while (firstFridayOfAugust.DayOfWeek != DayOfWeek.Friday)
                {
                    firstFridayOfAugust = firstFridayOfAugust.AddDays(1);
                }
                // Avanzar 1 semana (primeras 2 semanas = hasta 1 semana después del primer viernes)
                var endDate = firstFridayOfAugust.AddDays(7);

                return (startDate, endDate);
            }

            // PACNA: Pendiente información (configuración manual)
            // Por ahora, retornar fechas por defecto (año completo)
            if (programId == PROGRAM_ID_PACNA)
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31);
                return (startDate, endDate);
            }

            // Para otros programas, usar año completo por defecto
            _logger.LogWarning("Programa {ProgramId} no tiene regla de cálculo específica, usando año completo", programId);
            var defaultStartDate = new DateTime(year, 1, 1);
            var defaultEndDate = new DateTime(year, 12, 31);
            return (defaultStartDate, defaultEndDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular fechas del período para programa {ProgramId} y año {Year}", programId, year);
            throw;
        }
    }
}

