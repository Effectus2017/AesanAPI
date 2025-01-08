CREATE OR ALTER PROCEDURE [dbo].[100_GetAllAlternativeCommunications]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM AlternativeCommunication
     WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) FROM AlternativeCommunication
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
END;

CREATE OR ALTER PROCEDURE [dbo].[100_InsertAlternativeCommunication]
    @name NVARCHAR(100),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AlternativeCommunication (Name)
    VALUES (@name);

    SET @id = SCOPE_IDENTITY();
    RETURN @id;

END;

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteAlternativeCommunication]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    DELETE FROM AlternativeCommunication
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;

CREATE OR ALTER PROCEDURE [dbo].[100_GetAlternativeCommunicationById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM AlternativeCommunication
    WHERE Id = @id;
END;

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateAlternativeCommunication]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    UPDATE AlternativeCommunication
        SET Name = @name
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END; 