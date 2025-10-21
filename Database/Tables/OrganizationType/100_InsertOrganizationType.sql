CREATE OR ALTER PROCEDURE [dbo].[100_InsertOrganizationType]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT,
    @displayOrder INT,
    @requiresCenterType BIT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO OrganizationType
        (Name, NameEN, IsActive, DisplayOrder, RequiresCenterType)
    VALUES
        (@name, @nameEN, @isActive, @displayOrder, @requiresCenterType);

    SET @id = SCOPE_IDENTITY();
END 