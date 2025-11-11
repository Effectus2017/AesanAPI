-- =============================================
-- Stored Procedure: 100_UpdateSiteProgram
-- Descripción: Actualiza una relación sitio-programa existente
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteProgram]
    @id INT,
    @siteId INT,
    @programId INT,
    @startDate DATE,
    @endDate DATE,
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE SiteProgram
        SET SiteId = @siteId,
            ProgramId = @programId,
            StartDate = @startDate,
            EndDate = @endDate,
            IsActive = @isActive,
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
GO

