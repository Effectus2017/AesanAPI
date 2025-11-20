-- =============================================
-- Tabla: SiteExcursion
-- Descripción: Tabla para almacenar información de excursiones de sitios
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteExcursion]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [ChildGroupId] [int] NULL,
    -- NULL si es general, FK a SiteChildGroup si es por grupo
    [ActivityDescription] [nvarchar](500) NOT NULL,
    -- "Visita al Parque Luis Muñoz Marín-SJ", "Excursión1", etc.
    [ExcursionDate] [date] NOT NULL,
    [IsFullDay] [bit] NOT NULL DEFAULT 0,
    -- Día Completo
    [IsUnforeseen] [bit] NOT NULL DEFAULT 0,
    -- Imprevisto
    [Comment] [nvarchar](1000) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteExcursion] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteExcursion_SiteId] ON [SiteExcursion]([SiteId]);
CREATE INDEX [IX_SiteExcursion_ChildGroupId] ON [SiteExcursion]([ChildGroupId]) WHERE [ChildGroupId] IS NOT NULL;
CREATE INDEX [IX_SiteExcursion_ExcursionDate] ON [SiteExcursion]([ExcursionDate]);
CREATE INDEX [IX_SiteExcursion_SiteId_ExcursionDate] ON [SiteExcursion]([SiteId], [ExcursionDate]);
CREATE INDEX [IX_SiteExcursion_IsActive] ON [SiteExcursion]([IsActive]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteExcursion] ADD CONSTRAINT [FK_SiteExcursion_Site] 
    FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]) ON DELETE CASCADE;

ALTER TABLE [SiteExcursion] ADD CONSTRAINT [FK_SiteExcursion_ChildGroup] 
    FOREIGN KEY([ChildGroupId]) REFERENCES [SiteChildGroup]([Id]) ON DELETE SET NULL;

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar información de excursiones de sitios', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la excursión', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del grupo de niños (NULL si es general)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'ChildGroupId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Descripción de la actividad de la excursión', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'ActivityDescription';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de la excursión', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'ExcursionDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si es día completo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'IsFullDay';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si es un imprevisto', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'IsUnforeseen';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Comentarios adicionales', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursion', 
    @level2type = N'COLUMN', @level2name = N'Comment';

