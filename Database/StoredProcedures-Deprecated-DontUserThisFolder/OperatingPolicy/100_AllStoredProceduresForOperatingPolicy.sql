/*
Archivo consolidado de Stored Procedures para la entidad OperatingPolicy
Version: 1.0.0
Fecha: 2024-01-07
*/

-- Procedimiento para obtener todas las políticas operativas
CREATE OR ALTER PROCEDURE [dbo].[100_GetAllOperatingPolicies]
    @take INT,
    @skip INT,
    @description NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Description
    FROM dbo.OperatingPolicy
    WHERE (@alls = 1)
        OR
        (@description IS NULL OR Description LIKE '%' + @description + '%')
    ORDER BY Description
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM dbo.OperatingPolicy
    WHERE (@alls = 1)
        OR
        (@description IS NULL OR Description LIKE '%' + @description + '%');
END;
GO

-- Procedimiento para obtener una política operativa por ID
CREATE OR ALTER PROCEDURE [dbo].[100_GetOperatingPolicyById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Description
    FROM dbo.OperatingPolicy
    WHERE Id = @id;
END;
GO

-- Procedimiento para insertar una nueva política operativa
CREATE OR ALTER PROCEDURE [dbo].[100_InsertOperatingPolicy]
    @description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.OperatingPolicy (Description)
    VALUES (@description);

    SELECT SCOPE_IDENTITY() AS Id;
END;
GO

-- Procedimiento para actualizar una política operativa existente
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateOperatingPolicy]
    @id INT,
    @description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.OperatingPolicy
    SET Description = @description
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO

-- Procedimiento para eliminar una política operativa
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteOperatingPolicy]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.OperatingPolicy
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;
GO 