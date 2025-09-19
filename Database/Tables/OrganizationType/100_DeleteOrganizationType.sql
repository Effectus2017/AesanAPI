CREATE PROCEDURE [dbo].[100_DeleteOrganizationType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM OrganizationType WHERE Id = @id;
END 