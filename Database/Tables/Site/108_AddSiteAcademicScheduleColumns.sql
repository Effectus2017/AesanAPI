-- =============================================
-- Script: 108_AddSiteAcademicScheduleColumns
-- Descripción: Agrega columnas de Horario Académico (PDAM) a la tabla Site
-- Campos: Inicio de la Primera Clase Académica, Finalización de la Última Clase Académica
-- Fecha: 2026-02-02
-- =============================================

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Site]') AND name = 'FirstAcademicClassStartTime'
)
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [FirstAcademicClassStartTime] [time] NULL;

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Inicio de la primera clase académica (Horario Académico PDAM)',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'Site',
        @level2type = N'COLUMN', @level2name = N'FirstAcademicClassStartTime';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Site]') AND name = 'LastAcademicClassEndTime'
)
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [LastAcademicClassEndTime] [time] NULL;

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Finalización de la última clase académica (Horario Académico PDAM)',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'Site',
        @level2type = N'COLUMN', @level2name = N'LastAcademicClassEndTime';
END
GO
