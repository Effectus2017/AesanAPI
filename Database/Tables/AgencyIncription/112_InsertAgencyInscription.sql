-- Insertar una inscripción de agencia
-- 1.1.2
CREATE OR ALTER PROCEDURE [112_InsertAgencyInscription]
    @AgencyId int,
    @NonProfit bit,
    @FederalFundsDenied bit,
    @StateFundsDenied bit,
    @OrganizedAthleticPrograms bit,
    @AtRiskService bit,
    @BasicEducationRegistryId int,
    @ServiceTime datetime,
    @TaxExemptionStatusId int,
    @TaxExemptionTypeId int,
    @PublicAllianceContractId int,
    @NationalYouthProgram bit,
    @Id int OUTPUT
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
            @AgencyId, @NonProfit, @FederalFundsDenied, @StateFundsDenied,
            @OrganizedAthleticPrograms, @AtRiskService,
            @BasicEducationRegistryId, @ServiceTime,
            @TaxExemptionStatusId, @TaxExemptionTypeId,
            @PublicAllianceContractId, @NationalYouthProgram
        );
        SET @Id = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 