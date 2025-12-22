-- =============================================
-- Stored Procedure: 100_GetAllowedDaysByProgramId
-- Descripción: Obtiene los días de la semana permitidos para un programa específico
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllowedDaysByProgramId]
    @ProgramId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pod.[DayOfWeek] AS [Id],
        dow.[Name],
        dow.[NameEN]
    FROM
        [dbo].[ProgramOperatingDays] pod
    INNER JOIN
        [dbo].[DayOfWeek] dow ON pod.[DayOfWeek] = dow.[Id]
    WHERE
        pod.[ProgramId] = @ProgramId
        AND pod.[IsAllowed] = 1
    ORDER BY
        pod.[DayOfWeek];
END;
GO

