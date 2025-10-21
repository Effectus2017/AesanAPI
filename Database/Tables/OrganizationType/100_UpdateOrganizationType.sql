CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOrganizationType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT,
    @displayOrder INT,
    @requiresCenterType BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE OrganizationType
    SET Name = @name, NameEN = @nameEN, IsActive = @isActive, DisplayOrder = @displayOrder, RequiresCenterType = @requiresCenterType
    WHERE Id = @id;
END 