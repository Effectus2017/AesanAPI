-- =============================================
-- Tabla: SiteParticipant
-- Descripción: Tabla para almacenar tipos de participantes de los sitios
-- Reemplaza: SchoolParticipant
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteParticipant]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [ParticipantTypeId] [int] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteParticipant] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteParticipant_SiteId] ON [SiteParticipant]([SiteId]);
CREATE INDEX [IX_SiteParticipant_ParticipantTypeId] ON [SiteParticipant]([ParticipantTypeId]);
CREATE INDEX [IX_SiteParticipant_IsActive] ON [SiteParticipant]([IsActive]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteParticipant] ADD CONSTRAINT [FK_SiteParticipant_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);
ALTER TABLE [SiteParticipant] ADD CONSTRAINT [FK_SiteParticipant_ParticipantType] FOREIGN KEY([ParticipantTypeId]) REFERENCES [OptionSelection]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar tipos de participantes de los sitios. Reemplaza SchoolParticipant.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteParticipant';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la relación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteParticipant', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteParticipant', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de participante', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteParticipant', 
    @level2type = N'COLUMN', @level2name = N'ParticipantTypeId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la relación está activa', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteParticipant', 
    @level2type = N'COLUMN', @level2name = N'IsActive';
