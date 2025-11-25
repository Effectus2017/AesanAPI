/*
===========================================
Tabla: MessageTemplate (Templates de Mensajes Internos)
===========================================
Sistema de gestión de templates de mensajes internos para el sistema AESAN.
Permite almacenar, editar y gestionar los mensajes desde la base de datos.

Versión: 1.1
Fecha: 2025-01-XX

Propósito:
- Almacenar templates de mensajes internos del sistema
- Soporte bilingüe (español/inglés)
- Gestión centralizada de contenido de mensajes
- Edición de templates sin modificar código

Características:
- TemplateKey único para identificar cada tipo de mensaje
- Title y Body separados por idioma (ES/EN) para el contenido del mensaje
- Purpose separado por idioma (ES/EN) para identificar el propósito del template
- Icon, Image, Link para personalización visual
- Control de estado activo/inactivo (IsActive)
- Marcas de tiempo para auditoría (CreatedAt, UpdatedAt)
- Campos de auditoría de usuario (CreatedBy, UpdatedBy)

Estructura de Campos:
- Id: Identificador único del template
- TemplateKey: Clave única del template (ej: 'SponsorRegistrationCompleted', 'SponsorApproved')
- TitleES/TitleEN: Título del mensaje que se muestra al usuario en español e inglés
- BodyES/BodyEN: Contenido del mensaje que se muestra al usuario en español e inglés (el cuerpo del mensaje)
- PurposeES/PurposeEN: Propósito del template en español e inglés (descripción para identificar qué hace este template, solo para administración)
- Icon: Nombre del icono Material a mostrar con el mensaje (opcional)
- Image: URL de la imagen a mostrar con el mensaje (opcional)
- Link: Enlace asociado al mensaje (opcional)
- UseRouter: Indica si el link usa router de Angular (true) o es una URL externa (false)
- IsActive: Estado activo/inactivo del template
- CreatedAt/UpdatedAt: Auditoría temporal (fecha de creación y última actualización)
- CreatedBy/UpdatedBy: Auditoría de usuario (ID del usuario que creó/actualizó)

Relaciones:
- Referenciada por MessageTemplateService para obtener templates
- Usada junto con EmailTemplate para notificaciones completas

Ejemplos de uso:
- SponsorRegistrationCompleted: Mensaje cuando auspiciador completa registro
- SponsorApproved: Mensaje cuando se aprueba un auspiciador
- SponsorRejected: Mensaje cuando se rechaza un auspiciador
*/

CREATE TABLE [dbo].[MessageTemplate]
(
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TemplateKey] NVARCHAR(100) NOT NULL UNIQUE,
    [TitleES] NVARCHAR(500) NOT NULL,
    -- Título del mensaje en español (lo que ve el usuario)
    [TitleEN] NVARCHAR(500) NOT NULL,
    -- Título del mensaje en inglés (lo que ve el usuario)
    [BodyES] NVARCHAR(MAX) NOT NULL,
    -- Contenido del mensaje en español (lo que ve el usuario)
    [BodyEN] NVARCHAR(MAX) NOT NULL,
    -- Contenido del mensaje en inglés (lo que ve el usuario)
    [Icon] NVARCHAR(255) NULL,
    -- Nombre del icono Material (ej: 'heroicons_outline:check-circle')
    [Image] NVARCHAR(500) NULL,
    -- URL de la imagen a mostrar con el mensaje
    [Link] NVARCHAR(500) NULL,
    -- Enlace asociado al mensaje (puede ser ruta Angular o URL externa)
    [UseRouter] BIT NOT NULL DEFAULT 0,
    -- Si es true, el Link usa router de Angular; si es false, es URL externa
    [PurposeES] NVARCHAR(500) NULL,
    -- Propósito del template en español (solo para administración, identifica qué hace este template)
    [PurposeEN] NVARCHAR(500) NULL,
    -- Propósito del template en inglés (solo para administración, identifica qué hace este template)
    [IsActive] BIT NOT NULL DEFAULT 1,
    -- Estado activo/inactivo del template
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha de creación del template
    [UpdatedAt] DATETIME NULL,
    -- Fecha de última actualización del template
    [CreatedBy] NVARCHAR(450) NULL,
    -- ID del usuario que creó el template
    [UpdatedBy] NVARCHAR(450) NULL
    -- ID del usuario que actualizó el template por última vez
);

-- Índices para optimización
CREATE INDEX IX_MessageTemplate_TemplateKey ON [dbo].[MessageTemplate]([TemplateKey]);
CREATE INDEX IX_MessageTemplate_IsActive ON [dbo].[MessageTemplate]([IsActive]);
GO

-- agregar columna PurposeEN
ALTER TABLE [dbo].[MessageTemplate] ADD [PurposeEN] NVARCHAR(500) NULL;