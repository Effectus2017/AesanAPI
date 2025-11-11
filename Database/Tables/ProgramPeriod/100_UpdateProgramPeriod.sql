-- =============================================
-- Stored Procedure: 100_UpdateProgramPeriod
-- Descripción: Actualiza un período de programa existente
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateProgramPeriod]
    @id INT,
    @programId INT,
    @year INT,
    @startDate DATE,
    @endDate DATE,
    @isActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE ProgramPeriod
        SET ProgramId = @programId,
            Year = @year,
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

