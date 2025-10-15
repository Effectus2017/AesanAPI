-- Obtener todos los códigos de sitios existentes
CREATE OR ALTER PROCEDURE [dbo].[112_GetExistingSiteCodes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT SiteCode
    FROM [Site]
    WHERE SiteCode IS NOT NULL AND SiteCode != ''
    ORDER BY SiteCode;
END;
