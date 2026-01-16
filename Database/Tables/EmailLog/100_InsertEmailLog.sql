-- =============================================
-- Stored Procedure: 100_InsertEmailLog
-- Fecha: 2026-01-15
-- Descripción: Inserta un nuevo registro de envío de correo electrónico
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertEmailLog]
    @recipientemail NVARCHAR(255),
    @subject NVARCHAR(500),
    @emailtype NVARCHAR(100),
    @status NVARCHAR(50) = 'Pending',
    @errormessage NVARCHAR(MAX) = NULL,
    @sentat DATETIME = NULL,
    @attemptedat DATETIME = NULL,
    @userid NVARCHAR(450) = NULL,
    @agencyid INT = NULL,
    @emailtemplatekey NVARCHAR(100) = NULL,
    @retrycount INT = 0,
    @originalemaillogid INT = NULL,
    @createdby NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @newId INT;
    
    -- Si AttemptedAt no se proporciona, usar fecha actual
    IF @attemptedat IS NULL
        SET @attemptedat = GETDATE();
    
    INSERT INTO [dbo].[EmailLog]
    (
        [RecipientEmail],
        [Subject],
        [EmailType],
        [Status],
        [ErrorMessage],
        [SentAt],
        [AttemptedAt],
        [UserId],
        [AgencyId],
        [EmailTemplateKey],
        [RetryCount],
        [OriginalEmailLogId],
        [CreatedBy]
    )
    VALUES
    (
        @recipientemail,
        @subject,
        @emailtype,
        @status,
        @errormessage,
        @sentat,
        @attemptedat,
        @userid,
        @agencyid,
        @emailtemplatekey,
        @retrycount,
        @originalemaillogid,
        @createdby
    );
    
    SET @newId = SCOPE_IDENTITY();
    
    -- Retornar el ID del registro insertado
    SELECT id = @newId;
END;
GO
