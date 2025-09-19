CREATE PROCEDURE [dbo].[100_GetOrganizationTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, DisplayOrder
    FROM OrganizationType
    WHERE Id = @id;
END 