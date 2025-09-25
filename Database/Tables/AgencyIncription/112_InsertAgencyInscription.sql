-- Insertar una inscripción de agencia
-- 1.1.4 - Actualizado para incluir ExtendedHours
CREATE OR ALTER PROCEDURE [112_InsertAgencyInscription]
    @agencyId int,
    @nonProfit bit,
    @federalFundsDenied bit,
    @stateFundsDenied bit,
    @organizedAthleticPrograms bit,
    @atRiskService bit,
    @basicEducationRegistry bit,
    @extendedHours bit,
    @serviceTime datetime,
    @taxExemptionStatusId int,
    @taxExemptionTypeId int,
    @publicAllianceContractId int,
    @nationalYouthProgram bit,
    @isDayCareHome bit,
    @deadlineToCompleteRegistration datetime,
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
        BasicEducationRegistry, ExtendedHours, ServiceTime,
        TaxExemptionStatusId, TaxExemptionTypeId,
        PublicAllianceContractId, NationalYouthProgram, IsDayCareHome, DeadlineToCompleteRegistration
        )
    VALUES
        (
            @agencyId, @nonProfit, @federalFundsDenied, @stateFundsDenied,
            @organizedAthleticPrograms, @atRiskService,
            @basicEducationRegistry, @extendedHours, @serviceTime,
            @taxExemptionStatusId, @taxExemptionTypeId,
            @publicAllianceContractId, @nationalYouthProgram, @isDayCareHome, @deadlineToCompleteRegistration
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