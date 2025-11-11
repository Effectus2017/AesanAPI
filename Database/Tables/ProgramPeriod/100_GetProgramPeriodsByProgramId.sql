-- =============================================
-- Stored Procedure: 100_GetProgramPeriodsByProgramId
-- Descripción: Obtiene todos los períodos de un programa
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetProgramPeriodsByProgramId]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        ProgramId,
        Year,
        StartDate,
        EndDate,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM ProgramPeriod
    WHERE ProgramId = @programId
        AND IsActive = 1
    ORDER BY Year DESC;
END;
GO

