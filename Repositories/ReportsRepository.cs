using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Response;
using Api.Services;
using Dapper;

namespace Api.Repositories;

/// <summary>
/// Repositorio para reportes del sistema
/// </summary>
public class ReportsRepository(DapperContext context, ILoggingService loggingService) : IReportsRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

    /// ------------------------------------------------------------------------------------------------
    /// Obtener
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Obtiene la estructura jerárquica completa para el árbol de jerarquía de escuelas
    /// </summary>
    /// <param name="year">Año para filtrar la estructura</param>
    /// <param name="sponsorId">ID del auspiciador (opcional). Si es null, obtiene todos los auspiciadores</param>
    /// <returns>Estructura jerárquica con Auspiciador → Año → Escuelas → Sitios</returns>
    public async Task<HierarchyStructureResponse> GetHierarchyStructure(int year, int? sponsorId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@year", year, DbType.Int32);
            parameters.Add("@sponsorId", sponsorId, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync(
                "100_GetHierarchyStructure",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Leer auspiciadores
            var sponsors = (await multi.ReadAsync<dynamic>()).ToList();

            if (!sponsors.Any())
            {
                _logger.LogInformation("No se encontraron auspiciadores para el año y filtros especificados",
                    new Dictionary<string, string>
                    {
                        { "Year", year.ToString() },
                        { "SponsorId", sponsorId?.ToString() ?? "null" }
                    });

                return new HierarchyStructureResponse
                {
                    Sponsor = new SponsorNode
                    {
                        Id = 0,
                        Name = "Sin Auspiciador",
                        Code = ""
                    },
                    Year = new YearNode { Year = year },
                    Schools = new List<SchoolNode>()
                };
            }

            // Tomar el primer auspiciador (o el especificado)
            var sponsor = sponsors.FirstOrDefault();
            if (sponsor == null)
            {
                return new HierarchyStructureResponse
                {
                    Sponsor = new SponsorNode
                    {
                        Id = 0,
                        Name = "Sin Auspiciador",
                        Code = ""
                    },
                    Year = new YearNode { Year = year },
                    Schools = new List<SchoolNode>()
                };
            }

            var sponsorNode = new SponsorNode
            {
                Id = (int)sponsor.Id,
                Name = sponsor.Name?.ToString() ?? string.Empty,
                Code = sponsor.Code?.ToString() ?? string.Empty
            };

            // Leer escuelas y sitios
            var schoolsData = (await multi.ReadAsync<dynamic>()).ToList();

            // Agrupar por escuela
            var schoolsDict = new Dictionary<int, SchoolNode>();

            foreach (var row in schoolsData)
            {
                var schoolId = (int)row.Id;

                if (!schoolsDict.ContainsKey(schoolId))
                {
                    schoolsDict[schoolId] = new SchoolNode
                    {
                        Id = schoolId,
                        Name = row.Name?.ToString() ?? string.Empty,
                        SchoolCode = row.SchoolCode?.ToString() ?? string.Empty,
                        SchoolNumber = row.SchoolNumber != null ? (int?)row.SchoolNumber : null,
                        Sites = new List<SiteNode>()
                    };
                }

                // Agregar sitio si existe
                if (row.SiteId != null)
                {
                    var siteNode = new SiteNode
                    {
                        Id = (int)row.SiteId,
                        Name = row.SiteName?.ToString() ?? string.Empty,
                        SiteNumber = row.SiteNumber != null ? (int?)row.SiteNumber : null,
                        SiteCode = row.SiteCode?.ToString() ?? string.Empty,
                        IsActive = row.IsActive != null ? (bool?)row.IsActive : null
                    };

                    // Evitar duplicados
                    if (!schoolsDict[schoolId].Sites.Any(s => s.Id == siteNode.Id))
                    {
                        schoolsDict[schoolId].Sites.Add(siteNode);
                    }
                }
            }

            var schools = schoolsDict.Values.OrderBy(s => s.Name).ToList();

            _logger.LogInformation("Estructura jerárquica obtenida exitosamente",
                new Dictionary<string, string>
                {
                    { "Year", year.ToString() },
                    { "SponsorId", sponsorId?.ToString() ?? "null" },
                    { "SchoolsCount", schools.Count.ToString() },
                    { "TotalSites", schools.Sum(s => s.Sites.Count).ToString() }
                });

            return new HierarchyStructureResponse
            {
                Sponsor = sponsorNode,
                Year = new YearNode { Year = year },
                Schools = schools
            };
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener la estructura jerárquica",
                new Dictionary<string, string>
                {
                    { "Year", year.ToString() },
                    { "SponsorId", sponsorId?.ToString() ?? "null" }
                });
            throw new Exception($"Error al obtener la estructura jerárquica: {ex.Message}", ex);
        }
    }
}

