using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Api.Repositories;

public class ProgramRepository(DapperContext context, ILogger<ProgramRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IProgramRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ProgramRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene un programa por su ID
    /// </summary>
    /// <param name="id">El ID del programa</param>
    /// <returns>El programa</returns>
    public async Task<dynamic> GetProgramById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<DTOProgram>("100_GetProgramById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el programa con ID {ProgramId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los programas de la base de datos
    /// </summary>
    /// <param name="take">El número de programas a obtener</param>
    /// <param name="skip">El número de programas a saltar</param>
    /// <param name="names">Los nombres de los programas a buscar (separados por coma)</param>
    /// <param name="alls">Si se deben obtener todos los programas</param>
    /// <returns>Los programas</returns>
    public async Task<dynamic> GetAllProgramsFromDb(int take, int skip, string names, bool alls, bool isList)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@names", names, DbType.String);
            param.Add("@alls", alls, DbType.Boolean);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.Programs, take, skip, names, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await dbConnection.QueryMultipleAsync("101_GetPrograms", param, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapProgramList).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("101_GetPrograms", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapProgramFromResult).ToList();
                var count = result.Read<int>().Single();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los programas");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las inscripciones de programas
    /// </summary>
    /// <param name="take">El número de inscripciones a obtener</param>
    /// <param name="skip">El número de inscripciones a saltar</param>
    /// <param name="agencyId">El ID de la agencia</param>
    /// <param name="programId">El ID del programa</param>
    /// <returns>Las inscripciones de programas</returns>
    public async Task<dynamic> GetAllProgramInscriptions(int take, int skip, int? agencyId = null, int? programId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@programId", programId, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("101_GetAllProgramInscriptions", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var data = result.Read<dynamic>().Select(_mappingService.MapProgramInscription).ToList();
            var count = result.Read<int>().Single();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las inscripciones de programas");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo programa en la base de datos
    /// </summary>
    /// <param name="programRequest">Datos del programa a insertar</param>
    /// <returns>El ID del programa insertado</returns>
    public async Task<bool> InsertProgram(ProgramRequest programRequest)
    {
        try
        {
            _logger.LogInformation("Insertando nuevo programa");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", programRequest.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@description", programRequest.Description, DbType.String, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertProgram", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            InvalidateCache(id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el programa");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta una nueva inscripción de programa
    /// </summary>
    /// <param name="request">Datos de la inscripción</param>
    /// <returns>El ID de la inscripción insertada</returns>
    public async Task<bool> InsertProgramInscription(ProgramInscriptionRequest request)
    {
        try
        {
            _logger.LogInformation("Insertando nueva inscripción de programa");

            using IDbConnection dbConnection = _context.CreateConnection();

            // Crear tabla temporal con los datos
            var dataTable = new DataTable();
            dataTable.Columns.Add("agencyId", typeof(int));
            dataTable.Columns.Add("programId", typeof(int));
            dataTable.Columns.Add("applicationNumber", typeof(string));
            dataTable.Columns.Add("isPublic", typeof(bool));
            dataTable.Columns.Add("totalNumberSites", typeof(int));
            dataTable.Columns.Add("hasBasicEducationCertification", typeof(bool));
            dataTable.Columns.Add("isAeaMenuCreated", typeof(bool));
            dataTable.Columns.Add("exemptionRequirement", typeof(string));
            dataTable.Columns.Add("exemptionStatus", typeof(string));
            dataTable.Columns.Add("participatingAuthorityId", typeof(int));
            dataTable.Columns.Add("operatingPolicyId", typeof(int));
            dataTable.Columns.Add("hasCompletedCivilRightsQuestionnaire", typeof(bool));
            dataTable.Columns.Add("needsInformationInOtherLanguages", typeof(bool));
            dataTable.Columns.Add("informationInOtherLanguages", typeof(string));
            dataTable.Columns.Add("needsInterpreter", typeof(bool));
            dataTable.Columns.Add("interpreterLanguages", typeof(string));
            dataTable.Columns.Add("needsAlternativeCommunication", typeof(bool));
            dataTable.Columns.Add("alternativeCommunicationId", typeof(int));
            dataTable.Columns.Add("needsFederalRelayServiceId", typeof(int));
            dataTable.Columns.Add("showEvidenceId", typeof(int));
            dataTable.Columns.Add("showEvidenceDescription", typeof(string));
            dataTable.Columns.Add("snackPercentage", typeof(decimal));
            dataTable.Columns.Add("reducedSnackPercentage", typeof(decimal));
            dataTable.Columns.Add("federalFundingCertificationId", typeof(int));

            dataTable.Rows.Add(
                request.AgencyId,
                request.ProgramId,
                request.ApplicationNumber,
                request.IsPublic,
                request.TotalNumberSites,
                request.HasBasicEducationCertification,
                request.IsAeaMenuCreated,
                request.ExemptionRequirement,
                request.ExemptionStatus,
                request.ParticipatingAuthorityId,
                request.OperatingPolicyId,
                request.HasCompletedCivilRightsQuestionnaire,
                request.NeedsInformationInOtherLanguages,
                request.InformationInOtherLanguages,
                request.NeedsInterpreter,
                request.InterpreterLanguages,
                request.NeedsAlternativeCommunication,
                request.AlternativeCommunicationId,
                request.NeedsFederalRelayServiceId,
                request.ShowEvidenceId,
                request.ShowEvidenceDescription,
                request.SnackPercentage,
                request.ReducedSnackPercentage,
                request.FederalFundingCertificationId
            );

            var parameters = new DynamicParameters();
            parameters.Add("@inscription", dataTable.AsTableValuedParameter("ProgramInscriptionType"));
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertProgramInscription", parameters, commandType: CommandType.StoredProcedure);

            int inscriptionId = parameters.Get<int>("@id");

            // Insertar sitios directamente usando stored procedures
            if (request.Sites != null && request.Sites.Any())
            {
                foreach (var siteRequest in request.Sites)
                {
                    var siteId = await InsertSiteDirectly(dbConnection, siteRequest);

                    // Vincular el sitio con la inscripción
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO ProgramInscriptionSite (ProgramInscriptionId, SiteId) VALUES (@InscriptionId, @SiteId)",
                        new { InscriptionId = inscriptionId, SiteId = siteId }
                    );
                }
            }

            // Insertar fuentes de fondos federales
            if (request.FederalFundingSources != null && request.FederalFundingSources.Any())
            {
                foreach (var source in request.FederalFundingSources)
                {
                    var sourceParams = new DynamicParameters();
                    sourceParams.Add("@inscriptionId", inscriptionId);
                    sourceParams.Add("@name", source.Name);
                    sourceParams.Add("@dateFrom", source.DateFrom);
                    sourceParams.Add("@dateTo", source.DateTo);
                    sourceParams.Add("@amount", source.Amount);

                    await dbConnection.ExecuteAsync("100_InsertFederalFundingSource", sourceParams, commandType: CommandType.StoredProcedure);
                }
            }

            // Insertar documentos requeridos
            if (request.RequiredDocumentIds != null && request.RequiredDocumentIds.Any())
            {
                foreach (var documentId in request.RequiredDocumentIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO ProgramInscriptionRequiredDocuments (ProgramInscriptionId, DocumentsRequiredId) VALUES (@InscriptionId, @DocumentId)",
                        new { InscriptionId = inscriptionId, DocumentId = documentId }
                    );
                }
            }

            // Invalidar caché relacionado
            InvalidateCache(request.ProgramId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la inscripción del programa");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta un sitio directamente usando stored procedure para evitar dependencias circulares
    /// </summary>
    /// <param name="dbConnection">Conexión a la base de datos</param>
    /// <param name="siteRequest">Datos del sitio a insertar</param>
    /// <returns>ID del sitio insertado</returns>
    private async Task<int> InsertSiteDirectly(IDbConnection dbConnection, SiteRequest siteRequest)
    {
        try
        {
            var parameters = new DynamicParameters();

            parameters.Add("@agencyId", siteRequest.AgencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@name", siteRequest.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@startDate", siteRequest.StartDate, DbType.Date, ParameterDirection.Input);
            parameters.Add("@address", siteRequest.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", siteRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", siteRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@zipCode", siteRequest.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@latitude", siteRequest.Latitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@longitude", siteRequest.Longitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@postalAddress", siteRequest.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@postalCityId", siteRequest.PostalCityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@postalRegionId", siteRequest.PostalRegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@postalZipCode", siteRequest.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@sameAsPhysicalAddress", siteRequest.SameAsPhysicalAddress, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@organizationTypeId", siteRequest.OrganizationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@centerTypeId", siteRequest.CenterTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@nonProfit", siteRequest.NonProfit, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@baseYear", siteRequest.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@renewalYear", siteRequest.RenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingFromDate", siteRequest.OperatingFromDate, DbType.Date, ParameterDirection.Input);
            parameters.Add("@operatingToDate", siteRequest.OperatingToDate, DbType.Date, ParameterDirection.Input);
            parameters.Add("@operatingDaysCalculated", siteRequest.OperatingDaysCalculated, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@kitchenTypeId", siteRequest.KitchenTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@groupTypeId", siteRequest.GroupTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@deliveryTypeId", siteRequest.DeliveryTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@sponsorTypeId", siteRequest.SponsorTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@applicantTypeId", siteRequest.ApplicantTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@residentialTypeId", siteRequest.ResidentialTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingPolicyId", siteRequest.OperatingPolicyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@areaTypeId", siteRequest.AreaTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@locationTypeId", siteRequest.LocationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@hasWarehouse", siteRequest.HasWarehouse, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@hasDiningRoom", siteRequest.HasDiningRoom, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@sitePhone", siteRequest.SitePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@extension", siteRequest.Extension, DbType.String, ParameterDirection.Input);
            parameters.Add("@mobilePhone", siteRequest.MobilePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@communityId", siteRequest.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", siteRequest.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", siteRequest.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteLocationId", siteRequest.SiteLocationId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", siteRequest.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", siteRequest.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", siteRequest.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", siteRequest.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@siteCode", siteRequest.SiteCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@generalEnrollment", siteRequest.GeneralEnrollment, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@serviceTime", siteRequest.ServiceTime, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@organizedAthleticPrograms", siteRequest.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@atRiskService", siteRequest.AtRiskService, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", siteRequest.InactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@inactiveDate", siteRequest.InactiveDate, DbType.DateTime, ParameterDirection.Input);

            // Obtener el siguiente número de sitio para la agencia
            int nextSiteNumber = await GetNextSiteNumber(dbConnection, siteRequest.AgencyId.Value);
            parameters.Add("@siteNumber", nextSiteNumber, DbType.Int32, ParameterDirection.Input);

            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("104_InsertSite", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar sitio directamente: {Message}", ex.Message);
            throw new Exception($"Error al insertar sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene el siguiente número de sitio para una agencia
    /// </summary>
    /// <param name="dbConnection">Conexión a la base de datos</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>El siguiente número de sitio disponible</returns>
    private async Task<int> GetNextSiteNumber(IDbConnection dbConnection, int agencyId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AgencyId", agencyId, DbType.Int32, ParameterDirection.Input);

            var result = await dbConnection.QuerySingleAsync<int>("100_GetNextSiteNumber", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el siguiente número de sitio para la agencia {AgencyId}", agencyId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Invalida el caché para el programa
    /// </summary>
    /// <param name="programId">ID del programa</param>
    private void InvalidateCache(int? programId = null)
    {
        if (programId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Programs, 0, 0, "", false));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Programs);
        _cache.Remove(_appSettings.Cache.Keys.ProgramInscriptions);

        _logger.LogInformation("Cache invalidado para Program Repository");
    }

}
