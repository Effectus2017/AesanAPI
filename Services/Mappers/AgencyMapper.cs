using Api.Models;
using Api.Services;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con Agency
/// Contiene todos los métodos de mapeo para DTOAgency y DTOAgencyInscription
/// </summary>
public class AgencyMapper(Lazy<MappingService> mappingService)
{
    private readonly Lazy<MappingService> _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a un DTOAgency
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgency</returns>
    public DTOAgency MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOAgency
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                StatusId = item.StatusId ?? 0,
                SdrNumber = item.SdrNumber,
                UieNumber = item.UieNumber,
                EinNumber = item.EinNumber,
                Address = item.Address,
                ZipCode = item.ZipCode,
                PostalAddress = item.PostalAddress,
                PostalZipCode = item.PostalZipCode,
                Latitude = item.Latitude != null ? (float)item.Latitude : null,
                Longitude = item.Longitude != null ? (float)item.Longitude : null,
                Phone = item.Phone,
                Email = item.Email,
                ImageURL = item.ImageURL,

                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                AgencyCode = item.AgencyCode,

                City = item.CityId != null ? _mappingService.Value.MapCity(new { Id = item.CityId ?? 0, Name = item.CityName ?? string.Empty }) : null,
                Region = item.RegionId != null ? _mappingService.Value.MapRegion(new { Id = item.RegionId ?? 0, Name = item.RegionName ?? string.Empty }) : null,
                PostalCity = item.PostalCityId != null ? _mappingService.Value.MapCity(new { Id = item.PostalCityId ?? 0, Name = item.PostalCityName ?? string.Empty }) : null,
                PostalRegion = item.PostalRegionId != null ? _mappingService.Value.MapRegion(new { Id = item.PostalRegionId ?? 0, Name = item.PostalRegionName ?? string.Empty }) : null,
                Status = _mappingService.Value.MapAgencyStatus(item.StatusId, item.AgencyStatusName),
                User = item.UserId != null ? new DTOStaff
                {
                    Id = item.UserId ?? 0,
                    FirstName = item.UserFirstName ?? string.Empty,
                    MiddleName = item.UserMiddleName ?? string.Empty,
                    FatherLastName = item.UserFatherLastName ?? string.Empty,
                    MotherLastName = item.UserMotherLastName ?? string.Empty,
                    PositionId = item.UserPositionId ?? 0,
                    PositionName = item.UserPositionName ?? string.Empty,
                    Email = item.UserEmail ?? string.Empty,
                    UserId = item.UserGuid,
                    ContractStartDate = item.UserContractStartDate,
                    ContractEndDate = item.UserContractEndDate,
                    Position = item.UserPositionId != null ? new DTOOptionSelection
                    {
                        Id = item.UserPositionId,
                        Name = item.UserPositionName ?? string.Empty,
                        NameEN = item.UserPositionNameEN ?? string.Empty,
                        OptionKey = item.UserPositionOptionKey ?? string.Empty
                    } : null,
                } : null,

                // Datos de inscripción de la agencia
                Inscription = MapInscriptionFromResult(item)
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error al mapear la agencia: Propiedad no encontrada o inválida. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error inesperado al mapear la agencia: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mapea los datos de inscripción de agencia desde un resultado dinámico a un DTOAgencyInscription
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOAgencyInscription</returns>
    public static DTOAgencyInscription? MapInscriptionFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            // Verificar si hay datos de inscripción disponibles
            // Si no hay AgencyId en la inscripción, retornar null
            if (item.AgencyInscriptionId == null)
            {
                return null;
            }

            return new DTOAgencyInscription
            {
                Id = item.AgencyInscriptionId ?? 0,
                AgencyId = item.Id ?? 0,

                // Campos booleanos de inscripción
                NonProfit = item.NonProfit,
                FederalFundsDenied = item.FederalFundsDenied,
                StateFundsDenied = item.StateFundsDenied,
                OrganizedAthleticPrograms = item.OrganizedAthleticPrograms,
                AtRiskService = item.AtRiskService,
                NationalYouthProgram = item.NationalYouthProgram,
                IsDayCareHome = item.IsDayCareHome,

                // Campos de ID de opciones
                BasicEducationRegistry = item.BasicEducationRegistry,
                ExtendedHours = item.ExtendedHours,
                TaxExemptionStatusId = item.TaxExemptionStatusId,
                TaxExemptionTypeId = item.TaxExemptionTypeId,
                TypeOfEntityId = item.TypeOfEntityId,
                TypeOfApplicantId = item.TypeOfApplicantId,
                PublicAllianceContractId = item.PublicAllianceContractId,

                // Campos de fecha y texto
                ServiceTime = item.ServiceTime,
                RejectionJustification = item.RejectionJustification,
                Comments = item.Comments,
                AppointmentCoordinated = item.AppointmentCoordinated,
                AppointmentDate = item.AppointmentDate,
                DeadlineToCompleteRegistration = item.DeadlineToCompleteRegistration,

                // Relaciones con OptionSelection (si están disponibles en el resultado)
                TaxExemptionStatus = item.TaxExemptionStatusId != null ? new DTOOptionSelection
                {
                    Id = item.TaxExemptionStatusId,
                    Name = item.TaxExemptionStatusName ?? string.Empty,
                    NameEN = item.TaxExemptionStatusNameEN ?? string.Empty,
                    OptionKey = item.TaxExemptionStatusOptionKey ?? string.Empty
                } : null,

                TaxExemptionType = item.TaxExemptionTypeId != null ? new DTOOptionSelection
                {
                    Id = item.TaxExemptionTypeId,
                    Name = item.TaxExemptionTypeName ?? string.Empty,
                    NameEN = item.TaxExemptionTypeNameEN ?? string.Empty,
                    OptionKey = item.TaxExemptionTypeOptionKey ?? string.Empty
                } : null,

                TypeOfEntity = item.TypeOfEntityId != null ? new DTOOptionSelection
                {
                    Id = item.TypeOfEntityId,
                    Name = item.TypeOfEntityName ?? string.Empty,
                    NameEN = item.TypeOfEntityNameEN ?? string.Empty,
                    OptionKey = item.TypeOfEntityOptionKey ?? string.Empty
                } : null,

                TypeOfApplicant = item.TypeOfApplicantId != null ? new DTOOptionSelection
                {
                    Id = item.TypeOfApplicantId,
                    Name = item.TypeOfApplicantName ?? string.Empty,
                    NameEN = item.TypeOfApplicantNameEN ?? string.Empty,
                    OptionKey = item.TypeOfApplicantOptionKey ?? string.Empty
                } : null,

                PublicAllianceContract = item.PublicAllianceContractId != null ? new DTOOptionSelection
                {
                    Id = item.PublicAllianceContractId,
                    Name = item.PublicAllianceContractName ?? string.Empty,
                    NameEN = item.PublicAllianceContractNameEN ?? string.Empty,
                    OptionKey = item.PublicAllianceContractOptionKey ?? string.Empty
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            // Si hay error al mapear, retornar null en lugar de fallar
            // Esto permite que la agencia se mapee sin datos de inscripción
            return null;
        }
        catch (Exception ex)
        {
            // Si hay error al mapear, retornar null en lugar de fallar
            return null;
        }
    }

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a un objeto con campos necesarios para listas y tablas de UI
    /// Incluye datos básicos de agencia, usuario, monitor, estado y campos de inscripción
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>Objeto con campos necesarios para la UI</returns>
    public dynamic MapListFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            return new
            {
                // Campos básicos de la agencia
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                SdrNumber = item.SdrNumber ?? 0,
                UieNumber = item.UieNumber ?? 0,
                EinNumber = item.EinNumber ?? 0,
                Address = item.Address ?? string.Empty,
                Phone = item.Phone ?? string.Empty,
                Email = item.Email ?? string.Empty,
                IsActive = item.IsActive ?? false,
                IsListable = item.IsListable ?? false,
                item.CreatedAt,
                item.UpdatedAt,
                AgencyCode = item.AgencyCode ?? string.Empty,

                // Datos de ubicación
                City = item.CityId != null ? _mappingService.Value.MapCity(new { Id = item.CityId ?? 0, Name = item.CityName ?? string.Empty }) : null,
                Region = item.RegionId != null ? _mappingService.Value.MapRegion(new { Id = item.RegionId ?? 0, Name = item.RegionName ?? string.Empty }) : null,

                // Estado de la agencia
                Status = item.StatusId != null ? new
                {
                    Id = item.StatusId ?? 0,
                    Name = item.AgencyStatusName ?? string.Empty
                } : null,

                // Usuario creador (owner)
                User = item.UserId != null ? new
                {
                    Id = item.UserId ?? 0,
                    FirstName = item.UserFirstName ?? string.Empty,
                    MiddleName = item.UserMiddleName ?? string.Empty,
                    FatherLastName = item.UserFatherLastName ?? string.Empty,
                    MotherLastName = item.UserMotherLastName ?? string.Empty,
                    Email = item.UserEmail ?? string.Empty,
                    PositionId = item.UserPositionId ?? 0,
                    PositionName = item.UserPositionName ?? string.Empty,
                    ContractStartDate = item.UserContractStartDate,
                    ContractEndDate = item.UserContractEndDate
                } : null,

                // Usuario monitor
                Monitor = item.MonitorId != null ? new
                {
                    Id = item.MonitorId ?? 0,
                    FirstName = item.MonitorFirstName ?? string.Empty,
                    MiddleName = item.MonitorMiddleName ?? string.Empty,
                    FatherLastName = item.MonitorFatherLastName ?? string.Empty,
                    MotherLastName = item.MonitorMotherLastName ?? string.Empty,
                    Email = item.MonitorEmail ?? string.Empty,
                    PositionId = item.MonitorPositionId ?? 0,
                    PositionName = item.MonitorPositionName ?? string.Empty,
                    ContractStartDate = item.MonitorContractStartDate,
                    ContractEndDate = item.MonitorContractEndDate
                } : null,

                // Campos de inscripción básicos (los que se muestran en UI)
                AppointmentCoordinated = item.ProgramAppointmentCoordinated ?? false,
                AppointmentDate = item.ProgramAppointmentDate,
                RejectionJustification = item.ProgramRejectionJustification ?? string.Empty,
                item.DeadlineToCompleteRegistration,

                // Campos de inscripción adicionales (para compatibilidad)
                item.NonProfit,
                item.FederalFundsDenied,
                item.StateFundsDenied,
                item.OrganizedAthleticPrograms,
                item.AtRiskService,
                item.NationalYouthProgram,
                item.IsDayCareHome,
                item.BasicEducationRegistry,
                item.ExtendedHours,
                item.TaxExemptionStatusId,
                item.TaxExemptionTypeId,
                item.TypeOfEntityId,
                item.TypeOfApplicantId,
                item.PublicAllianceContractId
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }
}
