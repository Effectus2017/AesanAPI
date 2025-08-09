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

-- Tipos de Institución Infantil Residencial (RCCI) = Pernoctan o No Pernoctan=Requerido
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Pernoctan', 'Residential', 'typeOfResidential', 0, 1, 190),
    ('No Pernoctan', 'Non-Residential', 'typeOfResidential', 0, 1, 200);




INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Activo', 'Active', 'isActive', 1, 1, 210),
    ('Inactivo', 'Inactive', 'isActive', 0, 1, 220);


-- Tipo de área
-- Rural (23), Urbana (24)
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Rural', 'Rural', 'typeOfArea', 0, 1, 230),
    ('Urbana', 'Urban', 'typeOfArea', 0, 1, 240);


-- Puestos para Empleados Administrativos (optionKey = 'administrativePosition')
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('administrativePosition', 'Administrador', 'Administrator', 350, 1, GETDATE()),
    ('administrativePosition', 'Auxiliar Administrativo', 'Administrative Assistant', 360, 1, GETDATE()),
    ('administrativePosition', 'Contable', 'Accountant', 370, 1, GETDATE()),
    ('administrativePosition', 'Director', 'Director', 380, 1, GETDATE());

-- Puestos para Empleados Operacionales (optionKey = 'operationalPosition')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('operationalPosition', 'Ayudante de Cocina', 'Kitchen Assistant', 400, 1, GETDATE()),
    ('operationalPosition', 'Cocinero(a)', 'Cook', 410, 1, GETDATE()),
    ('operationalPosition', 'Chef', 'Chef', 420, 1, GETDATE()),
    ('operationalPosition', 'Chofer', 'Driver', 430, 1, GETDATE()),
    ('operationalPosition', 'Encargado(a) de Cocina', 'Kitchen Manager', 440, 1, GETDATE());

-- Títulos para Miembros de Junta (optionKey = 'boardMemberTitle')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('boardMemberTitle', 'Ayudante', 'Assistant', 500, 1, GETDATE()),
    ('boardMemberTitle', 'Presidente', 'President', 510, 1, GETDATE()),
    ('boardMemberTitle', 'Secretario(a)', 'Secretary', 520, 1, GETDATE()),
    ('boardMemberTitle', 'Tesorero', 'Treasurer', 530, 1, GETDATE());


-- Comunidad
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Comunidad especial/bolsillo de pobreza', 'Special Community/Poverty Pocket', 'community', 0, 1, 540),
    ('Vivienda Pública', 'Public Housing', 'community', 0, 1, 550),
    ('Desarrollo Rural', 'Rural Development', 'community', 0, 1, 560),
    ('Housing urban DevelopmentResidencial', 'Housing urban DevelopmentResidential', 'community', 0, 1, 570);

-- Caminantes / Walkers
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Niños caminantes', 'Homeless Children', 'walkers', 0, 1, 580),
    ('Sin hogar', 'Homeless', 'walkers', 0, 1, 590);

-- Servicios que operara y horario
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Desayuno', 'Breakfast', 'services', 0, 1, 600),
    ('Almuerzo', 'Lunch', 'services', 0, 1, 610),
    ('Merienda', 'Snack', 'services', 0, 1, 620),
    ('Cena', 'Dinner', 'services', 0, 1, 630),
    ('Merienda Nocturna', 'Night Snack', 'services', 0, 1, 640);

-- Tipo de distribución
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Diario', 'Daily', 'distributionType', 0, 1, 650),
    ('Agranel', 'Bulk', 'distributionType', 0, 1, 660);

-- Tipo de Sitio
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Abierto', 'Open', 'siteType', 0, 1, 670),
    ('Campamento Residencial', 'Residential Camp', 'siteType', 0, 1, 680),
    ('Campamento No Residencial', 'Non-Residential Camp', 'siteType', 0, 1, 690),
    ('Cerrado', 'Closed', 'siteType', 0, 1, 700),
    ('Restringido', 'Restricted', 'siteType', 0, 1, 710);

-- Experiencia
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Hallazgo significativo', 'Significant Finding', 'experience', 0, 1, 720),
    ('No participo año anterior', 'Did not participate last year', 'experience', 0, 1, 730),
    ('Participó el año anterior', 'Participated last year', 'experience', 0, 1, 740),
    ('Nuevo', 'New', 'experience', 0, 1, 750);

-- Resultado de Revision
INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Aprobado', 'Approved', 'reviewResult', 0, 1, 760),
    ('Rechazado', 'Rejected', 'reviewResult', 0, 1, 770);