-- =============================================
-- Tabla: SchoolDayCareHome
-- Descripción: Información específica para Day Care Homes
-- Fecha: 2025-01-15
-- =============================================

CREATE TABLE SchoolDayCareHome
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    IsAuthorizedToOperate BIT NULL,
    HasFamilyDepartmentLicense BIT NULL,
    NumberOfEnrolledChildren INT NULL,
    NumberOfProviderChildren INT NULL,
    NumberOfParticipantsWithBloodTies INT NULL,
    NumberOfParticipantsWithoutBloodTies INT NULL,
    MinorsLiveWithProvider BIT NULL,
    RelationshipTypeId INT NULL,
    OffersServiceToImmigrantChildren BIT NULL,
    HomeTypeId INT NULL,
    AdministratorAuthorizedName NVARCHAR(255) NULL,
    AdministratorBirthDate DATE NULL,
    OffersServiceToDifferentGroups BIT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT FK_SchoolDayCareHome_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SchoolDayCareHome_RelationshipTypeId FOREIGN KEY (RelationshipTypeId) REFERENCES OptionSelection(Id),
    CONSTRAINT FK_SchoolDayCareHome_HomeTypeId FOREIGN KEY (HomeTypeId) REFERENCES OptionSelection(Id),
    CONSTRAINT UK_SchoolDayCareHome_SchoolId UNIQUE (SchoolId)
    -- Relación 1:1 con School
);

-- Índices para optimizar consultas
CREATE INDEX IX_SchoolDayCareHome_SchoolId ON SchoolDayCareHome(SchoolId);
CREATE INDEX IX_SchoolDayCareHome_RelationshipTypeId ON SchoolDayCareHome(RelationshipTypeId);
CREATE INDEX IX_SchoolDayCareHome_HomeTypeId ON SchoolDayCareHome(HomeTypeId);