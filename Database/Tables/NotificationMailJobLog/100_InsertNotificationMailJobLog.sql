-- =============================================
-- Stored Procedure: 100_InsertNotificationMailJobLog
-- Descripción: Inserta un registro de ejecución del job de notificaciones mail
-- Convención: parámetros y alias de salida lowercase; columnas en cuerpo CapitalCase
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertNotificationMailJobLog]
    @jobname NVARCHAR(128),
    @startedat DATETIME2(7),
    @finishedat DATETIME2(7) = NULL,
    @status NVARCHAR(50),
    @message NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @newId INT;

    INSERT INTO [dbo].[NotificationMailJobLog]
    (
        [JobName],
        [StartedAt],
        [FinishedAt],
        [Status],
        [Message]
    )
    VALUES
    (
        @jobname,
        @startedat,
        @finishedat,
        @status,
        @message
    );

    SET @newId = SCOPE_IDENTITY();

    SELECT id = @newId;
END;
GO
