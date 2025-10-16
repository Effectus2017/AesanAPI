-- =============================================
-- Tabla: SchoolSite
-- Descripción: Tabla relacional para asignar Sites a Schools
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SchoolSite]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SchoolId] [int] NOT NULL,
    [SiteId] [int] NOT NULL,
    [AssignmentDate] [datetime] NOT NULL DEFAULT GETDATE(),
    [Comment] [nvarchar](500) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SchoolSite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_SchoolSite_SiteId] UNIQUE ([SiteId])
    -- Un Site solo puede estar asignado a una School activa
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SchoolSite_SchoolId] ON [SchoolSite]([SchoolId]);
CREATE INDEX [IX_SchoolSite_SiteId] ON [SchoolSite]([SiteId]);
CREATE INDEX [IX_SchoolSite_IsActive] ON [SchoolSite]([IsActive]);
CREATE INDEX [IX_SchoolSite_AssignmentDate] ON [SchoolSite]([AssignmentDate]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SchoolSite] ADD CONSTRAINT [FK_SchoolSite_School] FOREIGN KEY([SchoolId]) REFERENCES [School]([Id]);
ALTER TABLE [SchoolSite] ADD CONSTRAINT [FK_SchoolSite_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]);