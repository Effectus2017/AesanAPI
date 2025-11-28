-- =============================================
-- Tabla: SitePersonInCharge
-- Descripción: Tabla para almacenar información de la Persona a Cargo del sitio
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SitePersonInCharge]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [FirstName] [nvarchar](100) NULL,
    [MiddleName] [nvarchar](100) NULL,
    [FatherLastName] [nvarchar](100) NULL,
    [MotherLastName] [nvarchar](100) NULL,
    [SitePhone] [nvarchar](20) NULL,
    [Extension] [nvarchar](10) NULL,
    [MobilePhone] [nvarchar](20) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SitePersonInCharge] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_SitePersonInCharge_SiteId] UNIQUE ([SiteId])
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SitePersonInCharge_SiteId] ON [SitePersonInCharge]([SiteId]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SitePersonInCharge] ADD CONSTRAINT [FK_SitePersonInCharge_Site] 
    FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]) ON DELETE CASCADE;

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar información de la Persona a Cargo del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del registro', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Nombre de la persona a cargo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'FirstName';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Segundo nombre de la persona a cargo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'MiddleName';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Primer apellido de la persona a cargo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'FatherLastName';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Segundo apellido de la persona a cargo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'MotherLastName';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Teléfono del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'SitePhone';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Extensión telefónica', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'Extension';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Teléfono móvil', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SitePersonInCharge', 
    @level2type = N'COLUMN', @level2name = N'MobilePhone';

