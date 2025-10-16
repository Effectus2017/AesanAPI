-- =============================================
-- Stored Procedure: 100_DeleteSchoolSite
-- Descripción: Elimina una asignación School-Site (soft delete)
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchoolSite]
    @id INT
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
        
        -- Soft delete de la asignación
        UPDATE [SchoolSite]
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
