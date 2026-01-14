-- Insertar una inscripción de agencia
-- 1.1.9 - Agregado ParticipatesInHeadStartProgramId (Solo para PSAV)
-- 1.2.0 - Agregado BoardMeetingsPerYear, BoardMeetsRegularly, BoardExecutiveAuthority (Solo para PACNA)
CREATE OR ALTER PROCEDURE [112_InsertAgencyInscription]
    @agencyId int,
    @nonProfit bit,
    @federalFundsDenied bit,
    @federalFundsDeniedReason nvarchar(max),
    -- Nuevo parámetro
    @stateFundsDenied bit,
    @stateFundsDeniedReason nvarchar(max),
    @basicEducationRegistry bit,
    @extendedHours bit,
    -- Nuevo: ¿Desde cuándo su Entidad ofrece servicios?
    @servicesOfferedSince datetime = NULL,
    @taxExemptionStatusId int,
    @taxExemptionTypeId int,
    @publicAllianceContractId int = NULL,
    @nationalYouthProgram bit,
    @isDayCareHomeId int = NULL,
    @participatesInHeadStartProgramId int = NULL,
    @boardMeetingsPerYear int = NULL,
    @boardMeetsRegularly bit = NULL,
    @boardExecutiveAuthority nvarchar(max) = NULL,
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
        BasicEducationRegistry, ExtendedHours, ServicesOfferedSince,
        TaxExemptionStatusId, TaxExemptionTypeId,
        PublicAllianceContractId, NationalYouthProgram, IsDayCareHomeId, ParticipatesInHeadStartProgramId,
        BoardMeetingsPerYear, BoardMeetsRegularly, BoardExecutiveAuthority,
        DeadlineToCompleteRegistration
        )
    VALUES
        (
            @agencyId, @nonProfit, @federalFundsDenied, @federalFundsDeniedReason, -- Nuevo valor
            @stateFundsDenied, @stateFundsDeniedReason,
            @basicEducationRegistry, @extendedHours, @servicesOfferedSince,
            @taxExemptionStatusId, @taxExemptionTypeId,
            @publicAllianceContractId, @nationalYouthProgram, @isDayCareHomeId, @participatesInHeadStartProgramId,
            @boardMeetingsPerYear, @boardMeetsRegularly, @boardExecutiveAuthority,
            @deadlineToCompleteRegistration
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