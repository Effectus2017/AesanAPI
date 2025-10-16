-- =============================================
-- Tabla: SiteOperatingDays
-- Descripción: Tabla para almacenar días de funcionamiento de los sitios
-- Reemplaza: SchoolOperatingDays
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteOperatingDays]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [OperatingDate] [date] NOT NULL,
    [StartTime] [time] NULL,
    [EndTime] [time] NULL,
    [Comment] [nvarchar](255) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteOperatingDays] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteOperatingDays_SiteId] ON [SiteOperatingDays]([SiteId]);
CREATE INDEX [IX_SiteOperatingDays_OperatingDate] ON [SiteOperatingDays]([OperatingDate]);
CREATE INDEX [IX_SiteOperatingDays_IsActive] ON [SiteOperatingDays]([IsActive]);
CREATE INDEX [IX_SiteOperatingDays_SiteId_OperatingDate] ON [SiteOperatingDays]([SiteId], [OperatingDate]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteOperatingDays] ADD CONSTRAINT [FK_SiteOperatingDays_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar días de funcionamiento de los sitios. Reemplaza SchoolOperatingDays.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del día de funcionamiento', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de funcionamiento', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'OperatingDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de inicio del funcionamiento', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'StartTime';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de fin del funcionamiento', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'EndTime';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Comentarios sobre el día de funcionamiento', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'Comment';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si el día de funcionamiento está activo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'IsActive';
