-- =============================================
-- Migración: ChildGroupId obligatorio en SiteOperatingDayService
-- Descripción: Elimina filas sin grupo, hace ChildGroupId NOT NULL y ajusta índices/FK.
-- Ejecutar después de 100_* y antes de nuevos inserts que asuman grupo requerido.
-- =============================================

SET NOCOUNT ON;

BEGIN TRY
    -- 1. Eliminar filas legacy sin grupo (servicios que no vienen de la relación con grupos)
    DECLARE @deleted INT;
    DELETE FROM [dbo].[SiteOperatingDayService]
    WHERE [ChildGroupId] IS NULL;
    SET @deleted = @@ROWCOUNT;
    IF @deleted > 0
        PRINT 'Eliminadas ' + CAST(@deleted AS NVARCHAR(20)) + ' filas de SiteOperatingDayService con ChildGroupId NULL.';

    -- 2. Eliminar todos los objetos que referencian ChildGroupId (índices y FK) antes de ALTER COLUMN
    -- 2a. Índice no único sobre ChildGroupId
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SiteOperatingDayService_ChildGroupId' AND object_id = OBJECT_ID(N'[dbo].[SiteOperatingDayService]'))
    BEGIN
        DROP INDEX [IX_SiteOperatingDayService_ChildGroupId] ON [dbo].[SiteOperatingDayService];
        PRINT 'Índice IX_SiteOperatingDayService_ChildGroupId eliminado.';
    END

    -- 2b. Índices únicos
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UK_SiteOperatingDayService_OperatingDay_ServiceType' AND object_id = OBJECT_ID(N'[dbo].[SiteOperatingDayService]'))
    BEGIN
        DROP INDEX [UK_SiteOperatingDayService_OperatingDay_ServiceType] ON [dbo].[SiteOperatingDayService];
        PRINT 'Índice UK_SiteOperatingDayService_OperatingDay_ServiceType eliminado.';
    END

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UK_SiteOperatingDayService_OperatingDay_ServiceType_ChildGroup' AND object_id = OBJECT_ID(N'[dbo].[SiteOperatingDayService]'))
    BEGIN
        DROP INDEX [UK_SiteOperatingDayService_OperatingDay_ServiceType_ChildGroup] ON [dbo].[SiteOperatingDayService];
        PRINT 'Índice UK_SiteOperatingDayService_OperatingDay_ServiceType_ChildGroup eliminado.';
    END

    -- 3. Eliminar FK para poder alterar la columna (y cambiar ON DELETE)
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SiteOperatingDayService_ChildGroup' AND parent_object_id = OBJECT_ID(N'[dbo].[SiteOperatingDayService]'))
    BEGIN
        ALTER TABLE [dbo].[SiteOperatingDayService]
        DROP CONSTRAINT [FK_SiteOperatingDayService_ChildGroup];
        PRINT 'FK_SiteOperatingDayService_ChildGroup eliminada.';
    END

    -- 4. Hacer ChildGroupId NOT NULL
    ALTER TABLE [dbo].[SiteOperatingDayService]
    ALTER COLUMN [ChildGroupId] [int] NOT NULL;
    PRINT 'Columna ChildGroupId establecida como NOT NULL.';

    -- 5. Recrear FK con ON DELETE NO ACTION (no permitir borrar grupo si tiene servicios)
    ALTER TABLE [dbo].[SiteOperatingDayService]
    ADD CONSTRAINT [FK_SiteOperatingDayService_ChildGroup]
    FOREIGN KEY([ChildGroupId]) REFERENCES [dbo].[SiteChildGroup]([Id]) ON DELETE NO ACTION;
    PRINT 'FK_SiteOperatingDayService_ChildGroup creada (ON DELETE NO ACTION).';

    -- 6. Recrear índice no único sobre ChildGroupId
    CREATE INDEX [IX_SiteOperatingDayService_ChildGroupId] ON [dbo].[SiteOperatingDayService]([ChildGroupId]);
    PRINT 'Índice IX_SiteOperatingDayService_ChildGroupId creado.';

    -- 7. Crear índice único (sin filtro; todas las filas tienen grupo)
    CREATE UNIQUE INDEX [UK_SiteOperatingDayService_OperatingDay_ServiceType_ChildGroup]
    ON [dbo].[SiteOperatingDayService]([OperatingDayId], [ServiceTypeId], [ChildGroupId]);
    PRINT 'Índice único UK_SiteOperatingDayService_OperatingDay_ServiceType_ChildGroup creado.';

    -- 8. Actualizar descripción de la columna (usar UPDATE si existe; si no existe, ADD)
    IF EXISTS (
        SELECT 1 FROM sys.extended_properties p
        INNER JOIN sys.all_objects o ON p.major_id = o.object_id AND p.minor_id > 0
        INNER JOIN sys.columns c ON c.object_id = o.object_id AND c.column_id = p.minor_id AND c.name = N'ChildGroupId'
        WHERE o.name = N'SiteOperatingDayService' AND o.schema_id = SCHEMA_ID(N'dbo') AND p.name = N'MS_Description'
    )
    BEGIN
        EXEC sys.sp_updateextendedproperty
            @name = N'MS_Description',
            @value = N'ID del grupo de niños (FK a SiteChildGroup, requerido).',
            @level0type = N'SCHEMA', @level0name = N'dbo',
            @level1type = N'TABLE', @level1name = N'SiteOperatingDayService',
            @level2type = N'COLUMN', @level2name = N'ChildGroupId';
    END
    ELSE
    BEGIN
        EXEC sys.sp_addextendedproperty
            @name = N'MS_Description',
            @value = N'ID del grupo de niños (FK a SiteChildGroup, requerido).',
            @level0type = N'SCHEMA', @level0name = N'dbo',
            @level1type = N'TABLE', @level1name = N'SiteOperatingDayService',
            @level2type = N'COLUMN', @level2name = N'ChildGroupId';
    END

    PRINT 'Migración 109_MakeChildGroupIdRequired completada correctamente.';
END TRY
BEGIN CATCH
    DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @sev INT = ERROR_SEVERITY();
    DECLARE @state INT = ERROR_STATE();
    RAISERROR(@msg, @sev, @state);
END CATCH
GO
