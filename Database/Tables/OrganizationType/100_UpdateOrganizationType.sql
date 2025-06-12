CREATE PROCEDURE [dbo].[100_UpdateOrganizationType]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT,
    @displayOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE OrganizationTypes
    SET Name = @name, NameEN = @nameEN, IsActive = @isActive, DisplayOrder = @displayOrder
    WHERE Id = @id;
END 