using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Endpoints de notificación por correo (uso interno / Web Job).
/// Protegidos por X-Api-Key; no requieren JWT.
/// </summary>
[Route("notifications")]
[ApiController]
public class NotificationsController(
    ILogger<NotificationsController> logger,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IPasswordService passwordService) : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordService _passwordService = passwordService;

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

        var dtoAgency = (DTOAgency)agency;
        if (dtoAgency.User == null || string.IsNullOrWhiteSpace(dtoAgency.User.UserId))
        {
            return BadRequest(new { message = "La agencia no tiene un usuario/staff asociado para enviar el correo de bienvenida." });
        }

        var temporaryPassword = await _passwordService.GetTemporaryPassword(dtoAgency.User.UserId);
        if (string.IsNullOrWhiteSpace(temporaryPassword))
        {
            return BadRequest(new { message = "No hay contraseña temporal registrada para el usuario de la agencia. El correo de bienvenida se envía tras el registro." });
        }

        var userRequest = MapToUserAgencyRequest(dtoAgency);
        await _emailService.SendWelcomeAgencyEmail(userRequest, temporaryPassword, dtoAgency.User.UserId);

        _logger.LogInformation("SendWelcomeAgency ejecutado para AgencyId {AgencyId}", request.AgencyId);
        return Ok(new { message = "Correo de bienvenida enviado." });
    }

    private static UserAgencyRequest MapToUserAgencyRequest(DTOAgency agency)
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
