-- Deprecada
CREATE OR ALTER PROCEDURE [111_InsertAgencyInscription]
    @AgencyId int,
    @NonProfit bit,
    @FederalFundsDenied bit,
    @StateFundsDenied bit,
    @OrganizedAthleticPrograms bit,
    @AtRiskService bit,
    @BasicEducationRegistry int,
    @ServiceTime datetime,
    @TaxExemptionStatus int,
    @TaxExemptionType int,
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
        BasicEducationRegistry, ServiceTime,
        TaxExemptionStatus, TaxExemptionType
        )
    VALUES
        (
            @AgencyId, @NonProfit, @FederalFundsDenied, @StateFundsDenied,
            @OrganizedAthleticPrograms, @AtRiskService,
            @BasicEducationRegistry, @ServiceTime,
            @TaxExemptionStatus, @TaxExemptionType
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