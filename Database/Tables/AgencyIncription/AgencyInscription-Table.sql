
-- Esta tabla se encarga de almacenar los datos de la solicutud de participación de la agencia (sponsor), no es lo mismo que la solicutud a un programa

CREATE TABLE AgencyInscription
(
    Id int NOT NULL IDENTITY(1,1),
    AgencyId int NOT NULL,
    NonProfit bit NULL DEFAULT (0), -- Si es una organización sin fines de lucro
    FederalFundsDenied bit NULL DEFAULT (0), -- Si la agencia no acepta fondos federales
    StateFundsDenied bit NULL DEFAULT (0), -- Si la agencia no acepta fondos estatales
    OrganizedAthleticPrograms bit NULL DEFAULT (0), -- Si la agencia tiene programas de atletismo organizados
    AtRiskService bit NULL DEFAULT (0), -- Si la agencia ofrece servicios a personas en riesgo
    BasicEducationRegistry int NULL DEFAULT (0), -- Si la agencia tiene registro de educación básica
    ServiceTime datetime NULL, -- Si la agencia tiene servicio de tiempo
    TaxExemptionStatus int NULL DEFAULT (0), -- Si la agencia tiene exención de impuestos
    TaxExemptionType int NULL DEFAULT (0), -- Si la agencia tiene exención de impuestos
    RejectionJustification nvarchar(max) NULL, -- Si la agencia fue rechazada, se guarda la justificación
    Comments NVARCHAR(MAX) NULL, -- Comentarios extras para la agencia
    AppointmentCoordinated BIT NULL DEFAULT 0, -- Si la agencia tiene cita programada
    AppointmentDate DATETIME NULL, -- Fecha de la cita programada
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