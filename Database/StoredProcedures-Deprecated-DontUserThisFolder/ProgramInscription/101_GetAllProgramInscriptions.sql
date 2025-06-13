CREATE OR ALTER PROCEDURE [dbo].[101_GetAllProgramInscriptions]
    @take INT,
    @skip INT,
    @agencyId INT = NULL,
    @programId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener las inscripciones con información relacionada sin utilizar CTE
    SELECT
        pi.*,
        a.Name AS AgencyName,
        p.Name AS ProgramName,
        '' AS ProgramDescription,
        fa.Name AS FoodAuthorityName,
        '' AS OperatingPolicyDescription,
        ac.Name AS AlternativeCommunicationName,
        os1.Name AS NeedsFederalRelayServiceName,
        os2.Name AS ShowEvidenceName,
        ffc.FundingAmount,
        ffc.Description AS FederalFundingDescription
    FROM ProgramInscription pi
        LEFT JOIN Agency a ON pi.AgencyId = a.Id
        LEFT JOIN Program p ON pi.ProgramId = p.Id
        LEFT JOIN FoodAuthority fa ON pi.ParticipatingAuthorityId = fa.Id
        LEFT JOIN OperatingPolicy op ON pi.OperatingPolicyId = op.Id
        LEFT JOIN AlternativeCommunication ac ON pi.AlternativeCommunicationId = ac.Id
        LEFT JOIN OptionSelection os1 ON pi.NeedsFederalRelayServiceId = os1.Id
        LEFT JOIN OptionSelection os2 ON pi.ShowEvidenceId = os2.Id
        LEFT JOIN FederalFundingCertification ffc ON pi.FederalFundingCertificationId = ffc.Id
        LEFT JOIN AgencyUsers uaa ON a.Id = uaa.AgencyId AND (@userId IS NULL OR uaa.UserId = @userId)
    WHERE (@agencyId IS NULL OR pi.AgencyId = @agencyId)
        AND (@programId IS NULL OR pi.ProgramId = @programId)
        AND (@alls = 1 OR (@userId IS NULL OR uaa.IsActive = 1))
    ORDER BY pi.CreatedAt DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Obtener el conteo total
    SELECT COUNT(*)
    FROM ProgramInscription pi
    WHERE (@agencyId IS NULL OR pi.AgencyId = @agencyId)
        AND (@programId IS NULL OR pi.ProgramId = @programId);
END;

