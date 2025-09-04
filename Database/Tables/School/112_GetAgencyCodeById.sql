-- Obtener código de agencia por ID
CREATE OR ALTER PROCEDURE [dbo].[112_GetAgencyCodeById]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AgencyCode
    FROM Agency
    WHERE Id = @agencyId;
END;
