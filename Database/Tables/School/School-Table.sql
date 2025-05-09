-- Tabla: School (Sitios/Escuelas)
-- Versión 1.0 - Estructura extendida según requerimientos
CREATE TABLE School (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL, -- Nombre del Sitio
    StartDate DATE NULL, -- Fecha de Inicio
    Address NVARCHAR(255) NOT NULL, -- Dirección Física
    PostalAddress NVARCHAR(255) NULL, -- Dirección Postal
    ZipCode NVARCHAR(20) NOT NULL, -- Código Postal
    CityId INT NOT NULL, -- Pueblo
    RegionId INT NOT NULL, -- Región
    AreaCode NVARCHAR(10) NULL, -- Código de Área
    AdminFullName NVARCHAR(255) NULL, -- Nombre y Apellidos del Administrador/Representante
    Phone NVARCHAR(20) NULL, -- Teléfono
    PhoneExtension NVARCHAR(10) NULL, -- Extensión
    Mobile NVARCHAR(20) NULL, -- Celular
    BaseYear INT NULL, -- Año Base
    NextRenewalYear INT NULL, -- Año Próxima Renovación
    OrganizationTypeId INT NOT NULL, -- Tipo de Organización
    EducationLevelId INT NOT NULL, -- Nivel Educativo
    OperatingPeriodId INT NOT NULL, -- Período de Funcionamiento
    KitchenTypeId INT NULL, -- Tipo de Cocina (catálogo)
    GroupTypeId INT NULL, -- Tipo de Grupo (catálogo)
    DeliveryTypeId INT NULL, -- Tipo de Entrega (catálogo)
    SponsorTypeId INT NULL, -- Tipo de Auspiciador (catálogo)
    ApplicantTypeId INT NULL, -- Tipo de Solicitante (catálogo)
    OperatingPolicyId INT NULL, -- Política de Funcionamiento (catálogo)
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Relaciones
ALTER TABLE School
    ADD CONSTRAINT FK_School_City FOREIGN KEY (CityId) REFERENCES City(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_Region FOREIGN KEY (RegionId) REFERENCES Region(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_OrganizationType FOREIGN KEY (OrganizationTypeId) REFERENCES OrganizationType(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_EducationLevel FOREIGN KEY (EducationLevelId) REFERENCES EducationLevel(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_OperatingPeriod FOREIGN KEY (OperatingPeriodId) REFERENCES OperatingPeriod(Id);

-- Facilidades (Almacén, Salón Comedor) se gestionan en SchoolFacility
-- Los catálogos KitchenType, GroupType, DeliveryType, SponsorType, ApplicantType, OperatingPolicy deben crearse si no existen. 