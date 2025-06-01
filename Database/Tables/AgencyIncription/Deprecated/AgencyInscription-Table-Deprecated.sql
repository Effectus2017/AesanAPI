-- Esta tabla se encarga de almacenar los datos de la solicutud de participación de la agencia (sponsor), no es lo mismo que la solicutud a un programa
-- Deprecada
CREATE TABLE AgencyInscription
(
    Id int NOT NULL IDENTITY(1,1),
    AgencyId int NOT NULL,
    -- Si es una organización sin fines de lucro
    -- Is it a non-profit organization?
    -- Si (1) y No (2)
    NonProfit bit NULL DEFAULT (0),
    -- ¿Ha sido denegado o descalificado de fondos estatales en los últimos siete años?
    -- Have you been denied or disqualified from state funds in the last seven years?
    -- Si (1) y No (2)
    StateFundsDenied bit NULL DEFAULT (0),
    -- ¿Ha sido denegado o descalificado de fondos federales en los últimos siete años?
    -- Have you been denied or disqualified from federal funds in the last seven years?
    -- Si (1) y No (2)
    FederalFundsDenied bit NULL DEFAULT (0),
    -- ¿El Auspiciador ofrece programas atléticos organizados que participan en deportes competitivos interescolares o a nivel comunitario?
    -- Does the Sponsor offer any organized athletic programs engaged in interscholastic or community level competitive sports?
    -- Si (1) y No (2)
    OrganizedAthleticPrograms bit NULL DEFAULT (0),
    -- ¿Está interesado en participar en el servicio de merienda y cena en riesgo?
    -- Is the Sponsor interested in participating in the at-risk snack and dinner service?
    -- Si (1) y No (2)
    AtRiskService bit NULL DEFAULT (0),
    -- ¿Posee Certificación de Registro de Educación Básica?
    -- Do you have Basic Education Registry Certification?
    -- En Proceso (3), Otorgado (4), Denegado (5)
    BasicEducationRegistry int NULL DEFAULT (0),
    -- Si la agencia tiene registro de educación básica
    ServiceTime datetime NULL,
    -- Si la agencia tiene servicio de tiempo
    TaxExemptionStatus int NULL DEFAULT (0),
    -- Si la agencia tiene exención de impuestos
    TaxExemptionType int NULL DEFAULT (0),
    -- Si la agencia tiene exención de impuestos
    RejectionJustification nvarchar(max) NULL,
    -- Si la agencia fue rechazada, se guarda la justificación
    Comments NVARCHAR(MAX) NULL,
    -- Comentarios extras para la agencia
    AppointmentCoordinated BIT NULL DEFAULT 0,
    -- Si la agencia tiene cita programada
    AppointmentDate DATETIME NULL,
    -- Fecha de la cita programada
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);
GO

ALTER TABLE AgencyInscription
ADD AppointmentCoordinated BIT NULL DEFAULT 0,
    AppointmentDate DATETIME NULL,
    Comments NVARCHAR(MAX) NULL;
GO

-- alter para agegar UpdateAt
ALTER TABLE AgencyInscription
ADD UpdatedAt DATETIME NULL;
GO

ALTER TABLE AgencyInscription
ADD CONSTRAINT FK_AgencyInscription_Agency FOREIGN KEY (AgencyId) REFERENCES Agency(Id);
GO