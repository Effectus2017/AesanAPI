-- =============================================
-- Tabla: ProgramOperatingDays
-- Descripción: Define qué días de la semana puede operar cada programa
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[ProgramOperatingDays]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ProgramId] [int] NOT NULL,
    [DayOfWeek] [int] NOT NULL,
    -- 1=Lunes, 2=Martes, ..., 7=Domingo
    [IsAllowed] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_ProgramOperatingDays] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

-- Índice único para evitar duplicados de ProgramId + DayOfWeek
CREATE UNIQUE INDEX [UK_ProgramOperatingDays_ProgramId_DayOfWeek] ON [ProgramOperatingDays]([ProgramId], [DayOfWeek]);

-- Índices para optimizar consultas
CREATE INDEX [IX_ProgramOperatingDays_ProgramId] ON [ProgramOperatingDays]([ProgramId]);
CREATE INDEX [IX_ProgramOperatingDays_DayOfWeek] ON [ProgramOperatingDays]([DayOfWeek]);
CREATE INDEX [IX_ProgramOperatingDays_IsAllowed] ON [ProgramOperatingDays]([IsAllowed]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [ProgramOperatingDays] ADD CONSTRAINT [FK_ProgramOperatingDays_Program] FOREIGN KEY([ProgramId]) REFERENCES [Program]([Id]);

-- =============================================
-- Datos Iniciales
-- =============================================

-- PDAM (1): Lunes-Viernes (1-5) permitidos, Sábado-Domingo (6-7) no permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (1, 1, 1, GETDATE()),
    -- Lunes
    (1, 2, 1, GETDATE()),
    -- Martes
    (1, 3, 1, GETDATE()),
    -- Miércoles
    (1, 4, 1, GETDATE()),
    -- Jueves
    (1, 5, 1, GETDATE()),
    -- Viernes
    (1, 6, 0, GETDATE()),
    -- Sábado
    (1, 7, 0, GETDATE());
-- Domingo

-- PSAV (2): Lunes-Viernes (1-5) permitidos, Sábado-Domingo (6-7) no permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (2, 1, 1, GETDATE()),
    -- Lunes
    (2, 2, 1, GETDATE()),
    -- Martes
    (2, 3, 1, GETDATE()),
    -- Miércoles
    (2, 4, 1, GETDATE()),
    -- Jueves
    (2, 5, 1, GETDATE()),
    -- Viernes
    (2, 6, 0, GETDATE()),
    -- Sábado
    (2, 7, 0, GETDATE());
-- Domingo

-- PACNA (3): Todos los días (1-7) permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (3, 1, 1, GETDATE()),
    -- Lunes
    (3, 2, 1, GETDATE()),
    -- Martes
    (3, 3, 1, GETDATE()),
    -- Miércoles
    (3, 4, 1, GETDATE()),
    -- Jueves
    (3, 5, 1, GETDATE()),
    -- Viernes
    (3, 6, 1, GETDATE()),
    -- Sábado
    (3, 7, 1, GETDATE());
-- Domingo

-- PFHF (4): Todos los días (1-7) permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (4, 1, 1, GETDATE()),
    -- Lunes
    (4, 2, 1, GETDATE()),
    -- Martes
    (4, 3, 1, GETDATE()),
    -- Miércoles
    (4, 4, 1, GETDATE()),
    -- Jueves
    (4, 5, 1, GETDATE()),
    -- Viernes
    (4, 6, 1, GETDATE()),
    -- Sábado
    (4, 7, 1, GETDATE());
-- Domingo

-- PDFE (5): Todos los días (1-7) permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (5, 1, 1, GETDATE()),
    -- Lunes
    (5, 2, 1, GETDATE()),
    -- Martes
    (5, 3, 1, GETDATE()),
    -- Miércoles
    (5, 4, 1, GETDATE()),
    -- Jueves
    (5, 5, 1, GETDATE()),
    -- Viernes
    (5, 6, 1, GETDATE()),
    -- Sábado
    (5, 7, 1, GETDATE());
-- Domingo

-- AESAN (6): Todos los días (1-7) permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (6, 1, 1, GETDATE()),
    -- Lunes
    (6, 2, 1, GETDATE()),
    -- Martes
    (6, 3, 1, GETDATE()),
    -- Miércoles
    (6, 4, 1, GETDATE()),
    -- Jueves
    (6, 5, 1, GETDATE()),
    -- Viernes
    (6, 6, 1, GETDATE()),
    -- Sábado
    (6, 7, 1, GETDATE());
-- Domingo

-- PAF (7): Todos los días (1-7) permitidos
INSERT INTO [ProgramOperatingDays]
    ([ProgramId], [DayOfWeek], [IsAllowed], [CreatedAt])
VALUES
    (7, 1, 1, GETDATE()),
    -- Lunes
    (7, 2, 1, GETDATE()),
    -- Martes
    (7, 3, 1, GETDATE()),
    -- Miércoles
    (7, 4, 1, GETDATE()),
    -- Jueves
    (7, 5, 1, GETDATE()),
    -- Viernes
    (7, 6, 1, GETDATE()),
    -- Sábado
    (7, 7, 1, GETDATE());
-- Domingo

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla que define qué días de la semana puede operar cada programa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'ProgramOperatingDays';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del registro', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'ProgramOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del programa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'ProgramOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'ProgramId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Día de la semana: 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado, 7=Domingo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'ProgramOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'DayOfWeek';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si el día está permitido para el programa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'ProgramOperatingDays', 
    @level2type = N'COLUMN', @level2name = N'IsAllowed';

GO

