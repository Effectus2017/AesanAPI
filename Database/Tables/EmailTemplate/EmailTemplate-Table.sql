/*
===========================================
Tabla: EmailTemplate (Templates de Correo Electrónico)
===========================================
Sistema de gestión de templates de correo electrónico para el sistema AESAN.
Permite almacenar, editar y gestionar los correos electrónicos desde la base de datos.

Versión: 1.0
Fecha: 2025-01-15

Propósito:
- Almacenar templates de correo electrónico en formato HTML
- Soporte bilingüe (español/inglés)
- Gestión centralizada de contenido de emails
- Edición de templates sin modificar código

Características:
- TemplateKey único para identificar cada tipo de email
- Subject y Body separados por idioma (ES/EN)
- Descripción para identificar el propósito del template
- Control de estado activo/inactivo (IsActive)
- Marcas de tiempo para auditoría (CreatedAt, UpdatedAt)
- Campos de auditoría de usuario (CreatedBy, UpdatedBy)

Estructura:
- Id: Identificador único
- TemplateKey: Clave única del template (ej: 'WelcomeAgency', 'PasswordReset')
- SubjectES/SubjectEN: Asunto del correo en español e inglés
- BodyES/BodyEN: Cuerpo HTML del correo en español e inglés
- Description: Descripción del propósito del template
- IsActive: Estado activo/inactivo
- CreatedAt/UpdatedAt: Auditoría temporal
- CreatedBy/UpdatedBy: Auditoría de usuario

Relaciones:
- Referenciada por EmailService para obtener templates
- Usada en componentes de administración para edición

Ejemplos de uso:
- WelcomeAgency: Email de bienvenida a agencias
- PasswordReset: Email de restablecimiento de contraseña
- AgencyAssignment: Notificación de asignación de agencia
*/

CREATE TABLE [dbo].[EmailTemplate]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TemplateKey] NVARCHAR(100) NOT NULL UNIQUE,
    [SubjectES] NVARCHAR(500) NOT NULL,
    [SubjectEN] NVARCHAR(500) NOT NULL,
    [BodyES] NVARCHAR(MAX) NOT NULL,
    [BodyEN] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL,
    [CreatedBy] NVARCHAR(450) NULL,
    [UpdatedBy] NVARCHAR(450) NULL
);

-- Índices para optimización
CREATE INDEX IX_EmailTemplate_TemplateKey ON [dbo].[EmailTemplate]([TemplateKey]);
CREATE INDEX IX_EmailTemplate_IsActive ON [dbo].[EmailTemplate]([IsActive]);
GO

