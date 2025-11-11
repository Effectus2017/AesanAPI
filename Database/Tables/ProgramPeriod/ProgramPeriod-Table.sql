-- =============================================
-- Tabla: ProgramPeriod
-- Descripción: Almacena fechas de inicio y fin por programa y año
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[ProgramPeriod]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ProgramId] [int] NOT NULL,
    [Year] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [EndDate] [date] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_ProgramPeriod] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

-- Índice único para ProgramId + Year (un programa solo puede tener un período por año)
CREATE UNIQUE INDEX [UK_ProgramPeriod_ProgramId_Year] ON [ProgramPeriod]([ProgramId], [Year]);

-- Índices para optimizar consultas
CREATE INDEX [IX_ProgramPeriod_ProgramId] ON [ProgramPeriod]([ProgramId]);
CREATE INDEX [IX_ProgramPeriod_Year] ON [ProgramPeriod]([Year]);
CREATE INDEX [IX_ProgramPeriod_IsActive] ON [ProgramPeriod]([IsActive]);
CREATE INDEX [IX_ProgramPeriod_CreatedAt] ON [ProgramPeriod]([CreatedAt]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [ProgramPeriod] ADD CONSTRAINT [FK_ProgramPeriod_Program] FOREIGN KEY([ProgramId]) REFERENCES [Program]([Id]);
GO

