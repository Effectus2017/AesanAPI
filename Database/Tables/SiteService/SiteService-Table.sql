-- =============================================
-- Tabla: SiteService
-- Descripción: Tabla para almacenar servicios de alimentación de los sitios
-- Reemplaza: SchoolService
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteService]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [ChildGroupId] [int] NOT NULL,
    [Breakfast] [bit] NULL,
    [BreakfastFrom] [time] NULL,
    [BreakfastTo] [time] NULL,
    [Lunch] [bit] NULL,
    [LunchFrom] [time] NULL,
    [LunchTo] [time] NULL,
    [SnackAM] [bit] NULL,
    [SnackAMFrom] [time] NULL,
    [SnackAMTo] [time] NULL,
    [Dinner] [bit] NULL,
    [DinnerFrom] [time] NULL,
    [DinnerTo] [time] NULL,
    [SnackPM] [bit] NULL,
    [SnackPMFrom] [time] NULL,
    [SnackPMTo] [time] NULL,
    [SnackNight] [bit] NULL,
    [SnackNightFrom] [time] NULL,
    [SnackNightTo] [time] NULL,
    [DinnerExtended] [bit] NULL,
    [DinnerExtendedFrom] [time] NULL,
    [DinnerExtendedTo] [time] NULL,
    [DinnerAtRisk] [bit] NULL,
    [DinnerAtRiskFrom] [time] NULL,
    [DinnerAtRiskTo] [time] NULL,
    [SnackExtended] [bit] NULL,
    [SnackExtendedFrom] [time] NULL,
    [SnackExtendedTo] [time] NULL,
    [SnackAtRisk] [bit] NULL,
    [SnackAtRiskFrom] [time] NULL,
    [SnackAtRiskTo] [time] NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteService] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteService_SiteId] ON [SiteService]([SiteId]);
CREATE INDEX [IX_SiteService_ChildGroupId] ON [SiteService]([ChildGroupId]);
CREATE INDEX [IX_SiteService_SiteId_ChildGroupId] ON [SiteService]([SiteId], [ChildGroupId]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteService] ADD CONSTRAINT [FK_SiteService_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]) ON DELETE CASCADE;
ALTER TABLE [SiteService] ADD CONSTRAINT [FK_SiteService_ChildGroup] FOREIGN KEY([ChildGroupId]) REFERENCES [SiteChildGroup]([Id]) ON DELETE CASCADE;

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar servicios de alimentación de los sitios. Reemplaza SchoolService.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del servicio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del grupo de niños', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'ChildGroupId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si se ofrece desayuno', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'Breakfast';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de inicio del desayuno', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'BreakfastFrom';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de fin del desayuno', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'BreakfastTo';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si se ofrece almuerzo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'Lunch';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de inicio del almuerzo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'LunchFrom';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de fin del almuerzo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'LunchTo';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si se ofrece merienda matutina', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'SnackAM';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si se ofrece cena', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'Dinner';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si se ofrece merienda vespertina', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'SnackPM';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si se ofrece merienda nocturna', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteService', 
    @level2type = N'COLUMN', @level2name = N'SnackNight';
