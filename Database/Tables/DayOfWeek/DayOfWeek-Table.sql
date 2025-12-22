-- =============================================
-- Tabla: DayOfWeek
-- Descripción: Tabla de referencia para los días de la semana con sus nombres en español e inglés
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[DayOfWeek]
(
    [Id] [int] NOT NULL,
    -- 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado, 7=Domingo
    [Name] [nvarchar](50) NOT NULL,
    -- Nombre en español
    [NameEN] [nvarchar](50) NOT NULL,
    -- Nombre en inglés
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_DayOfWeek] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Datos Iniciales
-- =============================================

INSERT INTO [dbo].[DayOfWeek] ([Id], [Name], [NameEN], [CreatedAt])
VALUES
    (1, N'Lunes', N'Monday', GETDATE()),
    (2, N'Martes', N'Tuesday', GETDATE()),
    (3, N'Miércoles', N'Wednesday', GETDATE()),
    (4, N'Jueves', N'Thursday', GETDATE()),
    (5, N'Viernes', N'Friday', GETDATE()),
    (6, N'Sábado', N'Saturday', GETDATE()),
    (7, N'Domingo', N'Sunday', GETDATE());

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla de referencia para los días de la semana con sus nombres en español e inglés', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'DayOfWeek';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador del día: 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado, 7=Domingo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'DayOfWeek', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Nombre del día en español', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'DayOfWeek', 
    @level2type = N'COLUMN', @level2name = N'Name';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Nombre del día en inglés', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'DayOfWeek', 
    @level2type = N'COLUMN', @level2name = N'NameEN';

GO

