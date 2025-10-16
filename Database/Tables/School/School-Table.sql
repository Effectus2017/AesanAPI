-- =============================================
-- Tabla: School
-- Descripción: Tabla para almacenar información de escuelas
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[School]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AgencyId] [int] NOT NULL,
    [Name] [nvarchar](255) NOT NULL,
    [SchoolCode] [nvarchar](50) NULL,
    [SchoolNumber] [int] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_School] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

-- Índice único para AgencyId + SchoolNumber
CREATE UNIQUE INDEX [UK_School_AgencyId_SchoolNumber] ON [School]([AgencyId], [SchoolNumber]);

-- Índices para optimizar consultas
CREATE INDEX [IX_School_AgencyId] ON [School]([AgencyId]);
CREATE INDEX [IX_School_IsActive] ON [School]([IsActive]);
CREATE INDEX [IX_School_SchoolNumber] ON [School]([SchoolNumber]);
CREATE INDEX [IX_School_CreatedAt] ON [School]([CreatedAt]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [School] ADD CONSTRAINT [FK_School_Agency] FOREIGN KEY([AgencyId]) REFERENCES [Agency]([Id]);
