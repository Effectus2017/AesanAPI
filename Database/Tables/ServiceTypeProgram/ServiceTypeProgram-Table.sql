-- =============================================
-- Tabla: ServiceTypeProgram
-- Descripción: Relación Program–ServiceType con IsStrongService y MinimumMinutesToNextService
-- Fecha: 2026-01-27
-- Versión: 1.0
-- AESAN-257
-- =============================================

CREATE TABLE [dbo].[ServiceTypeProgram]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ServiceTypeId] [int] NOT NULL,
    [ProgramId] [int] NOT NULL,
    [IsStrongService] [bit] NOT NULL DEFAULT 0,
    [MinimumMinutesToNextService] [int] NULL,
    [DisplayOrder] [int] NOT NULL DEFAULT 0,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_ServiceTypeProgram] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ServiceTypeProgram_ServiceType] FOREIGN KEY ([ServiceTypeId]) REFERENCES [dbo].[ServiceType]([Id]),
    CONSTRAINT [FK_ServiceTypeProgram_Program] FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[Program]([Id]),
    CONSTRAINT [UX_ServiceTypeProgram_ProgramId_ServiceTypeId] UNIQUE ([ProgramId], [ServiceTypeId])
);

CREATE INDEX [IX_ServiceTypeProgram_ProgramId] ON [dbo].[ServiceTypeProgram]([ProgramId]);
CREATE INDEX [IX_ServiceTypeProgram_ServiceTypeId] ON [dbo].[ServiceTypeProgram]([ServiceTypeId]);

GO

EXEC sys.sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Relación Program–ServiceType. IsStrongService: servicio fuerte (ej. almuerzo PDAM; almuerzo/cena PACNA/PSAV). MinimumMinutesToNextService: minutos mínimos después de que termine este servicio antes del siguiente.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'ServiceTypeProgram';
GO
