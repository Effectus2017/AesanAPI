-- =============================================
-- Stored Procedure: 100_InsertProgramPeriod
-- Descripción: Inserta un nuevo período de programa
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertProgramPeriod]
    @programId INT,
    @year INT,
    @startDate DATE,
    @endDate DATE,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ProgramPeriod
        (ProgramId, Year, StartDate, EndDate, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (@programId, @year, @startDate, @endDate, @isActive, GETDATE(), NULL);

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END;
GO

