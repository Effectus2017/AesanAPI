-- =============================================
-- Stored Procedure: 100_ApproveUserRoleExtensionRequest
-- Descripción: Aprueba una solicitud y actualiza ValidTo del rol secundario en AspNetUserRoles.
-- Parámetros: lowercase; columnas: CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ApproveUserRoleExtensionRequest]
    @id INT,
    @newValidTo DATE = NULL,
    @processedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userId NVARCHAR(450);
    DECLARE @roleId NVARCHAR(450);
    DECLARE @requestedValidTo DATE;

    SELECT @userId = UserId, @roleId = RoleId, @requestedValidTo = RequestedValidTo
    FROM UserRoleExtensionRequest
    WHERE Id = @id AND Status = 'Pending';

    IF @userId IS NULL
    BEGIN
        SELECT success = 0, errormessage = 'Solicitud no encontrada o ya procesada.';
        RETURN;
    END

    DECLARE @validTo DATE = COALESCE(@newValidTo, @requestedValidTo);

    UPDATE AspNetUserRoles
    SET ValidTo = @validTo, UpdatedAt = GETDATE()
    WHERE UserId = @userId AND RoleId = @roleId AND IsPrimary = 0;

    UPDATE UserRoleExtensionRequest
    SET Status = 'Approved', ProcessedAt = GETUTCDATE(), ProcessedBy = @processedBy
    WHERE Id = @id;

    SELECT success = 1;
END;
GO
