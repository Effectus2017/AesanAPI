-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: 2025-03-07 19:52:37
-- Archivo original: /Volumes/Mac/Proyectos/AESAN/Proyecto/Api/Database/StoredProcedures/MealType/100_AllStoredProceduresForMealType.sql
-- =============================================

-- =============================================
-- Procedimientos almacenados para MealType
-- Versión: 100
-- =============================================

-- Procedimiento para eliminar un tipo de comida
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteMealType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rowsAffected INT;

    DELETE FROM dbo.MealType
    WHERE Id = @id;
    
    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO

-- Procedimiento para obtener todos los tipos de comida
CREATE OR ALTER PROCEDURE [dbo].[100_GetAllMealTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id,
           Name
    FROM MealType
     WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) FROM MealType
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
END;
GO

-- Procedimiento para obtener un tipo de comida por ID
CREATE OR ALTER PROCEDURE [dbo].[100_GetMealTypeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id,
           Name
    FROM MealType
    WHERE Id = @id;
END;
GO

-- Procedimiento para insertar un nuevo tipo de comida
CREATE OR ALTER PROCEDURE [dbo].[100_InsertMealType]
    @name NVARCHAR(100),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO MealType (Name)
    VALUES (@name);
    
    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

-- Procedimiento para actualizar un tipo de comida
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateMealType]
    @id INT,
    @name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rowsAffected INT;

    UPDATE MealType
        SET Name = @name
    WHERE Id = @id;
    
    SELECT @rowsAffected = @@ROWCOUNT;

    RETURN @rowsAffected;
END;
GO 
GO
