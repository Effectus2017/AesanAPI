/*
===========================================
Tabla: EmailLog (Registro de Envíos de Correos Electrónicos)
===========================================
Sistema de auditoría para todos los envíos de correos electrónicos del sistema AESAN.
Permite rastrear cada intento de envío, consultar historial y reenviar correos cuando sea necesario.

Versión: 1.0
Fecha: 2026-01-15

Propósito:
- Registrar cada intento de envío de correo (exitoso o fallido)
- Consultar el historial de envíos por usuario/email
- Permitir reenvío de correos desde el frontend cuando no llegaron
- Auditoría completa de comunicaciones por correo

Características:
- Registro de estado: Pending, Sent, Failed
- Información completa del correo enviado
- Relación con usuarios y agencias
- Soporte para reintentos y reenvíos
- Trazabilidad completa con timestamps

Estructura:
- Id: Identificador único
- RecipientEmail: Email del destinatario
- Subject: Asunto del correo
- EmailType: Tipo de correo (WelcomeAgency, ApprovalSponsor, etc.)
- Status: Estado del envío (Pending, Sent, Failed)
- ErrorMessage: Mensaje de error si falló
- SentAt: Fecha/hora de envío exitoso
- AttemptedAt: Fecha/hora del intento
- UserId: ID del usuario relacionado (nullable)
- AgencyId: ID de la agencia relacionada (nullable)
- EmailTemplateKey: Clave del template usado (nullable)
- RetryCount: Número de reintentos
- OriginalEmailLogId: ID del log original si es un reenvío (nullable)
- CreatedAt/CreatedBy: Auditoría

Relaciones:
- UserId → AspNetUsers (nullable)
- AgencyId → Agency (nullable)
- OriginalEmailLogId → EmailLog (self-reference, nullable)
- EmailTemplateKey → EmailTemplate.TemplateKey (nullable)

Ejemplos de uso:
- Registrar envío de correo de bienvenida
- Consultar historial de correos de un usuario
- Reenviar correo que no llegó
- Identificar correos fallidos para análisis
*/

CREATE TABLE [dbo].[EmailLog]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [RecipientEmail] NVARCHAR(255) NOT NULL,
    [Subject] NVARCHAR(500) NOT NULL,
    [EmailType] NVARCHAR(100) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [SentAt] DATETIME NULL,
    [AttemptedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UserId] NVARCHAR(450) NULL,
    [AgencyId] INT NULL,
    [EmailTemplateKey] NVARCHAR(100) NULL,
    [RetryCount] INT NOT NULL DEFAULT 0,
    [OriginalEmailLogId] INT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(450) NULL,
    
    -- Foreign key constraints
    CONSTRAINT [FK_EmailLog_User] FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_EmailLog_Agency] FOREIGN KEY ([AgencyId]) 
        REFERENCES [dbo].[Agency]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_EmailLog_OriginalEmailLog] FOREIGN KEY ([OriginalEmailLogId]) 
        REFERENCES [dbo].[EmailLog]([Id]) ON DELETE NO ACTION
);

-- Índices para optimización
CREATE NONCLUSTERED INDEX [IX_EmailLog_RecipientEmail] ON [dbo].[EmailLog]([RecipientEmail]);
CREATE NONCLUSTERED INDEX [IX_EmailLog_UserId] ON [dbo].[EmailLog]([UserId]);
CREATE NONCLUSTERED INDEX [IX_EmailLog_Status] ON [dbo].[EmailLog]([Status]);
CREATE NONCLUSTERED INDEX [IX_EmailLog_EmailType] ON [dbo].[EmailLog]([EmailType]);
CREATE NONCLUSTERED INDEX [IX_EmailLog_AttemptedAt] ON [dbo].[EmailLog]([AttemptedAt] DESC);
CREATE NONCLUSTERED INDEX [IX_EmailLog_AgencyId] ON [dbo].[EmailLog]([AgencyId]);
CREATE NONCLUSTERED INDEX [IX_EmailLog_OriginalEmailLogId] ON [dbo].[EmailLog]([OriginalEmailLogId]);
GO
