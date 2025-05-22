/*
Archivo consolidado de Stored Procedures para la entidad OperatingPeriod
Version: 1.0.0
Fecha: 2024-01-07
*/

-- Procedimiento para obtener todos los períodos operativos
CREATE OR ALTER PROCEDURE [dbo].[100_GetAllOperatingPeriods]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.OperatingPeriod
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM dbo.OperatingPeriod
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%');
END;
GO

-- Procedimiento para obtener un período operativo por ID
CREATE OR ALTER PROCEDURE [dbo].[100_GetOperatingPeriodById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.OperatingPeriod
    WHERE Id = @id;
END;
GO

-- Procedimiento para insertar un nuevo período operativo
CREATE OR ALTER PROCEDURE [dbo].[100_InsertOperatingPeriod]
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.OperatingPeriod (Name)
    VALUES (@name);

    SELECT SCOPE_IDENTITY() AS Id;
END;
GO

-- Procedimiento para actualizar un período operativo existente
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOperatingPeriod]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.OperatingPeriod
    SET Name = @name
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO

-- Procedimiento para eliminar un período operativo
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteOperatingPeriod]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.OperatingPeriod
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO 