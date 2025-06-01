/*
===========================================
Tabla: OptionSelection (Selección de Opciones)
===========================================
Catálogo estandarizado de opciones para selección en formularios.
Proporciona valores consistentes para campos como Sí/No/N/A, estados y otros.

Versión: 1.0
Fecha: 2025-01-01

Propósito:
- Tabla de referencia para opciones estandarizadas
- Valores consistentes para selecciones comunes
- Soporte multilingüe (español/inglés)

Características:
- Agrupación lógica por OptionKey
- Control de orden de visualización (DisplayOrder)
- Gestión de estado activo/inactivo (IsActive)
- Marcas de tiempo para auditoría (CreatedAt, UpdatedAt)

Estructura:
- Id: Identificador único
- Name/Nombre: Texto en español
- NameEN: Texto en inglés  
- OptionKey: Clave de agrupación
- IsActive: Estado activo/inactivo
- BooleanValue: Valor booleano para yesNo
- DisplayOrder: Orden de visualización
- CreatedAt/UpdatedAt: Auditoría

Relaciones:
- Referenciada por múltiples tablas
- Usada en formularios UI

Ejemplos de uso:
- Atributos booleanos (isStudent, isFoster)
- Estados de procesos (En Proceso, Otorgado)
- Opciones Sí/No/N/A
*/

CREATE TABLE OptionSelection
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL,
    OptionKey NVARCHAR(255) NOT NULL,
    BooleanValue BIT NOT NULL DEFAULT 0,
    -- Solo para yesNo
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);
-- Datos iniciales actualizados
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Si', 'Yes', 'yesNo', 1, 1, 10),
    ('No', 'No', 'yesNo', 0, 1, 20),
    ('En Proceso', 'In Progress', 'exceptionStatus', 0, 1, 30),
    ('Otorgado', 'Granted', 'exceptionStatus', 0, 1, 40),
    ('Denegado', 'Denied', 'exceptionStatus', 0, 1, 50),
    ('Semanal', 'Weekly', 'incomeFrequency', 0, 1, 60),
    ('Quincenal', 'Bi-weekly', 'incomeFrequency', 0, 1, 70),
    ('Bi semestral', 'Bi-semestral', 'incomeFrequency', 0, 1, 80),
    ('Mensual', 'Monthly', 'incomeFrequency', 0, 1, 90),
    ('Anual', 'Annual', 'incomeFrequency', 0, 1, 100),
    ('Estatal', 'State', 'taxExemptionType', 0, 1, 110),
    ('Federal', 'Federal', 'taxExemptionType', 0, 1, 120),
    ('Gobierno', 'Government', 'typeOfEntity', 0, 1, 130),
    ('Privado', 'Private', 'typeOfEntity', 0, 1, 140),
    ('Laico', 'Laic', 'typeOfApplicant', 0, 1, 150),
    ('Base de fe', 'Faith-based', 'typeOfApplicant', 0, 1, 160),
    ('Socio-Económico', 'Socio-Economic', 'typeOfEntity', 0, 1, 170),
    ('Híbrido', 'Hybrid', 'typeOfEntity', 0, 1, 180);

INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Socio-Económico', 'Socio-Economic', 'publicAllianceContract', 0, 1, 170),
    ('Híbrido', 'Hybrid', 'publicAllianceContract', 0, 1, 180);