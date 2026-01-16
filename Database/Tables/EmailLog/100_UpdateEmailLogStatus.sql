-- =============================================
-- Stored Procedure: 100_UpdateEmailLogStatus
-- Fecha: 2026-01-15
-- Descripción: Actualiza el estado de un log de correo electrónico
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateEmailLogStatus]
    @id INT,
    @status NVARCHAR(50),
    @errormessage NVARCHAR(MAX) = NULL,
    @sentat DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rowsAffected INT;
    
    UPDATE [dbo].[EmailLog]
    SET 
        [Status] = @status,
        [ErrorMessage] = @errormessage,
        [SentAt] = CASE 
            WHEN @status = 'Sent' AND @sentat IS NOT NULL THEN @sentat
            WHEN @status = 'Sent' AND @sentat IS NULL THEN GETDATE()
            ELSE [SentAt]
        END
    WHERE [Id] = @id;
    
    SET @rowsAffected = @@ROWCOUNT;
    
    -- Retornar el número de filas afectadas
    SELECT rowsAffected = @rowsAffected;
END;
GO
