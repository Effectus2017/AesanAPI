-- =============================================
-- Tabla: Site
-- Descripción: Tabla principal para almacenar información de sitios
-- Reemplaza: School
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE TABLE [dbo].[Site]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AgencyId] [int] NOT NULL,
    [Name] [nvarchar](255) NOT NULL,
    [StartDate] [date] NULL,
    [Address] [nvarchar](255) NOT NULL,
    [CityId] [int] NOT NULL,
    [RegionId] [int] NOT NULL,
    [ZipCode] [nvarchar](20) NOT NULL,
    [Latitude] [float] NULL,
    [Longitude] [float] NULL,
    [PostalAddress] [nvarchar](255) NULL,
    [PostalCityId] [int] NULL,
    [PostalRegionId] [int] NULL,
    [PostalZipCode] [nvarchar](20) NULL,
    [SameAsPhysicalAddress] [bit] NULL,
    [OrganizationTypeId] [int] NOT NULL,
    [CenterTypeId] [int] NULL,
    [NonProfit] [bit] NULL,
    [BaseYear] [int] NULL,
    [RenewalYear] [int] NULL,
    [OperatingFromDate] [date] NULL,
    [OperatingToDate] [date] NULL,
    [OperatingDaysCalculated] [int] NULL,
    [OperatingStartTime] [time] NULL,
    [OperatingEndTime] [time] NULL,
    [KitchenTypeId] [int] NULL,
    [GroupTypeId] [int] NULL,
    [DeliveryTypeId] [int] NULL,
    [SponsorTypeId] [int] NULL,
    [ApplicantTypeId] [int] NULL,
    [ResidentialTypeId] [int] NULL,
    [OperatingPolicyId] [int] NULL,
    [AreaTypeId] [int] NULL,
    [LocationTypeId] [int] NULL,
    [HasWarehouse] [bit] NULL,
    [HasDiningRoom] [bit] NULL,
    [AdministratorAuthorizedName] [nvarchar](255) NULL,
    [SitePhone] [nvarchar](20) NULL,
    [Extension] [nvarchar](10) NULL,
    [MobilePhone] [nvarchar](20) NULL,
    [CommunityId] [int] NULL,
    [WalkersId] [int] NULL,
    [SiteTypeId] [int] NULL,
    [ExperienceId] [int] NULL,
    [ReviewResultId] [int] NULL,
    [ReviewDate] [datetime] NULL,
    [ReviewJustification] [nvarchar](500) NULL,
    [GeneralEnrollment] [int] NULL,
    [SiteNumber] [int] NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [InactiveJustification] [nvarchar](500) NULL,
    [InactiveDate] [datetime] NULL,
    [SiteCode] [nvarchar](255) NULL,
    [SiteLocationId] [int] NULL,
    [ServiceTime] [datetime] NULL,
    [OrganizedAthleticPrograms] [bit] NULL,
    [AtRiskService] [bit] NULL,
    [PublicAllianceContractId] [int] NULL,
    [IsDayCareHomeId] [int] NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =============================================
-- Índices
-- =============================================

-- Índice único para AgencyId + SiteNumber
CREATE UNIQUE INDEX [UK_Site_AgencyId_SiteNumber] ON [Site]([AgencyId], [SiteNumber]);

-- Índices para optimizar consultas
CREATE INDEX [IX_Site_AgencyId] ON [Site]([AgencyId]);
CREATE INDEX [IX_Site_CityId] ON [Site]([CityId]);
CREATE INDEX [IX_Site_RegionId] ON [Site]([RegionId]);
CREATE INDEX [IX_Site_IsActive] ON [Site]([IsActive]);
CREATE INDEX [IX_Site_SiteNumber] ON [Site]([SiteNumber]);
CREATE INDEX [IX_Site_OrganizationTypeId] ON [Site]([OrganizationTypeId]);
CREATE INDEX [IX_Site_PublicAllianceContractId] ON [Site]([PublicAllianceContractId]);
CREATE INDEX [IX_Site_IsDayCareHomeId] ON [Site]([IsDayCareHomeId]);
CREATE INDEX [IX_Site_CreatedAt] ON [Site]([CreatedAt]);

-- =============================================
-- Foreign Keys
-- =============================================

ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Agency] FOREIGN KEY([AgencyId]) REFERENCES [Agency]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_City] FOREIGN KEY([CityId]) REFERENCES [City]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Region] FOREIGN KEY([RegionId]) REFERENCES [Region]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_PostalCity] FOREIGN KEY([PostalCityId]) REFERENCES [City]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_PostalRegion] FOREIGN KEY([PostalRegionId]) REFERENCES [Region]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_OrganizationType] FOREIGN KEY([OrganizationTypeId]) REFERENCES [OrganizationType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_CenterType] FOREIGN KEY([CenterTypeId]) REFERENCES [CenterType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_KitchenType] FOREIGN KEY([KitchenTypeId]) REFERENCES [KitchenType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_GroupType] FOREIGN KEY([GroupTypeId]) REFERENCES [GroupType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_DeliveryType] FOREIGN KEY([DeliveryTypeId]) REFERENCES [DeliveryType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_SponsorType] FOREIGN KEY([SponsorTypeId]) REFERENCES [SponsorType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_ApplicantType] FOREIGN KEY([ApplicantTypeId]) REFERENCES [OptionSelection]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_ResidentialType] FOREIGN KEY([ResidentialTypeId]) REFERENCES [OptionSelection]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_OperatingPolicy] FOREIGN KEY([OperatingPolicyId]) REFERENCES [OperatingPolicy]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_AreaType] FOREIGN KEY([AreaTypeId]) REFERENCES [AreaType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_LocationType] FOREIGN KEY([LocationTypeId]) REFERENCES [AreaType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Community] FOREIGN KEY([CommunityId]) REFERENCES [Community]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Walkers] FOREIGN KEY([WalkersId]) REFERENCES [Walkers]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_SiteType] FOREIGN KEY([SiteTypeId]) REFERENCES [SiteType]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Experience] FOREIGN KEY([ExperienceId]) REFERENCES [Experience]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_ReviewResult] FOREIGN KEY([ReviewResultId]) REFERENCES [ReviewResult]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_SiteLocation] FOREIGN KEY([SiteLocationId]) REFERENCES [OptionSelection]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_PublicAllianceContract] FOREIGN KEY([PublicAllianceContractId]) REFERENCES [OptionSelection]([Id]);
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_IsDayCareHome] FOREIGN KEY([IsDayCareHomeId]) REFERENCES [OptionSelection]([Id]);