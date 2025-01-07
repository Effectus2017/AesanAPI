CREATE OR ALTER PROCEDURE [dbo].[100_GetAllEducationLevels]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM EducationLevel
     WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) FROM EducationLevel
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertEducationLevel]
    @name NVARCHAR(100),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO EducationLevel (Name)
    VALUES (@name);

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteEducationLevel]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    DELETE FROM EducationLevel
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_GetEducationLevelById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM EducationLevel
    WHERE Id = @id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateEducationLevel]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    UPDATE EducationLevel
        SET Name = @name
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO 