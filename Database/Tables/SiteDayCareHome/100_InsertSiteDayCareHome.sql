-- =============================================
-- Stored Procedure: 100_InsertSiteDayCareHome
-- Descripción: Inserta información de Day Care Home para un sitio
-- Reemplaza: 100_InsertSchoolDayCareHome
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteDayCareHome]
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
    @administratorAuthorizedName NVARCHAR(255) = NULL,
    @administratorBirthDate DATE = NULL,
    @offersServiceToDifferentGroups BIT = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteDayCareHome
        (
        SiteId, IsAuthorizedToOperate, HasFamilyDepartmentLicense, NumberOfEnrolledChildren, NumberOfProviderChildren,
        NumberOfParticipantsWithBloodTies, NumberOfParticipantsWithoutBloodTies, MinorsLiveWithProvider,
        RelationshipTypeId, OffersServiceToImmigrantChildren, HomeTypeId, AdministratorAuthorizedName,
        AdministratorBirthDate, OffersServiceToDifferentGroups, CreatedAt
        )
    VALUES
        (
            @siteId, @isAuthorizedToOperate, @hasFamilyDepartmentLicense, @numberOfEnrolledChildren, @numberOfProviderChildren,
            @numberOfParticipantsWithBloodTies, @numberOfParticipantsWithoutBloodTies, @minorsLiveWithProvider,
            @relationshipTypeId, @offersServiceToImmigrantChildren, @homeTypeId, @administratorAuthorizedName,
            @administratorBirthDate, @offersServiceToDifferentGroups, GETDATE()
    );

    SET @id = SCOPE_IDENTITY();
END;
