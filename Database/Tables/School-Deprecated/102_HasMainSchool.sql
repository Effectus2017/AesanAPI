-- =============================================
-- Author:      Sistema AESAN
-- Create date: Fecha de Creación
-- Description: Verifica si existe una escuela principal en la base de datos
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[102_HasMainSchool]
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si existe al menos una escuela marcada como principal
    SELECT COUNT(1)
    FROM School
    WHERE IsMainSchool = 1 AND IsActive = 1;
END
GO