CREATE OR ALTER PROCEDURE [dbo].[100_UpdateEducationLevel]
    @id INT,
    @name NVARCHAR(100),
    @nameEN NVARCHAR(100),
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    UPDATE EducationLevel
        SET Name = @name,
            NameEN = @nameEN,
            IsActive = @isActive,
            UpdatedAt = GETDATE()
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO 