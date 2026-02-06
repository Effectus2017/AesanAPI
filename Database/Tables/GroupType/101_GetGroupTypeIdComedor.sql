-- =============================================
-- Stored Procedure: 101_GetGroupTypeIdComedor
-- Descripción: Obtiene el ID del tipo de grupo Comedor (Dining Room).
-- Usado para validar regla: solo un sitio Comedor por escuela.
-- Fecha: 2026-02-05
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetGroupTypeIdComedor]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = Id
    FROM GroupType
    WHERE Name = N'Comedor' OR NameEN = N'Dining Room';
END;
