-- DEPRECATED: Esta versión de la tabla School ha sido reemplazada por una nueva versión. No modificar ni usar para nuevas migraciones
-- Tabla: School (Sitios/Escuelas)
-- Versión 2.3 - Estructura con SiteNumber para numeración consecutiva por agencia
-- Última actualización: 2025-01-15 - Agregado SiteNumber con índice único por agencia
CREATE TABLE School
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,

    StartDate DATE NULL,
    Address NVARCHAR(255) NOT NULL,
    CityId INT NOT NULL,
    RegionId INT NOT NULL,
    ZipCode NVARCHAR(20) NOT NULL,
    Latitude FLOAT NULL,
    Longitude FLOAT NULL,

    PostalAddress NVARCHAR(255) NULL,
    PostalCityId INT NULL,
    PostalRegionId INT NULL,
    PostalZipCode NVARCHAR(20) NULL,
    SameAsPhysicalAddress BIT NULL,

    OrganizationTypeId INT NOT NULL,
    CenterTypeId INT NULL,
    NonProfit BIT NULL,
    BaseYear INT NULL,
    RenewalYear INT NULL,
    OperatingFromDate DATE NULL,
    OperatingToDate DATE NULL,
    OperatingDaysCalculated INT NULL,
    KitchenTypeId INT NULL,
    GroupTypeId INT NULL,
    DeliveryTypeId INT NULL,
    SponsorTypeId INT NULL,
    ApplicantTypeId INT NULL,
    ResidentialTypeId INT NULL,
    OperatingPolicyId INT NULL,
    AreaTypeId INT NULL,
    LocationTypeId INT NULL,
    HasWarehouse BIT NULL,
    HasDiningRoom BIT NULL,
    SitePhone NVARCHAR(20) NULL,
    Extension NVARCHAR(10) NULL,
    MobilePhone NVARCHAR(20) NULL,
    CommunityId INT NULL,
    WalkersId INT NULL,
    SiteTypeId INT NULL,
    ExperienceId INT NULL,
    ReviewResultId INT NULL,
    ReviewDate DATETIME NULL,
    ReviewJustification NVARCHAR(500) NULL,
    GeneralEnrollment INT NULL,
    -- Matrícula General - Número total de estudiantes matriculados
    SiteNumber INT NOT NULL,
    -- Número de Sitio - Contador consecutivo por agencia
    IsActive BIT NOT NULL DEFAULT 1,
    InactiveJustification NVARCHAR(500) NULL,
    InactiveDate DATETIME NULL,
    SiteCode NVARCHAR(255) NULL,
    SiteLocationId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Relaciones
ALTER TABLE School
    ADD CONSTRAINT FK_School_City FOREIGN KEY (CityId) REFERENCES City(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_Region FOREIGN KEY (RegionId) REFERENCES Region(Id);

-- OrganizationType se maneja en su tabla OrganizationType
ALTER TABLE School
    ADD CONSTRAINT FK_School_OrganizationType FOREIGN KEY (OrganizationTypeId) REFERENCES OrganizationType(Id);

-- EducationLevel se maneja en su tabla SchoolEducationLevel
-- ALTER TABLE School
--     ADD CONSTRAINT FK_School_EducationLevel FOREIGN KEY (EducationLevelId) REFERENCES EducationLevel(Id);

-- KitchenType se maneja en su tabla KitchenType
ALTER TABLE School
    ADD CONSTRAINT FK_School_KitchenType FOREIGN KEY (KitchenTypeId) REFERENCES KitchenType(Id);

-- GroupType se maneja en su tabla GroupType
ALTER TABLE School
    ADD CONSTRAINT FK_School_GroupType FOREIGN KEY (GroupTypeId) REFERENCES GroupType(Id);

-- DeliveryType se maneja en su tabla DeliveryType
ALTER TABLE School
    ADD CONSTRAINT FK_School_DeliveryType FOREIGN KEY (DeliveryTypeId) REFERENCES DeliveryType(Id);

-- SponsorType se maneja en su tabla SponsorType
ALTER TABLE School
    ADD CONSTRAINT FK_School_SponsorType FOREIGN KEY (SponsorTypeId) REFERENCES SponsorType(Id);

-- ApplicantType se maneja en OptionSelection (Fix)
ALTER TABLE School
    ADD CONSTRAINT FK_School_ApplicantType FOREIGN KEY (ApplicantTypeId) REFERENCES OptionSelection(Id);

-- ResidentialType se maneja en OptionSelection (Fix)
ALTER TABLE School
    ADD CONSTRAINT FK_School_ResidentialType FOREIGN KEY (ResidentialTypeId) REFERENCES OptionSelection(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_OperatingPolicy FOREIGN KEY (OperatingPolicyId) REFERENCES OperatingPolicy(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_PostalCity FOREIGN KEY (PostalCityId) REFERENCES City(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_PostalRegion FOREIGN KEY (PostalRegionId) REFERENCES Region(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_Agency FOREIGN KEY (AgencyId) REFERENCES Agency(Id);


-- Facilidades (Almacén, Salón Comedor) se gestionan en SchoolFacility
-- Los catálogos KitchenType, GroupType, DeliveryType, SponsorType, ApplicantType, OperatingPolicy deben crearse si no existen.

-- NOTA: Los siguientes campos fueron migrados a tablas especializadas:
-- - Servicios de comidas (Breakfast, Lunch, Dinner, Snack, SnackNight y sus horarios) -> SchoolService
-- - Campos específicos de Day Care Home (AdministratorAuthorizedName, etc.) -> SchoolDayCareHome
-- - Tipos de participantes -> SchoolParticipant 

ALTER TABLE School
    ADD CONSTRAINT FK_School_CenterType FOREIGN KEY (CenterTypeId) REFERENCES CenterType(Id);

-- alter table para agregar columna TypeOfAreaId
ALTER TABLE School
    ADD AreaTypeId INT NULL;

ALTER TABLE School
    ADD CONSTRAINT FK_School_AreaType FOREIGN KEY (AreaTypeId) REFERENCES AreaType(Id);

-- LocationType se maneja en su tabla AreaType (mismos valores que AreaType)
ALTER TABLE School
    ADD CONSTRAINT FK_School_LocationType FOREIGN KEY (LocationTypeId) REFERENCES AreaType(Id);

-- Community se maneja en su tabla Community
ALTER TABLE School
    ADD CONSTRAINT FK_School_Community FOREIGN KEY (CommunityId) REFERENCES Community(Id);

-- Walkers se maneja en su tabla Walkers
ALTER TABLE School
    ADD CONSTRAINT FK_School_Walkers FOREIGN KEY (WalkersId) REFERENCES Walkers(Id);

-- SiteType se maneja en su tabla SiteType
ALTER TABLE School
    ADD CONSTRAINT FK_School_SiteType FOREIGN KEY (SiteTypeId) REFERENCES SiteType(Id);

-- Experience se maneja en su tabla Experience
ALTER TABLE School
    ADD CONSTRAINT FK_School_Experience FOREIGN KEY (ExperienceId) REFERENCES Experience(Id);

-- ReviewResult se maneja en su tabla ReviewResult
ALTER TABLE School
    ADD CONSTRAINT FK_School_ReviewResult FOREIGN KEY (ReviewResultId) REFERENCES ReviewResult(Id);

-- SiteLocation se maneja en su tabla OptionSelection
ALTER TABLE School
    ADD CONSTRAINT FK_School_SiteLocation FOREIGN KEY (SiteLocationId) REFERENCES OptionSelection(Id);

-- Índice único para garantizar unicidad de SiteNumber por agencia
CREATE UNIQUE INDEX UK_School_AgencyId_SiteNumber ON School(AgencyId, SiteNumber);



