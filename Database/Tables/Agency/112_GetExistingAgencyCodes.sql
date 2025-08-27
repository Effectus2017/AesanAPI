CREATE OR ALTER PROCEDURE [112_GetExistingAgencyCodes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AgencyCode
    FROM Agency
    WHERE AgencyCode IS NOT NULL AND LTRIM(RTRIM(AgencyCode)) <> '';
END;
GO
