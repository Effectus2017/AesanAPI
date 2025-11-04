-- =============================================
-- Script: Migración - Agregar IDENTITY a ServiceType.Id
-- Descripción: Agrega IDENTITY a la columna Id si no lo tiene
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

-- Verificar si la tabla existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServiceType]') AND type in (N'U'))
BEGIN
    -- Verificar si la columna Id ya tiene IDENTITY
    DECLARE @hasIdentity BIT = 0;
    
    SELECT @hasIdentity = CASE 
        WHEN is_identity = 1 THEN 1 
        ELSE 0 
    END
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[ServiceType]')
        AND name = 'Id';
    
    IF @hasIdentity = 0
    BEGIN
        PRINT 'La columna Id no tiene IDENTITY. Agregando IDENTITY...';
        
        -- Crear tabla temporal con estructura correcta
        CREATE TABLE [dbo].[ServiceType_Temp]
        (
            [Id] [int] IDENTITY(1,1) NOT NULL,
            [Name] [nvarchar](100) NOT NULL,
            [NameEN] [nvarchar](100) NOT NULL,
            [Code] [nvarchar](50) NOT NULL,
            [Description] [nvarchar](500) NULL,
            [DisplayOrder] [int] NOT NULL DEFAULT 0,
            [IsActive] [bit] NOT NULL DEFAULT 1,
            [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
            [UpdatedAt] [datetime] NULL
        );
        
        -- Copiar datos existentes (si hay)
        SET IDENTITY_INSERT [dbo].[ServiceType_Temp] ON;
        
        INSERT INTO [dbo].[ServiceType_Temp] 
            ([Id], [Name], [NameEN], [Code], [Description], [DisplayOrder], [IsActive], [CreatedAt], [UpdatedAt])
        SELECT 
            [Id], [Name], [NameEN], [Code], [Description], [DisplayOrder], [IsActive], [CreatedAt], [UpdatedAt]
        FROM [dbo].[ServiceType];
        
        SET IDENTITY_INSERT [dbo].[ServiceType_Temp] OFF;
        
        -- Eliminar índices y constraints de la tabla original
        IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_ServiceType_Code' AND object_id = OBJECT_ID(N'[dbo].[ServiceType]'))
        BEGIN
            DROP INDEX [UX_ServiceType_Code] ON [dbo].[ServiceType];
        END
        
        IF EXISTS (SELECT * FROM sys.objects WHERE name = 'PK_ServiceType' AND type = 'PK' AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceType]'))
        BEGIN
            ALTER TABLE [dbo].[ServiceType] DROP CONSTRAINT [PK_ServiceType];
        END
        
        -- Eliminar tabla original
        DROP TABLE [dbo].[ServiceType];
        
        -- Renombrar tabla temporal
        EXEC sp_rename '[dbo].[ServiceType_Temp]', 'ServiceType';
        
        -- Recrear constraints e índices
        ALTER TABLE [dbo].[ServiceType]
        ADD CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED ([Id] ASC);
        
        CREATE UNIQUE INDEX [UX_ServiceType_Code] ON [dbo].[ServiceType]([Code]);
        
        -- Resetear IDENTITY al máximo ID existente
        DECLARE @maxId INT;
        SELECT @maxId = ISNULL(MAX(Id), 0) FROM [dbo].[ServiceType];
        DBCC CHECKIDENT('[dbo].[ServiceType]', RESEED, @maxId);
        
        PRINT 'IDENTITY agregado exitosamente. IDENTITY reseed a ' + CAST(@maxId AS VARCHAR);
    END
    ELSE
    BEGIN
        PRINT 'La columna Id ya tiene IDENTITY. No se requiere migración.';
    END
END
ELSE
BEGIN
    PRINT 'La tabla ServiceType no existe. Ejecutar ServiceType-Table.sql primero.';
END
GO

