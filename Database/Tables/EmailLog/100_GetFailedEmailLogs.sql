-- =============================================
-- Stored Procedure: 100_GetFailedEmailLogs
-- Fecha: 2026-01-15
-- Descripción: Obtiene los logs de correos fallidos para un email específico
--              Útil para identificar correos que pueden ser reenviados
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetFailedEmailLogs]
    @email NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        id = Id,
        recipientemail = RecipientEmail,
        subject = Subject,
        emailtype = EmailType,
        status = Status,
        errormessage = ErrorMessage,
        sentat = SentAt,
        attemptedat = AttemptedAt,
        userid = UserId,
        agencyid = AgencyId,
        emailtemplatekey = EmailTemplateKey,
        retrycount = RetryCount,
        originalemaillogid = OriginalEmailLogId,
        createdat = CreatedAt,
        createdby = CreatedBy
    FROM [dbo].[EmailLog]
    WHERE [Status] = 'Failed'
        AND (@email IS NULL OR [RecipientEmail] = @email)
    ORDER BY [AttemptedAt] DESC;
END;
GO
