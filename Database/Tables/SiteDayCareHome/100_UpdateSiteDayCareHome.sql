-- =============================================
-- Stored Procedure: 100_UpdateSiteDayCareHome
-- Descripción: Actualiza información de Day Care Home para un sitio
-- Reemplaza: 100_UpdateSchoolDayCareHome
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteDayCareHome]
    @siteId INT,
    @isAuthorizedToOperate BIT = NULL,
    @hasFamilyDepartmentLicense BIT = NULL,
    @numberOfEnrolledChildren INT = NULL,
    @numberOfProviderChildren INT = NULL,
    @numberOfParticipantsWithBloodTies INT = NULL,
    @numberOfParticipantsWithoutBloodTies INT = NULL,
    @minorsLiveWithProvider BIT = NULL,
    @relationshipTypeId INT = NULL,
    @offersServiceToImmigrantChildren BIT = NULL,
    @homeTypeId INT = NULL,
    @administratorBirthDate DATE = NULL,
    @offersServiceToDifferentGroups BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE SiteDayCareHome 
        SET 
            IsAuthorizedToOperate = @isAuthorizedToOperate,
            HasFamilyDepartmentLicense = @hasFamilyDepartmentLicense,
            NumberOfEnrolledChildren = @numberOfEnrolledChildren,
            NumberOfProviderChildren = @numberOfProviderChildren,
            NumberOfParticipantsWithBloodTies = @numberOfParticipantsWithBloodTies,
            NumberOfParticipantsWithoutBloodTies = @numberOfParticipantsWithoutBloodTies,
            MinorsLiveWithProvider = @minorsLiveWithProvider,
            RelationshipTypeId = @relationshipTypeId,
            OffersServiceToImmigrantChildren = @offersServiceToImmigrantChildren,
            HomeTypeId = @homeTypeId,
            AdministratorBirthDate = @administratorBirthDate,
            OffersServiceToDifferentGroups = @offersServiceToDifferentGroups,
            UpdatedAt = GETDATE()
        WHERE SiteId = @siteId;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
