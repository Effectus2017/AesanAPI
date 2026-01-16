-- =============================================
-- Stored Procedure: 100_GetEmailLogById
-- Fecha: 2026-01-15
-- Descripción: Obtiene un log de correo electrónico específico por su ID
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetEmailLogById]
    @id INT
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
    WHERE [Id] = @id;
END;
GO
