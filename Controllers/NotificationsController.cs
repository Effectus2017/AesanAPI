using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Endpoints de notificación por correo (uso interno / Web Job).
/// Protegidos por X-Api-Key; no requieren JWT.
/// </summary>
[ApiController]
[Route("notifications")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationsController(
    ILogger<NotificationsController> logger,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IPasswordService passwordService,
    INotificationMailJobLogRepository notificationMailJobLogRepository) : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly INotificationMailJobLogRepository _notificationMailJobLogRepository = notificationMailJobLogRepository;

    /// <summary>
    /// Dispara el envío del correo de bienvenida a la agencia (WelcomeAgency).
    /// Uso interno / Web Job. Requiere X-Api-Key.
    /// </summary>
    [HttpPost("send-welcome-agency")]
    [SwaggerOperation(Summary = "Enviar correo de bienvenida a agencia", Description = "Uso interno / Web Job. Invoca IEmailService.SendWelcomeAgencyEmail para la agencia indicada.")]
    public async Task<IActionResult> SendWelcomeAgency([FromBody] SendWelcomeAgencyRequest request)
    {
        if (request == null || request.AgencyId <= 0)
        {
            return BadRequest(new { message = "AgencyId es requerido y debe ser mayor a 0." });
        }

        var agency = await _unitOfWork.AgencyRepository.GetAgencyById(request.AgencyId);
        if (agency == null)
        {
            return NotFound(new { message = $"Agencia con ID {request.AgencyId} no encontrada." });
        }

        if (agency.User == null || string.IsNullOrWhiteSpace(agency.User.UserId))
        {
            return BadRequest(new { message = "La agencia no tiene un usuario/staff asociado para enviar el correo de bienvenida." });
        }

        var temporaryPassword = await _passwordService.GetTemporaryPassword(agency.User.UserId);
        if (string.IsNullOrWhiteSpace(temporaryPassword))
        {
            return BadRequest(new { message = "No hay contraseña temporal registrada para el usuario de la agencia. El correo de bienvenida se envía tras el registro." });
        }

        var userRequest = MapToUserAgencyRequest(agency);
        await _emailService.SendWelcomeAgencyEmail(userRequest, temporaryPassword, agency.User.UserId);

        _logger.LogInformation("SendWelcomeAgency ejecutado para AgencyId {AgencyId}", request.AgencyId);
        return Ok(new { message = "Correo de bienvenida enviado." });
    }

    /// <summary>
    /// Registra un evento de log de ejecución del job de notificaciones mail.
    /// Uso interno / Web Job. Requiere X-Api-Key.
    /// </summary>
    [HttpPost("job-log")]
    [SwaggerOperation(Summary = "Registrar log de ejecución del job", Description = "Uso interno / Web Job. Persiste en BD que el job se está ejecutando (o finalizó).")]
    public async Task<IActionResult> LogJob([FromBody] NotificationMailJobLogRequest request, CancellationToken cancellationToken)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.JobName) ||
            string.IsNullOrWhiteSpace(request.Status) ||
            request.StartedAt == default)
        {
            return BadRequest(new { message = "JobName, Status y StartedAt son requeridos." });
        }

        var id = await _notificationMailJobLogRepository.InsertAsync(request, cancellationToken);
        return Ok(new { id });
    }

    private static UserAgencyRequest MapToUserAgencyRequest(AgencyResponse agency)
    {
        var agencyRequest = new AgencyRequest
        {
            Name = agency.Name ?? "",
            StatusId = agency.StatusId,
            SdrNumber = agency.SdrNumber,
            UieNumber = agency.UieNumber,
            EinNumber = agency.EinNumber,
            Address = agency.Address ?? "",
            ZipCode = agency.ZipCode ?? "",
            CityId = agency.City?.Id ?? 0,
            RegionId = agency.Region?.Id ?? 0,
            Email = agency.Email,
            Phone = agency.Phone ?? "",
            AgencyCode = agency.AgencyCode,
            ImageUrl = agency.ImageURL
        };

        var staff = agency.User;
        var staffRequest = new StaffRequest
        {
            Id = staff.Id,
            FirstName = staff.FirstName ?? "",
            MiddleName = staff.MiddleName,
            FatherLastName = staff.FatherLastName ?? "",
            MotherLastName = staff.MotherLastName ?? "",
            Email = staff.Email ?? "",
            UserId = staff.UserId,
            AgencyId = agency.Id,
            StatusId = staff.StatusId,
            PositionId = staff.PositionId,
            StaffTypeId = staff.StaffTypeId
        };

        return new UserAgencyRequest
        {
            Agency = agencyRequest,
            Staff = staffRequest
        };
    }
}
