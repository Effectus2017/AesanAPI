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
    -- Si la agencia tiene registro de educación básica
    -- Does it have a Basic Education Registry Certificate?
    -- Si (1) y No (2)
    BasicEducationRegistry bit NULL DEFAULT (0),
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
    -- ¿Es usted una Agencia Auspiciadora de Hogares? (Solo para programa PACNA)
    -- Are you a Day Care Homes? (Only for PACNA program)
    -- Si (1) y No (2)
    IsDayCareHome bit NULL DEFAULT (0),
    -- Fecha limite para completar la inscripción de los Sitios
    -- Deadline to complete the registration of the Sites
    -- 10 minutos (10), 30 minutos (30), 1 hora (60), 2 horas (120), 3 horas (180), 4 horas (240), 5 horas (300), 6 horas (360), 7 horas (420), 8 horas (480), 9 horas (540), 10 horas (600)
    DeadlineToCompleteRegistration datetime NULL,
    -- Campo de fecha de registro de la inscripción completada
    -- Date of completed registration
    CompletedRegistrationDate datetime NULL,
    -- ¿Está interesado en participar de horario extendido? (Solo para PACNA)
    -- Are you interested in participating in extended hours? (Only for PACNA)
    -- Si (1) y No (2)
    ExtendedHours bit NULL DEFAULT (0),
    -- ¿Qué razón por la cual fue descalificado o denegado de fondos estatales?
    -- What reason was the sponsor disqualified or denied state funds?
    StateFundsDeniedReason nvarchar(max) NULL,
    -- ¿Qué razón por la cual fue descalificado o denegado de fondos federales?
    -- What reason was the sponsor disqualified or denied federal funds?
    FederalFundsDeniedReason nvarchar(max) NULL,

    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);

ALTER TABLE AgencyInscription
ADD CompletedRegistrationDate datetime NULL;
GO

ALTER TABLE AgencyInscription
ADD TaxExemptionStatusId int NULL,
    TaxExemptionTypeId int NULL,
    TypeOfEntityId int NULL,
    TypeOfApplicantId int NULL,
    PublicAllianceContractId int NULL,
    NationalYouthProgram bit NULL DEFAULT (0),
    IsDayCareHome bit NULL DEFAULT (0);
GO

-- Add foreign key constraints for OptionSelection fields
ALTER TABLE AgencyInscription
ADD FOREIGN KEY (TaxExemptionStatusId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (TaxExemptionTypeId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (TypeOfEntityId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (TypeOfApplicantId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (PublicAllianceContractId) REFERENCES OptionSelection(Id);
GO

-- Agregar columna para la fecha limite para completar la inscripción de los Sitios
-- Deadline to complete the registration of the Sites
ALTER TABLE AgencyInscription
ADD DeadlineToCompleteRegistration datetime NULL;
GO

-- hacer un update para todos los registros de la tabla AgencyInscription y poner la fecha limite para completar la inscripción de los Sitios
UPDATE AgencyInscription
SET DeadlineToCompleteRegistration = DATEADD(DAY, 10, GETDATE());
GO