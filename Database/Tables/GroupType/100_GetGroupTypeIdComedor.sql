-- =============================================
-- Stored Procedure: 100_GetGroupTypeIdComedor
-- Versión original: obtiene el ID del tipo de grupo Comedor por Name/NameEN.
-- La aplicación usa 101_GetGroupTypeIdComedor (filtro por Code).
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetGroupTypeIdComedor]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = Id
    FROM GroupType
    WHERE Name = N'Comedor' OR NameEN = N'Dining Room';
END;
