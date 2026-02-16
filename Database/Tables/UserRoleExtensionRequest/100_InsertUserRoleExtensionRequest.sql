-- =============================================
-- Stored Procedure: 100_InsertUserRoleExtensionRequest
-- Descripción: Inserta una solicitud de extensión de vigencia de rol secundario.
-- Parámetros y alias de salida: lowercase; columnas en cuerpo: CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertUserRoleExtensionRequest]
    @userId NVARCHAR(450),
    @roleId NVARCHAR(450),
    @requestedValidTo DATE,
    @reason NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO UserRoleExtensionRequest (UserId, RoleId, RequestedValidTo, Reason, Status)
    VALUES (@userId, @roleId, @requestedValidTo, @reason, 'Pending');

    SELECT id = SCOPE_IDENTITY();
END;
GO
