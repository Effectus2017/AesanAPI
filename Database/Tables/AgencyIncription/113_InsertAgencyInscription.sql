-- Insertar una inscripción de agencia
-- 1.4.0 - TaxExemptionStatusId nullable (PACNA sin fines de lucro = No: no aplica exención)
CREATE OR ALTER PROCEDURE [113_InsertAgencyInscription]
    @agencyId int,
    @nonProfit bit,
    @federalFundsDenied bit,
    @federalFundsDeniedReason nvarchar(max),
    @stateFundsDenied bit,
    @stateFundsDeniedReason nvarchar(max),
    @basicEducationRegistry bit,
    @extendedHours bit,
    @servicesOfferedSince datetime = NULL,
    @taxExemptionStatusId int = NULL,
    @taxExemptionTypeId int,
    @typeOfEntityId int = NULL,
    @typeOfApplicantId int = NULL,
    @publicAllianceContractId int = NULL,
    @nationalYouthProgram bit,
    @isDayCareHomeId int = NULL,
    @participatesInHeadStartProgramId int = NULL,
    @boardMeetingsPerYear int = NULL,
    @boardMeetsRegularly bit = NULL,
    @deadlineToCompleteRegistration datetime,
    @id int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO AgencyInscription
        (
            AgencyId, NonProfit, FederalFundsDenied, FederalFundsDeniedReason,
            StateFundsDenied, StateFundsDeniedReason,
            BasicEducationRegistry, ExtendedHours, ServicesOfferedSince,
            TaxExemptionStatusId, TaxExemptionTypeId, TypeOfEntityId, TypeOfApplicantId,
            PublicAllianceContractId, NationalYouthProgram, IsDayCareHomeId, ParticipatesInHeadStartProgramId,
            BoardMeetingsPerYear, BoardMeetsRegularly,
            DeadlineToCompleteRegistration
        )
        VALUES
        (
            @agencyId, @nonProfit, @federalFundsDenied, @federalFundsDeniedReason,
            @stateFundsDenied, @stateFundsDeniedReason,
            @basicEducationRegistry, @extendedHours, @servicesOfferedSince,
            @taxExemptionStatusId, @taxExemptionTypeId, @typeOfEntityId, @typeOfApplicantId,
            @publicAllianceContractId, @nationalYouthProgram, @isDayCareHomeId, @participatesInHeadStartProgramId,
            @boardMeetingsPerYear, @boardMeetsRegularly,
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
