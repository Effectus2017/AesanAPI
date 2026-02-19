-- =============================================
-- Stored Procedure: 100_InsertLogApplication
-- Convención: parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertLogApplication]
    @category NVARCHAR(50) = 'Application',
    @level NVARCHAR(20) = NULL,
    @message NVARCHAR(MAX),
    @payload NVARCHAR(MAX) = NULL,
    @userid NVARCHAR(450) = NULL,
    @status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @newId BIGINT;

    INSERT INTO [dbo].[LogApplication]
    (
        [Category],
        [Level],
        [Message],
        [Payload],
        [UserId],
        [Status]
    )
    VALUES
    (
        @category,
        @level,
        @message,
        @payload,
        @userid,
        @status
    );

    SET @newId = SCOPE_IDENTITY();

    SELECT id = @newId;
END;
GO
