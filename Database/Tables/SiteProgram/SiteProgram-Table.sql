-- =============================================
-- Tabla: SiteProgram
-- Descripción: Relación entre Sites y Programs con fechas variables
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteProgram]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [ProgramId] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [EndDate] [date] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteProgram] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

-- Índices para optimizar consultas
CREATE INDEX [IX_SiteProgram_SiteId] ON [SiteProgram]([SiteId]);
CREATE INDEX [IX_SiteProgram_ProgramId] ON [SiteProgram]([ProgramId]);
CREATE INDEX [IX_SiteProgram_IsActive] ON [SiteProgram]([IsActive]);
CREATE INDEX [IX_SiteProgram_CreatedAt] ON [SiteProgram]([CreatedAt]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteProgram] ADD CONSTRAINT [FK_SiteProgram_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);
ALTER TABLE [SiteProgram] ADD CONSTRAINT [FK_SiteProgram_Program] FOREIGN KEY([ProgramId]) REFERENCES [Program]([Id]);
GO

