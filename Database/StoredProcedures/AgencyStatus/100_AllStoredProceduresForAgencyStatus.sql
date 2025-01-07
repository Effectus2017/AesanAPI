CREATE OR ALTER PROCEDURE [dbo].[100_GetAllAgencyStatuses]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM AgencyStatus
     WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) FROM AgencyStatus
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertAgencyStatus]
    @name NVARCHAR(100),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AgencyStatus (Name)
    VALUES (@name);

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteAgencyStatus]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    DELETE FROM AgencyStatus
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyStatusById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM AgencyStatus
    WHERE Id = @id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateAgencyStatus]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @rowsAffected INT;

    UPDATE AgencyStatus
        SET Name = @name
    WHERE Id = @id;

    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO 