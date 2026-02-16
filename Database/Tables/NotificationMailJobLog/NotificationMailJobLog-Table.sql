/*
===========================================
Tabla: NotificationMailJobLog (Log de ejecución del job de notificaciones mail)
===========================================
Registro de cada ejecución del servicio de notificaciones mail (Web Job).
Permite auditar cuándo se ejecutó el job y su estado (Running, Success, Failed).

Versión: 1.0

Propósito:
- Registrar cada inicio (y en el futuro fin) de ejecución del job ProcessAgencyNotifications
- Consultar historial de ejecuciones para monitoreo y diagnóstico
- Preparado para ampliar con FinishedAt y Status Success/Failed

Estructura:
- Id: Identificador único
- JobName: Nombre del job (ej. ProcessAgencyNotifications)
- StartedAt: Fecha/hora de inicio (UTC)
- FinishedAt: Fecha/hora de fin (nullable, uso futuro)
- Status: Running, Success, Failed
- Message: Detalle opcional
- CreatedAt: Auditoría
*/

CREATE TABLE [dbo].[NotificationMailJobLog]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [JobName] NVARCHAR(128) NOT NULL,
    [StartedAt] DATETIME2(7) NOT NULL,
    [FinishedAt] DATETIME2(7) NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [Message] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

CREATE NONCLUSTERED INDEX [IX_NotificationMailJobLog_StartedAt] ON [dbo].[NotificationMailJobLog]([StartedAt] DESC);
CREATE NONCLUSTERED INDEX [IX_NotificationMailJobLog_JobName] ON [dbo].[NotificationMailJobLog]([JobName]);
CREATE NONCLUSTERED INDEX [IX_NotificationMailJobLog_Status] ON [dbo].[NotificationMailJobLog]([Status]);
GO
