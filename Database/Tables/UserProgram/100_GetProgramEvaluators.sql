-- =============================================
-- Stored Procedure: 100_GetProgramEvaluators
-- Descripción: Obtiene los UserIds de los evaluadores asignados a una lista de programas
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetProgramEvaluators]
    @programIds NVARCHAR(MAX) -- Comma separated IDs
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT up.UserId
    FROM UserProgram up
        INNER JOIN AspNetUsers u ON up.UserId = u.Id
        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE up.ProgramId IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@programIds, ','))
        AND up.IsActive = 1
        AND r.Name = 'Evaluador';
END;
GO
