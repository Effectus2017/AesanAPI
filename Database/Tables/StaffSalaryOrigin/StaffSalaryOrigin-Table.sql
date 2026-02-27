-- =============================================
-- Tabla: StaffSalaryOrigin
-- Descripción: Tabla para almacenar los orígenes del salario del staff (selección múltiple).
-- Migración desde: Staff.SalaryOriginIds (columna NVARCHAR) a tabla de relación
-- Fecha: 2025-02
-- Versión: 1.0
-- =============================================

-- Verificar si la tabla ya existe antes de crearla
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StaffSalaryOrigin' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[StaffSalaryOrigin]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [StaffId] [int] NOT NULL,
        [OptionSelectionId] [int] NOT NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_StaffSalaryOrigin] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_StaffSalaryOrigin_StaffId_OptionSelectionId] UNIQUE ([StaffId], [OptionSelectionId])
    );

    -- =============================================
    -- Índices
    -- =============================================

    CREATE INDEX [IX_StaffSalaryOrigin_StaffId] ON [StaffSalaryOrigin]([StaffId]);
    CREATE INDEX [IX_StaffSalaryOrigin_OptionSelectionId] ON [StaffSalaryOrigin]([OptionSelectionId]);
    CREATE INDEX [IX_StaffSalaryOrigin_IsActive] ON [StaffSalaryOrigin]([IsActive]);

    -- =============================================
    -- Foreign Keys
    -- =============================================

    ALTER TABLE [StaffSalaryOrigin] ADD CONSTRAINT [FK_StaffSalaryOrigin_Staff] FOREIGN KEY([StaffId]) REFERENCES [Staff]([Id]) ON DELETE CASCADE;
    ALTER TABLE [StaffSalaryOrigin] ADD CONSTRAINT [FK_StaffSalaryOrigin_OptionSelection] FOREIGN KEY([OptionSelectionId]) REFERENCES [OptionSelection]([Id]);

    -- =============================================
    -- Comentarios
    -- =============================================

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Tabla para almacenar los orígenes del salario del staff. Migración desde columna Staff.SalaryOriginIds a tabla de relación.',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'StaffSalaryOrigin';

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Identificador único de la relación',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'StaffSalaryOrigin',
        @level2type = N'COLUMN', @level2name = N'Id';

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'ID del staff',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'StaffSalaryOrigin',
        @level2type = N'COLUMN', @level2name = N'StaffId';

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'ID de la opción de selección (OptionKey = ''salaryOrigin'')',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'StaffSalaryOrigin',
        @level2type = N'COLUMN', @level2name = N'OptionSelectionId';

    EXEC sys.sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'Indica si la relación está activa',
        @level0type = N'SCHEMA', @level0name = N'dbo',
        @level1type = N'TABLE', @level1name = N'StaffSalaryOrigin',
        @level2type = N'COLUMN', @level2name = N'IsActive';

    PRINT 'Tabla StaffSalaryOrigin creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla StaffSalaryOrigin ya existe.';
END
GO
