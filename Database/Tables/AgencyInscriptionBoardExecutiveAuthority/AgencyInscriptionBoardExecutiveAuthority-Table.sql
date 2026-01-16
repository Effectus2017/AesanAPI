-- =============================================
-- Tabla: AgencyInscriptionBoardExecutiveAuthority
-- Descripción: Tabla para almacenar las funciones de autoridad de la Junta de Directores hacia el Director Ejecutivo
-- Migración desde: AgencyInscription.BoardExecutiveAuthority (JSON) a tabla de relación
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

-- Verificar si la tabla ya existe antes de crearla
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AgencyInscriptionBoardExecutiveAuthority' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[AgencyInscriptionBoardExecutiveAuthority]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [AgencyInscriptionId] [int] NOT NULL,
        [OptionSelectionId] [int] NOT NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_AgencyInscriptionBoardExecutiveAuthority] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_AgencyInscriptionBoardExecutiveAuthority_AgencyInscriptionId_OptionSelectionId] UNIQUE ([AgencyInscriptionId], [OptionSelectionId])
    );

    -- =============================================
    -- Índices
    -- =============================================

    CREATE INDEX [IX_AgencyInscriptionBoardExecutiveAuthority_AgencyInscriptionId] ON [AgencyInscriptionBoardExecutiveAuthority]([AgencyInscriptionId]);
    CREATE INDEX [IX_AgencyInscriptionBoardExecutiveAuthority_OptionSelectionId] ON [AgencyInscriptionBoardExecutiveAuthority]([OptionSelectionId]);
    CREATE INDEX [IX_AgencyInscriptionBoardExecutiveAuthority_IsActive] ON [AgencyInscriptionBoardExecutiveAuthority]([IsActive]);

    -- =============================================
    -- Foreign Keys
    -- =============================================

    ALTER TABLE [AgencyInscriptionBoardExecutiveAuthority] ADD CONSTRAINT [FK_AgencyInscriptionBoardExecutiveAuthority_AgencyInscription] FOREIGN KEY([AgencyInscriptionId]) REFERENCES [AgencyInscription]([Id]) ON DELETE CASCADE;
    ALTER TABLE [AgencyInscriptionBoardExecutiveAuthority] ADD CONSTRAINT [FK_AgencyInscriptionBoardExecutiveAuthority_OptionSelection] FOREIGN KEY([OptionSelectionId]) REFERENCES [OptionSelection]([Id]);

    -- =============================================
    -- Comentarios
    -- =============================================

    EXEC sys.sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'Tabla para almacenar las funciones de autoridad de la Junta de Directores hacia el Director Ejecutivo. Migración desde almacenamiento JSON a tabla de relación.', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'AgencyInscriptionBoardExecutiveAuthority';

    EXEC sys.sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'Identificador único de la relación', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'AgencyInscriptionBoardExecutiveAuthority', 
        @level2type = N'COLUMN', @level2name = N'Id';

    EXEC sys.sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'ID de la inscripción de la agencia', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'AgencyInscriptionBoardExecutiveAuthority', 
        @level2type = N'COLUMN', @level2name = N'AgencyInscriptionId';

    EXEC sys.sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'ID de la opción de selección (OptionKey = ''boardExecutiveAuthority'')', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'AgencyInscriptionBoardExecutiveAuthority', 
        @level2type = N'COLUMN', @level2name = N'OptionSelectionId';

    EXEC sys.sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'Indica si la relación está activa', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'AgencyInscriptionBoardExecutiveAuthority', 
        @level2type = N'COLUMN', @level2name = N'IsActive';

    PRINT 'Tabla AgencyInscriptionBoardExecutiveAuthority creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla AgencyInscriptionBoardExecutiveAuthority ya existe.';
END
GO
