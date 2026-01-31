-- =============================================
-- Tabla: SiteOperatingDayService
-- Descripción: Tabla para almacenar servicios de alimentación relacionados con días de funcionamiento
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

IF NOT EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[SiteOperatingDayService]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SiteOperatingDayService]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [OperatingDayId] [int] NOT NULL,
        [ServiceTypeId] [int] NOT NULL,
        [ChildGroupId] [int] NOT NULL,
        [StartTime] [time] NOT NULL,
        [EndTime] [time] NOT NULL,
        [IsEnabled] [bit] NOT NULL DEFAULT 1,
        [Comment] [nvarchar](500) NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_SiteOperatingDayService] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- =============================================
    -- Índices
    -- =============================================

    CREATE INDEX [IX_SiteOperatingDayService_OperatingDayId] ON [SiteOperatingDayService]([OperatingDayId]);
    CREATE INDEX [IX_SiteOperatingDayService_ServiceTypeId] ON [SiteOperatingDayService]([ServiceTypeId]);
    CREATE INDEX [IX_SiteOperatingDayService_ChildGroupId] ON [SiteOperatingDayService]([ChildGroupId]);
    CREATE INDEX [IX_SiteOperatingDayService_IsEnabled] ON [SiteOperatingDayService]([IsEnabled]);
    CREATE INDEX [IX_SiteOperatingDayService_StartTime] ON [SiteOperatingDayService]([StartTime]);
    CREATE INDEX [IX_SiteOperatingDayService_EndTime] ON [SiteOperatingDayService]([EndTime]);

    -- UNIQUE: No puede haber servicios duplicados por día y grupo
    CREATE UNIQUE INDEX [UK_SiteOperatingDayService_OperatingDay_ServiceType_ChildGroup]
        ON [SiteOperatingDayService]([OperatingDayId], [ServiceTypeId], [ChildGroupId]);

    -- =============================================
    -- Foreign Keys
    -- =============================================

    ALTER TABLE [SiteOperatingDayService] ADD CONSTRAINT [FK_SiteOperatingDayService_OperatingDay]
        FOREIGN KEY([OperatingDayId]) REFERENCES [SiteOperatingDays]([Id]) ON DELETE CASCADE;

    ALTER TABLE [SiteOperatingDayService] ADD CONSTRAINT [FK_SiteOperatingDayService_ServiceType]
        FOREIGN KEY([ServiceTypeId]) REFERENCES [ServiceType]([Id]) ON DELETE NO ACTION;

    ALTER TABLE [SiteOperatingDayService] ADD CONSTRAINT [FK_SiteOperatingDayService_ChildGroup]
        FOREIGN KEY([ChildGroupId]) REFERENCES [SiteChildGroup]([Id]) ON DELETE NO ACTION;

    -- =============================================
    -- Check Constraints
    -- =============================================

    -- Validación: StartTime debe ser menor que EndTime
    ALTER TABLE [SiteOperatingDayService] ADD CONSTRAINT [CK_SiteOperatingDayService_TimeRange] 
        CHECK ([StartTime] < [EndTime]);

    PRINT 'Tabla SiteOperatingDayService creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla SiteOperatingDayService ya existe';
END
GO

-- =============================================
-- Comentarios (solo si la tabla existe)
-- =============================================

IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[SiteOperatingDayService]') AND type in (N'U'))
BEGIN
    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar servicios de alimentación relacionados con días de funcionamiento. Los servicios se replican automáticamente al crear días de funcionamiento.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del servicio por día', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'Id';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del día de funcionamiento (FK a SiteOperatingDays)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'OperatingDayId';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de servicio (FK a ServiceType con IDs fijos 1-10)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'ServiceTypeId';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del grupo de niños (FK a SiteChildGroup, requerido)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'ChildGroupId';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de inicio del servicio (debe estar dentro del rango del día de funcionamiento)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'StartTime';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Hora de fin del servicio (debe estar dentro del rango del día de funcionamiento)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'EndTime';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si el servicio está habilitado para este día', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'IsEnabled';

    EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Comentarios sobre el servicio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteOperatingDayService', 
    @level2type = N'COLUMN', @level2name = N'Comment';
END
GO
