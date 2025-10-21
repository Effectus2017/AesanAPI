CREATE OR ALTER PROCEDURE [dbo].[100_GetOrganizationTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, DisplayOrder, RequiresCenterType
    FROM OrganizationType
    WHERE Id = @id;
END 