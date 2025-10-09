-- =============================================
-- Tabla: SiteStaff
-- Descripción: Tabla para almacenar asignaciones de personal a sitios
-- Reemplaza: SchoolStaff
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteStaff]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [StaffId] [int] NOT NULL,
    [AssignmentDate] [date] NOT NULL DEFAULT GETDATE(),
    [AssignmentTypeId] [int] NOT NULL DEFAULT 1,
    [IsPrimary] [bit] NOT NULL DEFAULT 0,
    [StartDate] [date] NULL,
    [EndDate] [date] NULL,
    [Comments] [nvarchar](500) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteStaff] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteStaff_SiteId] ON [SiteStaff]([SiteId]);
CREATE INDEX [IX_SiteStaff_StaffId] ON [SiteStaff]([StaffId]);
CREATE INDEX [IX_SiteStaff_AssignmentTypeId] ON [SiteStaff]([AssignmentTypeId]);
CREATE INDEX [IX_SiteStaff_IsActive] ON [SiteStaff]([IsActive]);
CREATE INDEX [IX_SiteStaff_IsPrimary] ON [SiteStaff]([IsPrimary]);
CREATE INDEX [IX_SiteStaff_AssignmentDate] ON [SiteStaff]([AssignmentDate]);
CREATE INDEX [IX_SiteStaff_StartDate] ON [SiteStaff]([StartDate]);
CREATE INDEX [IX_SiteStaff_EndDate] ON [SiteStaff]([EndDate]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteStaff] ADD CONSTRAINT [FK_SiteStaff_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);
ALTER TABLE [SiteStaff] ADD CONSTRAINT [FK_SiteStaff_Staff] FOREIGN KEY([StaffId]) REFERENCES [Staff]([Id]);
ALTER TABLE [SiteStaff] ADD CONSTRAINT [FK_SiteStaff_AssignmentType] FOREIGN KEY([AssignmentTypeId]) REFERENCES [OptionSelection]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar asignaciones de personal a sitios. Reemplaza SchoolStaff.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del miembro del personal', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'StaffId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'AssignmentDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'AssignmentTypeId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si es la asignación principal', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'IsPrimary';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de inicio de la asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'StartDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de fin de la asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'EndDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Comentarios sobre la asignación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'Comments';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la asignación está activa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteStaff', 
    @level2type = N'COLUMN', @level2name = N'IsActive';
