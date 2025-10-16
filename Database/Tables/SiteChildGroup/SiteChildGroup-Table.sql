-- =============================================
-- Tabla: SiteChildGroup
-- Descripción: Tabla para almacenar grupos de niños específicos de los sitios
-- Reemplaza: SchoolChildGroup
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteChildGroup]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [GroupName] [nvarchar](255) NOT NULL,
    [NumberOfChildren] [int] NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteChildGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteChildGroup_SiteId] ON [SiteChildGroup]([SiteId]);
CREATE INDEX [IX_SiteChildGroup_GroupName] ON [SiteChildGroup]([GroupName]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteChildGroup] ADD CONSTRAINT [FK_SiteChildGroup_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar grupos de niños específicos de los sitios. Reemplaza SchoolChildGroup.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteChildGroup';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del grupo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteChildGroup', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteChildGroup', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Nombre del grupo de niños', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteChildGroup', 
    @level2type = N'COLUMN', @level2name = N'GroupName';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Número de niños en el grupo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteChildGroup', 
    @level2type = N'COLUMN', @level2name = N'NumberOfChildren';
