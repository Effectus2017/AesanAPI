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
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);
GO