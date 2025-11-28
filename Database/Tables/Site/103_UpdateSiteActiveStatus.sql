-- =============================================
-- Stored Procedure: 103_UpdateSiteActiveStatus
-- Descripción: Actualiza el estado activo/inactivo de un sitio
-- Reemplaza: 103_UpdateSchoolActiveStatus
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[103_UpdateSiteActiveStatus]
    @id INT,
    @isActive BIT,
    @inactiveJustification NVARCHAR(500) = NULL,
    @inactiveDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE Site 
        SET 
            IsActive = @isActive,
            InactiveJustification = @inactiveJustification,
            InactiveDate = CASE WHEN @isActive = 0 THEN ISNULL(@inactiveDate, GETDATE()) ELSE NULL END,
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @rowsAffected = @@ROWCOUNT;

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
