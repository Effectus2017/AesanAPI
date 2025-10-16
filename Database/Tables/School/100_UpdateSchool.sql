-- =============================================
-- Stored Procedure: 100_UpdateSchool
-- Descripción: Actualiza una escuela existente (SchoolCode y SchoolNumber no se pueden modificar)
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchool]
    @id INT,
    @agencyId INT,
    @name NVARCHAR(255),
    @isActive BIT = 1
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
        
        -- Actualizar escuela (SchoolCode y SchoolNumber no se modifican)
        UPDATE [School]
        SET 
            [AgencyId] = @agencyId,
            [Name] = @name,
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