-- =============================================
-- Tabla: SiteExcursionExcludedService
-- Descripción: Tabla para almacenar servicios excluidos por excursión
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteExcursionExcludedService]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteExcursionId] [int] NOT NULL,
    [ServiceTypeId] [int] NOT NULL,  -- FK a OptionSelection(OptionKey='service-type')
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SiteExcursionExcludedService] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteExcursionExcludedService_SiteExcursionId] ON [SiteExcursionExcludedService]([SiteExcursionId]);
CREATE INDEX [IX_SiteExcursionExcludedService_ServiceTypeId] ON [SiteExcursionExcludedService]([ServiceTypeId]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteExcursionExcludedService] ADD CONSTRAINT [FK_SiteExcursionExcludedService_SiteExcursion] 
    FOREIGN KEY([SiteExcursionId]) REFERENCES [SiteExcursion]([Id]) ON DELETE CASCADE;

ALTER TABLE [SiteExcursionExcludedService] ADD CONSTRAINT [FK_SiteExcursionExcludedService_ServiceType] 
    FOREIGN KEY([ServiceTypeId]) REFERENCES [OptionSelection]([Id]) ON DELETE NO ACTION;

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar servicios excluidos por excursión', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursionExcludedService';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del servicio excluido', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursionExcludedService', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID de la excursión', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursionExcludedService', 
    @level2type = N'COLUMN', @level2name = N'SiteExcursionId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de servicio excluido (FK a OptionSelection)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteExcursionExcludedService', 
    @level2type = N'COLUMN', @level2name = N'ServiceTypeId';

