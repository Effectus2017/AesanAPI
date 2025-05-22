-- Procedimiento para obtener todas las autoridades alimentarias
CREATE OR ALTER PROCEDURE [dbo].[100_GetAllFoodAuthorities]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           Description,
           CreatedAt
    FROM dbo.FoodAuthority
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM dbo.FoodAuthority
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%');
END;
GO

-- Procedimiento para obtener una autoridad alimentaria por ID
CREATE OR ALTER PROCEDURE [dbo].[100_GetFoodAuthorityById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           Description,
           CreatedAt
    FROM dbo.FoodAuthority
    WHERE Id = @id;
END;
GO

-- Procedimiento para insertar una nueva autoridad alimentaria
CREATE OR ALTER PROCEDURE [dbo].[100_InsertFoodAuthority]
    @name NVARCHAR(100),
    @description NVARCHAR(MAX),
    @createdAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.FoodAuthority (Name, Description, CreatedAt)
    VALUES (@name, @description, @createdAt);

    SELECT SCOPE_IDENTITY() AS Id;
END;
GO

-- Procedimiento para actualizar una autoridad alimentaria existente
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateFoodAuthority]
    @id INT,
    @name NVARCHAR(100),
    @description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.FoodAuthority
    SET Name = @name,
        Description = @description
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO

-- Procedimiento para eliminar una autoridad alimentaria
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteFoodAuthority]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.FoodAuthority
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO 