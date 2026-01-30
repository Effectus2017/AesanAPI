-- =============================================
-- Tabla: SiteChildGroupService
-- Descripción: Servicios de alimentación por grupo (ChildGroupId + ServiceTypeId).
-- Sin SiteId: el grupo ya pertenece al sitio. Relación Sitio > Grupo > Servicios.
-- Fecha: 2026-01-27
-- =============================================

CREATE TABLE [dbo].[SiteChildGroupService]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ChildGroupId] [int] NOT NULL,
    [ServiceTypeId] [int] NOT NULL,
    [IsOffered] [bit] NOT NULL DEFAULT 0,
    [FromTime] [time] NULL,
    [ToTime] [time] NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteChildGroupService] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SiteChildGroupService_SiteChildGroup] FOREIGN KEY([ChildGroupId]) REFERENCES [dbo].[SiteChildGroup]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SiteChildGroupService_ServiceType] FOREIGN KEY([ServiceTypeId]) REFERENCES [dbo].[ServiceType]([Id]),
    CONSTRAINT [UX_SiteChildGroupService_ChildGroupId_ServiceTypeId] UNIQUE ([ChildGroupId], [ServiceTypeId])
);

CREATE INDEX [IX_SiteChildGroupService_ChildGroupId] ON [dbo].[SiteChildGroupService]([ChildGroupId]);
CREATE INDEX [IX_SiteChildGroupService_ServiceTypeId] ON [dbo].[SiteChildGroupService]([ServiceTypeId]);

EXEC sys.sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Servicios de alimentación por grupo. Una fila por (ChildGroupId, ServiceTypeId). Sin SiteId.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'SiteChildGroupService';
