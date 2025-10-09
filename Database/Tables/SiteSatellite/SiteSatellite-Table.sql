-- =============================================
-- Tabla: SiteSatellite
-- Descripción: Tabla para almacenar relaciones entre sitios principales y satélites
-- Reemplaza: SchoolSatellite
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteSatellite]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [MainSiteId] [int] NOT NULL,
    [SatelliteSiteId] [int] NOT NULL,
    [AssignmentDate] [date] NULL,
    [Comment] [nvarchar](255) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteSatellite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_SiteSatellite] UNIQUE ([SatelliteSiteId])
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteSatellite_MainSiteId] ON [SiteSatellite]([MainSiteId]);
CREATE INDEX [IX_SiteSatellite_SatelliteSiteId] ON [SiteSatellite]([SatelliteSiteId]);
CREATE INDEX [IX_SiteSatellite_IsActive] ON [SiteSatellite]([IsActive]);
CREATE INDEX [IX_SiteSatellite_AssignmentDate] ON [SiteSatellite]([AssignmentDate]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteSatellite] ADD CONSTRAINT [FK_SiteSatellite_MainSite] FOREIGN KEY([MainSiteId]) REFERENCES [Site]([Id]);
ALTER TABLE [SiteSatellite] ADD CONSTRAINT [FK_SiteSatellite_SatelliteSite] FOREIGN KEY([SatelliteSiteId]) REFERENCES [Site]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar relaciones entre sitios principales y satélites. Reemplaza SchoolSatellite.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la relación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio principal', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite', 
    @level2type = N'COLUMN', @level2name = N'MainSiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio satélite', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite', 
    @level2type = N'COLUMN', @level2name = N'SatelliteSiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de asignación del sitio satélite', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite', 
    @level2type = N'COLUMN', @level2name = N'AssignmentDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Comentarios sobre la asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite', 
    @level2type = N'COLUMN', @level2name = N'Comment';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la relación está activa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteSatellite', 
    @level2type = N'COLUMN', @level2name = N'IsActive';
