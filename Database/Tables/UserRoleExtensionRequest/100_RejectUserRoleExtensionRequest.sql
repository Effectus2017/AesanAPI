-- =============================================
-- Stored Procedure: 100_RejectUserRoleExtensionRequest
-- Descripción: Rechaza una solicitud de extensión.
-- Parámetros: lowercase; columnas: CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_RejectUserRoleExtensionRequest]
    @id INT,
    @processedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM UserRoleExtensionRequest WHERE Id = @id AND Status = 'Pending')
    BEGIN
        SELECT success = 0, errormessage = 'Solicitud no encontrada o ya procesada.';
        RETURN;
    END

    UPDATE UserRoleExtensionRequest
    SET Status = 'Rejected', ProcessedAt = GETUTCDATE(), ProcessedBy = @processedBy
    WHERE Id = @id;

    SELECT success = 1;
END;
GO
