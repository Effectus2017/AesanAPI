using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Api.Repositories;

public class ProgramRepository(DapperContext context, ILogger<ProgramRepository> logger, ISchoolRepository schoolRepository, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IProgramRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ProgramRepository> _logger = logger;
    private readonly ISchoolRepository _schoolRepository = schoolRepository ?? throw new ArgumentNullException(nameof(schoolRepository));
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

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

            var result = await dbConnection.QueryFirstOrDefaultAsync<DTOProgram>(
                "100_GetProgramById",
                param,
                commandType: CommandType.StoredProcedure
            );

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
    public async Task<dynamic> GetAllProgramsFromDb(int take, int skip, string names, bool alls)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@names", names, DbType.String);
            param.Add("@alls", alls, DbType.Boolean);
            var result = await dbConnection.QueryMultipleAsync(
                "101_GetPrograms",
                param,
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
            {
                return null;
            }

            var data = result
                .Read<dynamic>()
                .Select(
                    item =>
                        new DTOProgram
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description
                        }
                )
                .ToList();

            var count = result.Read<int>().Single();
            return new { data, count };
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
    public async Task<dynamic> GetAllProgramInscriptions(
        int take,
        int skip,
        int? agencyId = null,
        int? programId = null
    )
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@programId", programId, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync(
                "101_GetAllProgramInscriptions",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var inscriptions = result
                .Read<dynamic>()
                .Select(
                    item =>
                        new DTOProgramInscription
                        {
                            Id = item.Id,
                            Agency = new DTOAgency
                            {
                                Id = item.AgencyId,
                                Name = item.AgencyName,
                            },
                            Program = new DTOProgram
                            {
                                Id = item.ProgramId,
                                Name = item.ProgramName,
                                Description = item.ProgramDescription
                            },
                            ApplicationNumber = item.ApplicationNumber,
                            IsPublic = item.IsPublic,
                            TotalNumberSchools = item.TotalNumberSchools,
                            HasBasicEducationCertification = item.HasBasicEducationCertification,
                            IsAeaMenuCreated = item.IsAeaMenuCreated,
                            ExemptionRequirement = item.ExemptionRequirement,
                            ExemptionStatus = item.ExemptionStatus,
                            ParticipatingAuthority = new DTOFoodAuthority
                            {
                                Id = item.ParticipatingAuthorityId,
                                Name = item.FoodAuthorityName
                            },
                            OperatingPolicy = new DTOOperatingPolicy
                            {
                                Id = item.OperatingPolicyId,
                                Name = item.OperatingPolicyName,
                                NameEN = item.OperatingPolicyNameEN
                            },
                            HasCompletedCivilRightsQuestionnaire = item.HasCompletedCivilRightsQuestionnaire,
                            NeedsInformationInOtherLanguages = item.NeedsInformationInOtherLanguages,
                            InformationInOtherLanguages = item.InformationInOtherLanguages,
                            NeedsInterpreter = item.NeedsInterpreter,
                            InterpreterLanguages = item.InterpreterLanguages,
                            NeedsAlternativeCommunication = item.NeedsAlternativeCommunication,
                            AlternativeCommunication = item.AlternativeCommunicationId != null
                                ? new DTOAlternativeCommunication
                                {
                                    Id = item.AlternativeCommunicationId,
                                    Name = item.AlternativeCommunicationName
                                }
                                : null,
                            NeedsFederalRelayService = new DTOOptionSelection
                            {
                                Id = item.NeedsFederalRelayServiceId,
                                Name = item.NeedsFederalRelayServiceName
                            },
                            ShowEvidence = new DTOOptionSelection
                            {
                                Id = item.ShowEvidenceId,
                                Name = item.ShowEvidenceName
                            },
                            ShowEvidenceDescription = item.ShowEvidenceDescription,
                            SnackPercentage = item.SnackPercentage,
                            ReducedSnackPercentage = item.ReducedSnackPercentage,
                            FederalFundingCertification = item.FederalFundingCertificationId != null
                                ? new DTOFederalFundingCertification
                                {
                                    Id = item.FederalFundingCertificationId,
                                    FundingAmount = item.FundingAmount,
                                    Description = item.FederalFundingDescription
                                }
                                : null,
                            Date = item.Date,
                            CreatedAt = item.CreatedAt,
                            UpdatedAt = item.UpdatedAt
                        }
                )
                .ToList();

            var count = result.Read<int>().Single();
            return new { data = inscriptions, count };
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
            dataTable.Columns.Add("totalNumberSchools", typeof(int));
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
                request.TotalNumberSchools,
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

            // Insertar escuelas usando el repositorio de escuelas
            if (request.Schools != null && request.Schools.Any())
            {
                foreach (var schoolRequest in request.Schools)
                {
                    var schoolId = await _schoolRepository.InsertSchool(schoolRequest);

                    // Vincular la escuela con la inscripción
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO ProgramInscriptionSchool (ProgramInscriptionId, SchoolId) VALUES (@InscriptionId, @SchoolId)",
                        new { InscriptionId = inscriptionId, SchoolId = schoolId }
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

    public async Task<List<DTOOptionSelection>> GetAllOptionSelections()
    {
        return await _cache.CacheQuery(
            _appSettings.Cache.Keys.OptionSelections,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var result = await dbConnection.QueryAsync<DTOOptionSelection>(
                    "SELECT * FROM OptionSelection ORDER BY Name"
                );
                return result.ToList();
            },
            _logger,
            _appSettings
        );
    }

    // Método para invalidar el caché manualmente si es necesario
    private void InvalidateCache(int? programId = null)
    {
        if (programId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Program, programId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Programs);
        _cache.Remove(_appSettings.Cache.Keys.ProgramInscriptions);
        _cache.Remove(_appSettings.Cache.Keys.OptionSelections);

        _logger.LogInformation("Cache invalidado para Program Repository");
    }
}
