using Api.Models;
using Api.Models.Response;
using Api.Services;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper para entidades relacionadas con Agency
/// Contiene todos los métodos de mapeo para AgencyResponse y AgencyInscriptionResponse
/// </summary>
public class AgencyMapper(Lazy<MappingService> mappingService)
{
    private readonly Lazy<MappingService> _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a un AgencyResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>AgencyResponse</returns>
    public AgencyResponse MapFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new AgencyResponse
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                StatusId = item.StatusId ?? 0,
                SdrNumber = item.SdrNumber,
                UieNumber = item.UieNumber ?? 0,
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
                Status = item.StatusId != null ? new AgencyStatusResponse
                {
                    Id = item.StatusId ?? 0,
                    Name = item.StatusName ?? string.Empty,
                    NameEN = item.StatusNameEN ?? string.Empty,
                    IsActive = item.StatusIsActive ?? false,
                    DisplayOrder = item.StatusDisplayOrder ?? 0
                } : null,
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
    /// Mapea los datos de inscripción de agencia desde un resultado dinámico a un AgencyInscriptionResponse
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>AgencyInscriptionResponse</returns>
    public static AgencyInscriptionResponse? MapInscriptionFromResult(dynamic item)
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

            return new AgencyInscriptionResponse
            {
                Id = item.AgencyInscriptionId ?? 0,
                AgencyId = item.Id ?? 0,

                // Campos booleanos de inscripción
                NonProfit = item.NonProfit,
                FederalFundsDenied = item.FederalFundsDenied,
                FederalFundsDeniedReason = item.FederalFundsDeniedReason, // Nuevo mapeo
                StateFundsDenied = item.StateFundsDenied,
                StateFundsDeniedReason = item.StateFundsDeniedReason,
                NationalYouthProgram = item.NationalYouthProgram,
                IsDayCareHomeId = item.IsDayCareHomeId,

                // Campos de ID de opciones
                BasicEducationRegistry = item.BasicEducationRegistry,
                ExtendedHours = item.ExtendedHours,
                TaxExemptionStatusId = item.TaxExemptionStatusId,
                TaxExemptionTypeId = item.TaxExemptionTypeId,
                TypeOfEntityId = item.TypeOfEntityId,
                TypeOfApplicantId = item.TypeOfApplicantId,
                PublicAllianceContractId = item.PublicAllianceContractId,
                ParticipatesInHeadStartProgramId = item.ParticipatesInHeadStartProgramId,
                BoardMeetingsPerYear = item.BoardMeetingsPerYear,
                BoardMeetsRegularly = item.BoardMeetsRegularly,

                // Campos de fecha y texto
                RejectionJustification = item.RejectionJustification,
                Comments = item.Comments,
                AppointmentCoordinated = item.AppointmentCoordinated,
                AppointmentDate = item.AppointmentDate,
                DeadlineToCompleteRegistration = item.DeadlineToCompleteRegistration,
                CompletedRegistrationDate = item.CompletedRegistrationDate,
                ServicesOfferedSince = item.ServicesOfferedSince,

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
                } : null,

                IsDayCareHome = item.IsDayCareHomeId != null ? new DTOOptionSelection
                {
                    Id = item.IsDayCareHomeId,
                    Name = item.IsDayCareHomeName ?? string.Empty,
                    NameEN = item.IsDayCareHomeNameEN ?? string.Empty,
                    OptionKey = item.IsDayCareHomeOptionKey ?? string.Empty,
                    BooleanValue = item.IsDayCareHomeBooleanValue
                } : null,

                ParticipatesInHeadStartProgram = item.ParticipatesInHeadStartProgramId != null ? new DTOOptionSelection
                {
                    Id = item.ParticipatesInHeadStartProgramId,
                    Name = item.ParticipatesInHeadStartProgramName ?? string.Empty,
                    NameEN = item.ParticipatesInHeadStartProgramNameEN ?? string.Empty,
                    OptionKey = item.ParticipatesInHeadStartProgramOptionKey ?? string.Empty
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
    /// Mapea un AgencyResponse a AgencyTableResponse (misma estructura; para ir reduciendo de a poco).
    /// </summary>
    public static AgencyTableResponse MapFromAgencyResponse(AgencyResponse a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        return new AgencyTableResponse
        {
            Id = a.Id,
            StatusId = a.StatusId,
            Name = a.Name,
            SdrNumber = a.SdrNumber,
            UieNumber = a.UieNumber,
            EinNumber = a.EinNumber,
            Address = a.Address,
            ZipCode = a.ZipCode,
            PostalAddress = a.PostalAddress,
            PostalZipCode = a.PostalZipCode,
            Phone = a.Phone,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            Email = a.Email,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            ImageURL = a.ImageURL,
            AgencyCode = a.AgencyCode,
            IsRecurrent = a.IsRecurrent,
            City = a.City,
            Region = a.Region,
            PostalCity = a.PostalCity,
            PostalRegion = a.PostalRegion,
            Status = a.Status,
            User = a.User,
            AssignedUsers = a.AssignedUsers ?? [],
            Programs = a.Programs ?? [],
            Inscription = a.Inscription
        };
    }

    /// <summary>
    /// Mapea un resultado dinámico a AgencyTableResponse (solo columnas visibles en la tabla).
    /// El primer result set de 119_GetAgencies no incluye UserFirstName/Monitor*; se dejan en null.
    /// </summary>
    public AgencyTableResponse? MapTableFromResult(dynamic item)
    {
        try
        {
            if (item == null)
                return null;

            var agency = MapFromResult(item);
            return MapFromAgencyResponse(agency);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Mapea un resultado dinámico a AgencyDropdownItemResponse (Id, Name, IsActive) para dropdowns.
    /// </summary>
    public AgencyDropdownItemResponse? MapDropdownItemFromResult(dynamic item)
    {
        try
        {
            if (item == null)
                return null;

            return new AgencyDropdownItemResponse
            {
                Id = item.Id ?? 0,
                Name = item.Name?.ToString() ?? string.Empty,
                IsActive = item.IsActive ?? true
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Mapea una agencia desde un resultado dinámico a AgencyTableResponse (alias para flujo de lista tabla).
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>AgencyTableResponse o null</returns>
    public AgencyTableResponse? MapListFromResult(dynamic item)
    {
        return MapTableFromResult(item);
    }
}
