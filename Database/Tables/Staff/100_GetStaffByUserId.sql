-- =============================================
-- Stored Procedure: 100_GetStaffByUserId
-- =============================================
-- Obtiene un miembro del staff específico por su UserId
-- Retorna solo los campos necesarios para el token JWT

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffByUserId]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.UserId,
        s.IsActive
    FROM Staff s
    WHERE s.UserId = @userId
        AND s.IsActive = 1;
END

