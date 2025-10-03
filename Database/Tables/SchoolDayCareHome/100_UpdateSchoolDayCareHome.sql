CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolDayCareHome]
    @id INT,
    @schoolId INT = NULL,
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
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SchoolDayCareHome
    SET SchoolId = ISNULL(@schoolId, SchoolId),
        IsAuthorizedToOperate = ISNULL(@isAuthorizedToOperate, IsAuthorizedToOperate),
        HasFamilyDepartmentLicense = ISNULL(@hasFamilyDepartmentLicense, HasFamilyDepartmentLicense),
        NumberOfEnrolledChildren = ISNULL(@numberOfEnrolledChildren, NumberOfEnrolledChildren),
        NumberOfProviderChildren = ISNULL(@numberOfProviderChildren, NumberOfProviderChildren),
        NumberOfParticipantsWithBloodTies = ISNULL(@numberOfParticipantsWithBloodTies, NumberOfParticipantsWithBloodTies),
        NumberOfParticipantsWithoutBloodTies = ISNULL(@numberOfParticipantsWithoutBloodTies, NumberOfParticipantsWithoutBloodTies),
        MinorsLiveWithProvider = ISNULL(@minorsLiveWithProvider, MinorsLiveWithProvider),
        RelationshipTypeId = ISNULL(@relationshipTypeId, RelationshipTypeId),
        OffersServiceToImmigrantChildren = ISNULL(@offersServiceToImmigrantChildren, OffersServiceToImmigrantChildren),
        HomeTypeId = ISNULL(@homeTypeId, HomeTypeId),
        AdministratorAuthorizedName = ISNULL(@administratorAuthorizedName, AdministratorAuthorizedName),
        AdministratorBirthDate = ISNULL(@administratorBirthDate, AdministratorBirthDate),
        OffersServiceToDifferentGroups = ISNULL(@offersServiceToDifferentGroups, OffersServiceToDifferentGroups),
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
END;

-- Ejemplo de uso:
-- EXEC [100_UpdateSchoolDayCareHome] @id = 1, @isAuthorizedToOperate = 1, @numberOfEnrolledChildren = 15, @schoolId = 1, @rowsAffected = 0 OUTPUT;
