-- =============================================
-- Tabla: SiteDayCareHome
-- Descripción: Tabla para almacenar información específica de Day Care Home de los sitios
-- Reemplaza: SchoolDayCareHome
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[SiteDayCareHome]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SiteId] [int] NOT NULL,
    [IsAuthorizedToOperate] [bit] NULL,
    [HasFamilyDepartmentLicense] [bit] NULL,
    [NumberOfEnrolledChildren] [int] NULL,
    [NumberOfProviderChildren] [int] NULL,
    [NumberOfParticipantsWithBloodTies] [int] NULL,
    [NumberOfParticipantsWithoutBloodTies] [int] NULL,
    [MinorsLiveWithProvider] [bit] NULL,
    [RelationshipTypeId] [int] NULL,
    [OffersServiceToImmigrantChildren] [bit] NULL,
    [HomeTypeId] [int] NULL,
    [AdministratorAuthorizedName] [nvarchar](255) NULL,
    [AdministratorBirthDate] [date] NULL,
    [OffersServiceToDifferentGroups] [bit] NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_SiteDayCareHome] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_SiteDayCareHome_SiteId] UNIQUE ([SiteId])
);

-- =============================================
-- Índices
-- =============================================

CREATE INDEX [IX_SiteDayCareHome_SiteId] ON [SiteDayCareHome]([SiteId]);
CREATE INDEX [IX_SiteDayCareHome_RelationshipTypeId] ON [SiteDayCareHome]([RelationshipTypeId]);
CREATE INDEX [IX_SiteDayCareHome_HomeTypeId] ON [SiteDayCareHome]([HomeTypeId]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [SiteDayCareHome] ADD CONSTRAINT [FK_SiteDayCareHome_Site] FOREIGN KEY([SiteId]) REFERENCES [Site]([Id]) ON DELETE CASCADE;
ALTER TABLE [SiteDayCareHome] ADD CONSTRAINT [FK_SiteDayCareHome_RelationshipType] FOREIGN KEY([RelationshipTypeId]) REFERENCES [OptionSelection]([Id]);
ALTER TABLE [SiteDayCareHome] ADD CONSTRAINT [FK_SiteDayCareHome_HomeType] FOREIGN KEY([HomeTypeId]) REFERENCES [OptionSelection]([Id]);

-- =============================================
-- Comentarios
-- =============================================

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla para almacenar información específica de Day Care Home de los sitios. Reemplaza SchoolDayCareHome.', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único del registro', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del sitio', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'SiteId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si está autorizado para operar', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'IsAuthorizedToOperate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si tiene licencia del Departamento de Familia', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'HasFamilyDepartmentLicense';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Número de niños inscritos', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'NumberOfEnrolledChildren';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Número de niños del proveedor', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'NumberOfProviderChildren';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Número de participantes con lazos de sangre', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'NumberOfParticipantsWithBloodTies';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Número de participantes sin lazos de sangre', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'NumberOfParticipantsWithoutBloodTies';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si menores viven con el proveedor', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'MinorsLiveWithProvider';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de relación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'RelationshipTypeId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si ofrece servicio a niños inmigrantes', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'OffersServiceToImmigrantChildren';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del tipo de hogar', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'HomeTypeId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Nombre del administrador autorizado', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'AdministratorAuthorizedName';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de nacimiento del administrador', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'AdministratorBirthDate';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si ofrece servicio a diferentes grupos', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SiteDayCareHome', 
    @level2type = N'COLUMN', @level2name = N'OffersServiceToDifferentGroups';
