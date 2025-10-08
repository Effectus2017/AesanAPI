-- Insertar una inscripción de agencia
-- 1.1.5 - Actualizado para incluir StateFundsDeniedReason
CREATE OR ALTER PROCEDURE [112_InsertAgencyInscription]
    @agencyId int,
    @nonProfit bit,
    @federalFundsDenied bit,
    @federalFundsDeniedReason nvarchar(max),
    -- Nuevo parámetro
    @stateFundsDenied bit,
    @stateFundsDeniedReason nvarchar(max),
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
        AgencyId, NonProfit, FederalFundsDenied, FederalFundsDeniedReason, -- Nuevo campo
        StateFundsDenied, StateFundsDeniedReason,
        OrganizedAthleticPrograms, AtRiskService,
        BasicEducationRegistry, ExtendedHours, ServiceTime,
        TaxExemptionStatusId, TaxExemptionTypeId,
        PublicAllianceContractId, NationalYouthProgram, IsDayCareHome, DeadlineToCompleteRegistration
        )
    VALUES
        (
            @agencyId, @nonProfit, @federalFundsDenied, @federalFundsDeniedReason, -- Nuevo valor
            @stateFundsDenied, @stateFundsDeniedReason,
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