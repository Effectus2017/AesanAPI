CREATE OR ALTER PROCEDURE [dbo].[100_InsertEducationLevel]
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO EducationLevel
        (Name, NameEN, IsActive, CreatedAt)
    VALUES
        (@name, @nameEN, @isActive, GETDATE());

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO