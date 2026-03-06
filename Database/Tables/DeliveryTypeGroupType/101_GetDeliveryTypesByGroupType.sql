-- Versión 101: añade filtro opcional por programa (@programid).
-- Parámetros: @grouptypeid (requerido), @programid (opcional). Si @programid no es NULL, solo devuelve tipos de entrega válidos para ese programa.
CREATE OR ALTER PROCEDURE [101_GetDeliveryTypesByGroupType]
    @grouptypeid INT,
    @programid INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = dt.Id,
        name = dt.Name,
        nameen = dt.NameEN,
        isactive = dt.IsActive,
        displayorder = dt.DisplayOrder,
        requirespermission = dtg.RequiresPermission,
        createdat = dt.CreatedAt,
        updatedat = dt.UpdatedAt
    FROM DeliveryType dt
        INNER JOIN DeliveryTypeGroupType dtg ON dt.Id = dtg.DeliveryTypeId
        LEFT JOIN DeliveryTypeProgram dtp ON dt.Id = dtp.DeliveryTypeId AND dtp.ProgramId = @programid
    WHERE dtg.GroupTypeId = @grouptypeid
        AND dt.IsActive = 1
        AND dtg.IsActive = 1
        AND (@programid IS NULL OR dtp.ProgramId IS NOT NULL)
    ORDER BY dt.DisplayOrder, dt.Name;
END;
GO

GO