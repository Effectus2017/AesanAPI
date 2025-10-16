-- =============================================
-- Stored Procedure: 100_DeleteSchool
-- Descripción: Elimina una escuela (soft delete)
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchool]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Verificar que la escuela existe
        IF NOT EXISTS (SELECT 1
    FROM [School]
    WHERE [Id] = @id)
        BEGIN
        RAISERROR('La escuela con ID %d no existe', 16, 1, @id);
        RETURN;
    END
        
        -- Verificar que no tenga Sites asignados
        IF EXISTS (SELECT 1
    FROM [SchoolSite]
    WHERE [SchoolId] = @id AND [IsActive] = 1)
        BEGIN
        RAISERROR('No se puede eliminar la escuela porque tiene sitios asignados', 16, 1);
        RETURN;
    END
        
        -- Soft delete de la escuela
        UPDATE [School]
        SET 
            [IsActive] = 0,
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
