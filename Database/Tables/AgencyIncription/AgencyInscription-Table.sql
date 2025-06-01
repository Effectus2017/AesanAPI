-- Nueva versión de la tabla se encarga de almacenar los datos de la solicutud de participación de la agencia (sponsor), no es lo mismo que la solicutud a un programa
-- 1.1.0
CREATE TABLE AgencyInscription
(
    Id int NOT NULL IDENTITY(1,1),
    -- Id de la agencia
    AgencyId int NOT NULL,
    -- Si es una organización sin fines de lucro
    -- Is it a non-profit organization?
    -- Si (1) y No (2)
    NonProfit bit NULL DEFAULT (0),
    -- Si la agencia no acepta fondos federales
    -- Does it accept federal funds?
    -- Si (1) y No (2)
    FederalFundsDenied bit NULL DEFAULT (0),
    -- Si la agencia no acepta fondos estatales
    -- Does it accept state funds?
    -- Si (1) y No (2)
    StateFundsDenied bit NULL DEFAULT (0),
    -- Si la agencia tiene programas de atletismo organizados
    -- Does it have organized athletic programs?
    -- Si (1) y No (2)
    OrganizedAthleticPrograms bit NULL DEFAULT (0),
    -- Si la agencia ofrece servicios a personas en riesgo
    -- Does it offer services to at-risk individuals?
    -- Si (1) y No (2)
    AtRiskService bit NULL DEFAULT (0),
    -- Si la agencia tiene registro de educación básica
    -- Does it have a Basic Education Registry Certificate?
    -- En Proceso (3), Otorgado (4), Denegado (5)
    BasicEducationRegistryId int NULL,
    -- Si la agencia tiene servicio de tiempo
    ServiceTime datetime NULL,
    -- Si la agencia fue rechazada, se guarda la justificación
    RejectionJustification nvarchar(max) NULL,
    -- Si la agencia fue rechazada, se guarda la justificación
    Comments NVARCHAR(MAX) NULL,
    -- Comentarios extras para la agencia
    AppointmentCoordinated BIT NULL DEFAULT 0,
    -- Si la agencia tiene cita programada
    AppointmentDate DATETIME NULL,
    -- ¿En qué estatus se encuentra su Exención Contributiva?
    -- In what status is your Tax Exemption?
    -- En Proceso (3), Otorgado (4), Denegado (5)
    TaxExemptionStatusId int NULL,
    -- ¿Qué tipo de Exención Contributiva tiene? (Tabla OptionSelection)
    -- What type of Tax Exemption does it have?
    -- Estatal (11), Federal (12) (Tabla OptionSelection)
    TaxExemptionTypeId int NULL,
    -- Tipo de Entidad
    -- Type of Entity
    -- Privado (14), Gobierno (15)
    TypeOfEntityId int NULL,
    -- Tipo de Solicitante
    -- Type of Applicant
    -- Laico (16), Base de fe (17)
    TypeOfApplicantId int NULL,
    -- ¿De poseer un contrato Público Alianza especifique su modalidad?
    -- If you have a Public Alliance contract, please specify the type of contract
    -- Socio-Económico (17), Híbrido (18)
    PublicAllianceContractId int NULL,
    -- ¿Su Institución es un Programa Nacional de Juventud?
    -- Is it a National Youth Program?
    -- Si (1) y No (2)
    NationalYouthProgram bit NULL DEFAULT (0),

    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);

ALTER TABLE AgencyInscription
ADD TaxExemptionStatusId int NULL,
    TaxExemptionTypeId int NULL,
    TypeOfEntityId int NULL,
    TypeOfApplicantId int NULL,
    PublicAllianceContractId int NULL,
    NationalYouthProgram bit NULL DEFAULT (0);
GO

ALTER TABLE AgencyInscription
ADD FOREIGN KEY (TaxExemptionStatusId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (TaxExemptionTypeId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (TypeOfEntityId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (TypeOfApplicantId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (PublicAllianceContractId) REFERENCES OptionSelection(Id);
GO

-- Rename BasicEducationRegistry to BasicEducationRegistryId
EXEC sp_rename 'AgencyInscription.BasicEducationRegistry', 'BasicEducationRegistryId', 'COLUMN';
GO

-- Add foreign key constraint for BasicEducationRegistryId
ALTER TABLE AgencyInscription
ADD CONSTRAINT FK_AgencyInscription_BasicEducationRegistry 
    FOREIGN KEY (BasicEducationRegistryId) REFERENCES OptionSelection(Id);
GO
