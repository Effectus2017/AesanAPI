-- =============================================
-- Tabla: ServiceType
-- Descripción: Catálogo de tipos de servicio de alimentación
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

/*
Tabla dedicada para tipos de servicio de alimentación.

Ventajas:
- IDs fijos e inmutables
- No depende de OptionSelection
- Específica del dominio de servicios
- Fácil de mantener y extender

Características:
- IDs predefinidos 1-10 (no auto-incrementales para estos registros base)
- Simples, naturales y fáciles de recordar
- Nombres en español e inglés
- Orden de visualización
- Auditoría completa
*/

IF NOT EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[ServiceType]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ServiceType]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        -- IDENTITY para auto-incremento, pero usaremos SET IDENTITY_INSERT para los primeros 10 (IDs fijos 1-10)
        [Name] [nvarchar](100) NOT NULL,
        -- Nombre en español
        [NameEN] [nvarchar](100) NOT NULL,
        -- Nombre en inglés
        [Code] [nvarchar](50) NOT NULL,
        -- Código único e inmutable (ej: BREAKFAST, LUNCH)
        [Description] [nvarchar](500) NULL,
        -- Descripción opcional
        [DisplayOrder] [int] NOT NULL DEFAULT 0,
        -- Orden de visualización
        [IsActive] [bit] NOT NULL DEFAULT 1,
        -- Estado activo/inactivo
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Índice único para el código
    CREATE UNIQUE INDEX [UX_ServiceType_Code] ON [dbo].[ServiceType]([Code]);

    PRINT 'Tabla ServiceType creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla ServiceType ya existe';
END
GO

