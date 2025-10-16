-- =============================================
-- Tabla: SiteFacility
-- Descripción: Tabla para almacenar instalaciones de los sitios
-- Reemplaza: SchoolFacility
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteFacility]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [FacilityTypeId] [int] NOT NULL,
    [Description] [nvarchar](255) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteFacility] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteFacility_SiteId] ON [SiteFacility]([SiteId]);
CREATE INDEX [IX_SiteFacility_FacilityTypeId] ON [SiteFacility]([FacilityTypeId]);
CREATE INDEX [IX_SiteFacility_IsActive] ON [SiteFacility]([IsActive]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteFacility] ADD CONSTRAINT [FK_SiteFacility_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);
ALTER TABLE [SiteFacility] ADD CONSTRAINT [FK_SiteFacility_FacilityType] FOREIGN KEY([FacilityTypeId]) REFERENCES [OptionSelection]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar instalaciones de los sitios. Reemplaza SchoolFacility.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteFacility';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la instalación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteFacility', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteFacility', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de instalación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteFacility', 
    @level2type = N'COLUMN', @level2name = N'FacilityTypeId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Descripción de la instalación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteFacility', 
    @level2type = N'COLUMN', @level2name = N'Description';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la instalación está activa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteFacility', 
    @level2type = N'COLUMN', @level2name = N'IsActive';
