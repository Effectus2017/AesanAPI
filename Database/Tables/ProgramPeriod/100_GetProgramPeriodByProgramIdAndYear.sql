-- =============================================
-- Stored Procedure: 100_GetProgramPeriodByProgramIdAndYear
-- Descripción: Obtiene un período específico de un programa por año
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetProgramPeriodByProgramIdAndYear]
    @programId INT,
    @year INT
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
        AND Year = @year
        AND IsActive = 1;
END;
GO

