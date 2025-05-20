-- =============================================
-- Tabla: HouseholdMemberIncome (Ingresos de Miembros)
-- =============================================
-- Registra los ingresos de cada miembro del hogar.
-- Permite múltiples ingresos por miembro con diferentes tipos y frecuencias.
CREATE TABLE HouseholdMemberIncome
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental
    MemberId INT NOT NULL,
    -- ID del miembro del hogar
    IncomeTypeId INT NOT NULL,
    -- Tipo de ingreso
    Amount DECIMAL(10,2) NOT NULL,
    -- Monto del ingreso
    FrequencyId INT NOT NULL,
    -- Frecuencia con que se recibe
    IsActive BIT NOT NULL DEFAULT 1,
    -- Indica si el registro está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,
    -- Fecha y hora de última actualización
    FOREIGN KEY (MemberId) REFERENCES HouseholdMember(Id),
    FOREIGN KEY (IncomeTypeId) REFERENCES IncomeType(Id),
    FOREIGN KEY (FrequencyId) REFERENCES IncomeFrequency(Id)
);