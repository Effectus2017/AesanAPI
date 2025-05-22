
-- Procedimiento para obtener todas las instalaciones
CREATE OR ALTER PROCEDURE [dbo].[100_GetAllFacilities]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.Facility
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM dbo.Facility
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%');
END;
GO

-- Procedimiento para obtener una instalación por ID
CREATE OR ALTER PROCEDURE [dbo].[100_GetFacilityById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.Facility
    WHERE Id = @id;
END;
GO

-- Procedimiento para insertar una nueva instalación
CREATE OR ALTER PROCEDURE [dbo].[100_InsertFacility]
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Facility (Name)
    VALUES (@name);

    SELECT SCOPE_IDENTITY() AS Id;
END;
GO

-- Procedimiento para actualizar una instalación existente
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateFacility]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Facility
    SET Name = @name
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO

-- Procedimiento para eliminar una instalación
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteFacility]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Facility
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO 