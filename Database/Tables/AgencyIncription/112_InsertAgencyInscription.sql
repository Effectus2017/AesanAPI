-- Insertar una inscripción de agencia
-- 1.1.2
CREATE OR ALTER PROCEDURE [112_InsertAgencyInscription]
    @agencyId int,
    @nonProfit bit,
    @federalFundsDenied bit,
    @stateFundsDenied bit,
    @organizedAthleticPrograms bit,
    @atRiskService bit,
    @basicEducationRegistryId int,
    @serviceTime datetime,
    @taxExemptionStatusId int,
    @taxExemptionTypeId int,
    @publicAllianceContractId int,
    @nationalYouthProgram bit,
    @id int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY   
        INSERT INTO AgencyInscription
        (
        AgencyId, NonProfit, FederalFundsDenied, StateFundsDenied,
        OrganizedAthleticPrograms, AtRiskService,
        BasicEducationRegistryId, ServiceTime,
        TaxExemptionStatusId, TaxExemptionTypeId,
        PublicAllianceContractId, NationalYouthProgram
        )
    VALUES
        (
            @agencyId, @nonProfit, @federalFundsDenied, @stateFundsDenied,
            @organizedAthleticPrograms, @atRiskService,
            @basicEducationRegistryId, @serviceTime,
            @taxExemptionStatusId, @taxExemptionTypeId,
            @publicAllianceContractId, @nationalYouthProgram
        );
        SET @id = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 