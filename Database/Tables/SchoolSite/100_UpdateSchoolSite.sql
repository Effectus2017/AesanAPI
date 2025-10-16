-- =============================================
-- Stored Procedure: 100_UpdateSchoolSite
-- Descripción: Actualiza una asignación School-Site
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolSite]
    @id INT,
    @schoolId INT,
    @siteId INT,
    @comment NVARCHAR(500) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Verificar que la asignación existe
        IF NOT EXISTS (SELECT 1
    FROM [SchoolSite]
    WHERE [Id] = @id)
        BEGIN
        RAISERROR('La asignación con ID %d no existe', 16, 1, @id);
        RETURN;
    END
        
        -- Verificar que la School existe y está activa
        IF NOT EXISTS (SELECT 1
    FROM [School]
    WHERE [Id] = @schoolId AND [IsActive] = 1)
        BEGIN
        RAISERROR('La escuela con ID %d no existe o no está activa', 16, 1, @schoolId);
        RETURN;
    END
        
        -- Verificar que el Site existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM [Site]
    WHERE [Id] = @siteId AND [IsActive] = 1)
        BEGIN
        RAISERROR('El sitio con ID %d no existe o no está activo', 16, 1, @siteId);
        RETURN;
    END
        
        -- Verificar que el Site no esté asignado a otra School (si cambió el SiteId)
        IF EXISTS (SELECT 1
    FROM [SchoolSite]
    WHERE [SiteId] = @siteId AND [Id] != @id AND [IsActive] = 1)
        BEGIN
        RAISERROR('El sitio ya está asignado a otra escuela', 16, 1);
        RETURN;
    END
        
        -- Actualizar asignación
        UPDATE [SchoolSite]
        SET 
            [SchoolId] = @schoolId,
            [SiteId] = @siteId,
            [Comment] = @comment,
            [IsActive] = @isActive,
            [UpdatedAt] = GETDATE()
        WHERE [Id] = @id;
        
        SET @rowsAffected = @@ROWCOUNT;
        
        COMMIT TRANSACTION;
        
        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
