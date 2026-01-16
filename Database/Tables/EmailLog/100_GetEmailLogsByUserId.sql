-- =============================================
-- Stored Procedure: 100_GetEmailLogsByUserId
-- Fecha: 2026-01-15
-- Descripción: Obtiene todos los logs de correo electrónico de un usuario específico
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetEmailLogsByUserId]
    @userid NVARCHAR(450)
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
    WHERE [UserId] = @userid
    ORDER BY [AttemptedAt] DESC;
END;
GO
