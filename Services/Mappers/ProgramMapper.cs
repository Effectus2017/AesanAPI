using Api.Models;

namespace Api.Services.Mappers;

/// <summary>
/// Mapper estático para entidades relacionadas con Program
/// Contiene métodos de mapeo para DTOProgram
/// </summary>
public static class ProgramMapper
{
    /// <summary>
    /// Mapea los programas de una agencia desde un resultado dinámico a un DTOProgram  
    /// </summary>
    /// <param name="programs">Resultados dinámicos de los programas</param>
    /// <returns>Lista de DTOProgram</returns>
    public static List<DTOProgram> MapFromResult(IEnumerable<dynamic> programs)
    {
        try
        {
            if (programs == null)
            {
                return new List<DTOProgram>();
            }

            return programs.Select(MapSingleProgramFromResult).Where(p => p != null).ToList()!;
        }
        catch (Exception ex)
        {
            // Log the error but return empty list to avoid breaking the application
            return new List<DTOProgram>();
        }
    }

    /// <summary>
    /// Mapea un programa individual desde un resultado dinámico a un DTOProgram
    /// </summary>
    /// <param name="program">Resultado dinámico del programa</param>
    /// <returns>DTOProgram</returns>
    public static DTOProgram? MapSingleProgramFromResult(dynamic program)
    {
        try
        {
            if (program == null)
            {
                return null;
            }

            return new DTOProgram
            {
                Id = program.Id ?? 0,
                Name = program.Name ?? string.Empty,
                NameEN = program.NameEN ?? string.Empty,
                Description = program.Description ?? string.Empty,
                DescriptionEN = program.DescriptionEN ?? string.Empty,
                IsActive = program.IsActive ?? false,
                CreatedAt = program.CreatedAt,
                UpdatedAt = program.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to avoid breaking the application
            return null;
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una lista de programas (versión simplificada)
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOProgram</returns>
    public static DTOProgram MapListFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOProgram();
            }

            return new DTOProgram
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOProgram();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un programa completo
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOProgram</returns>
    public static DTOProgram MapFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOProgram();
            }

            return new DTOProgram
            {
                Id = result.Id,
                Name = result.Name,
                NameEN = result.NameEN,
                Description = result.Description,
                DescriptionEN = result.DescriptionEN,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOProgram();
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un programa de inscripción
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>DTOProgramInscription</returns>
    public static DTOProgramInscription MapInscriptionFromResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new DTOProgramInscription();
            }

            return new DTOProgramInscription
            {
                Id = result.Id,
                Agency = new DTOAgency
                {
                    Id = result.AgencyId,
                    Name = result.AgencyName,
                },
                Program = new DTOProgram
                {
                    Id = result.ProgramId,
                    Name = result.ProgramName,
                    Description = result.ProgramDescription
                },
                ApplicationNumber = result.ApplicationNumber,
                IsPublic = result.IsPublic,
                TotalNumberSchools = result.TotalNumberSchools,
                HasBasicEducationCertification = result.HasBasicEducationCertification,
                IsAeaMenuCreated = result.IsAeaMenuCreated,
                ExemptionRequirement = result.ExemptionRequirement,
                ExemptionStatus = result.ExemptionStatus,
                ParticipatingAuthority = new DTOFoodAuthority
                {
                    Id = result.ParticipatingAuthorityId,
                    Name = result.FoodAuthorityName
                },
                OperatingPolicy = new DTOOperatingPolicy
                {
                    Id = result.OperatingPolicyId,
                    Name = result.OperatingPolicyName,
                    NameEN = result.OperatingPolicyNameEN
                },
                HasCompletedCivilRightsQuestionnaire = result.HasCompletedCivilRightsQuestionnaire,
                NeedsInformationInOtherLanguages = result.NeedsInformationInOtherLanguages,
                InformationInOtherLanguages = result.InformationInOtherLanguages,
                NeedsInterpreter = result.NeedsInterpreter,
                InterpreterLanguages = result.InterpreterLanguages,
                NeedsAlternativeCommunication = result.NeedsAlternativeCommunication,
                AlternativeCommunication = result.AlternativeCommunicationId != null
                    ? new DTOAlternativeCommunication
                    {
                        Id = result.AlternativeCommunicationId,
                        Name = result.AlternativeCommunicationName
                    }
                    : null,
                NeedsFederalRelayService = new DTOOptionSelection
                {
                    Id = result.NeedsFederalRelayServiceId,
                    Name = result.NeedsFederalRelayServiceName ?? string.Empty,
                    NameEN = string.Empty,
                    OptionKey = string.Empty
                },
                ShowEvidence = new DTOOptionSelection
                {
                    Id = result.ShowEvidenceId,
                    Name = result.ShowEvidenceName ?? string.Empty,
                    NameEN = string.Empty,
                    OptionKey = string.Empty
                },
                ShowEvidenceDescription = result.ShowEvidenceDescription,
                SnackPercentage = result.SnackPercentage,
                ReducedSnackPercentage = result.ReducedSnackPercentage,
                FederalFundingCertification = result.FederalFundingCertificationId != null
                    ? new DTOFederalFundingCertification
                    {
                        Id = result.FederalFundingCertificationId,
                        FundingAmount = result.FundingAmount,
                        Description = result.FederalFundingDescription
                    }
                    : null,
                Date = result.Date,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the error but return empty object to avoid breaking the application
            return new DTOProgramInscription();
        }
    }
}
