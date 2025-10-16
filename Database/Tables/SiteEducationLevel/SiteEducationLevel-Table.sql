-- =============================================
-- Tabla: SiteEducationLevel
-- Descripción: Tabla para almacenar niveles educativos de los sitios
-- Reemplaza: SchoolEducationLevel
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteEducationLevel]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [EducationLevelId] [int] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteEducationLevel] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_SiteEducationLevel_SiteId_EducationLevelId] UNIQUE ([SiteId], [EducationLevelId])
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteEducationLevel_SiteId] ON [SiteEducationLevel]([SiteId]);
CREATE INDEX [IX_SiteEducationLevel_EducationLevelId] ON [SiteEducationLevel]([EducationLevelId]);
CREATE INDEX [IX_SiteEducationLevel_IsActive] ON [SiteEducationLevel]([IsActive]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteEducationLevel] ADD CONSTRAINT [FK_SiteEducationLevel_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);
ALTER TABLE [SiteEducationLevel] ADD CONSTRAINT [FK_SiteEducationLevel_EducationLevel] FOREIGN KEY([EducationLevelId]) REFERENCES [EducationLevel]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar niveles educativos de los sitios. Reemplaza SchoolEducationLevel.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteEducationLevel';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la relación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteEducationLevel', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteEducationLevel', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del nivel educativo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteEducationLevel', 
    @level2type = N'COLUMN', @level2name = N'EducationLevelId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la relación está activa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteEducationLevel', 
    @level2type = N'COLUMN', @level2name = N'IsActive';
