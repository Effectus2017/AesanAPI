-- =============================================
-- Tabla: SiteOperatingDaysOfWeek
-- Descripción: Tabla para almacenar los días de la semana seleccionados para operar en los sitios
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteOperatingDaysOfWeek]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [DayOfWeek] [int] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteOperatingDaysOfWeek] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_SiteOperatingDaysOfWeek_SiteId_DayOfWeek] UNIQUE ([SiteId], [DayOfWeek])
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteOperatingDaysOfWeek_SiteId] ON [SiteOperatingDaysOfWeek]([SiteId]);
CREATE INDEX [IX_SiteOperatingDaysOfWeek_DayOfWeek] ON [SiteOperatingDaysOfWeek]([DayOfWeek]);
CREATE INDEX [IX_SiteOperatingDaysOfWeek_IsActive] ON [SiteOperatingDaysOfWeek]([IsActive]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteOperatingDaysOfWeek] ADD CONSTRAINT [FK_SiteOperatingDaysOfWeek_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar los días de la semana seleccionados para operar en los sitios (1=Lunes, 2=Martes, ..., 7=Domingo)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDaysOfWeek';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la relación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDaysOfWeek', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDaysOfWeek', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Día de la semana (1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado, 7=Domingo)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDaysOfWeek', 
    @level2type = N'COLUMN', @level2name = N'DayOfWeek';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la relación está activa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDaysOfWeek', 
    @level2type = N'COLUMN', @level2name = N'IsActive';

