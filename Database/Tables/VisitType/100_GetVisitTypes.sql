-- =============================================
-- SP: 100_GetVisitTypes — tipos de visita para dropdown / listas
--   @alls = 0: solo activos (comportamiento por defecto en calendario)
--   @alls = 1: incluye inactivos (p. ej. mantenimiento)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_GetVisitTypes]
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id = vt.Id,
        code = vt.Code,
        nameEs = vt.NameEs,
        nameEN = vt.NameEN,
        sortOrder = vt.SortOrder,
        ruleMaxWeeksFromProgramStart = vt.RuleMaxWeeksFromProgramStart
    FROM [dbo].[VisitType] vt
    WHERE (@alls = 1 OR vt.IsActive = 1)
    ORDER BY vt.SortOrder ASC, vt.Id ASC;
END
GO
