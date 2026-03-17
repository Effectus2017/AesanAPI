-- =============================================
-- Stored Procedure: 101_GetGroupTypesByProgram
-- Versión 2: incluye Code; acepta @schoolid opcional; si la escuela ya tiene sitio Comedor, excluye Comedor del resultado.
-- Descripción: Obtiene los tipos de grupo válidos para un programa (con Code).
--              Si se envía @schoolid y esa escuela ya tiene un sitio Comedor, no se incluye Comedor (un solo Comedor por escuela).
-- Parámetros en lowercase según convención.
-- Fecha: 2026-03-16
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetGroupTypesByProgram]
    @programid INT,
    @schoolid INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @comedorId INT = (SELECT Id FROM GroupType WHERE Code = N'DINING_ROOM');

    DECLARE @schoolHasComedor BIT = 0;
    IF @schoolid IS NOT NULL AND @comedorId IS NOT NULL
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM Site s
                INNER JOIN SchoolSite ss ON s.Id = ss.SiteId
            WHERE ss.SchoolId = @schoolid
                AND ss.IsActive = 1
                AND s.GroupTypeId = @comedorId
        )
            SET @schoolHasComedor = 1;
    END

    SELECT id = gt.Id,
        name = gt.Name,
        nameen = gt.NameEN,
        code = gt.Code,
        isactive = gt.IsActive,
        displayorder = gt.DisplayOrder,
        createdat = gt.CreatedAt,
        updatedat = gt.UpdatedAt
    FROM GroupType gt
        INNER JOIN GroupTypeProgram gtp ON gt.Id = gtp.GroupTypeId
    WHERE gtp.ProgramId = @programid
        AND gt.IsActive = 1
        AND gtp.IsActive = 1
        AND (
            @schoolHasComedor = 0
            OR gt.Id != @comedorId
        )
    ORDER BY gt.DisplayOrder, gt.Name;
END;
GO
