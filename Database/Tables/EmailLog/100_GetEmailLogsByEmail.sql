-- =============================================
-- Stored Procedure: 100_GetEmailLogsByEmail
-- Fecha: 2026-01-15
-- Descripción: Obtiene todos los logs de correo electrónico para un email específico
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetEmailLogsByEmail]
    @email NVARCHAR(255)
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
    WHERE [RecipientEmail] = @email
    ORDER BY [AttemptedAt] DESC;
END;
GO
