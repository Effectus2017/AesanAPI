-- =============================================
-- Stored Procedure: 100_HasMainStaff
-- =============================================
-- Verifica si existe un miembro del staff principal
-- Retorna 1 si existe, 0 si no existe

CREATE OR ALTER PROCEDURE [dbo].[100_HasMainStaff]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM Staff
    WHERE IsActive = 1;
END