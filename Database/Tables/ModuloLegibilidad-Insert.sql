/*
===========================================
Datos Iniciales - Módulo de Elegibilidad
===========================================
Este script contiene los datos iniciales necesarios para el funcionamiento
del módulo de elegibilidad para comida escolar.

Versión: 1.0
Fecha: 2024-03-21
*/

-- Tipos de Solicitud
INSERT INTO ApplicationType (Name, Description) VALUES
('Regular', 'Solicitud estándar para hogares que no participan en programas de asistencia'),
('Categórica', 'Solicitud para hogares que participan en programas de asistencia como SNAP, TANF o FDPIR'),
('Directa', 'Certificación directa a través de agencias estatales');

-- Tipos de Ingreso
INSERT INTO IncomeType (Name, Description, AppliesTo) VALUES
-- Ingresos para Niños
('Trabajo', 'Ingresos del trabajo regular a tiempo completo o parcial', 'Child'),
('Seguro Social', 'Pagos de Seguro Social por discapacidad, beneficios para sobrevivientes', 'Child'),
('Gastos', 'Dinero recibido regularmente para gastos personales', 'Child'),
('Otros Ingresos', 'Cualquier otro ingreso regular recibido por el niño', 'Child'),

-- Ingresos para Adultos
('Salarios', 'Salarios, sueldos, bonos en efectivo', 'Adult'),
('Autoempleo', 'Ingresos netos de trabajo por cuenta propia', 'Adult'),
('Asistencia Pública', 'Beneficios de desempleo, compensación laboral', 'Adult'),
('Pensión Alimenticia', 'Pensión alimenticia y manutención infantil', 'Adult'),
('Jubilación', 'Seguro Social, pensiones privadas, otros ingresos de jubilación', 'Adult'),
('Otros', 'Ingresos por inversiones, rentas, regalías, etc.', 'Adult');

-- Frecuencias de Ingreso
INSERT INTO IncomeFrequency (Name, ConversionFactor) VALUES
('Semanal', 52.0),
('Quincenal', 26.0),
('Dos veces al mes', 24.0),
('Mensual', 12.0),
('Anual', 1.0);

-- Etnicidades
INSERT INTO Ethnicity (Name, Description) VALUES
('Hispano o Latino', 'Una persona de origen o cultura cubana, mexicana, puertorriqueña, sudamericana o centroamericana u otra cultura u origen español'),
('No Hispano o Latino', 'Una persona que no es de origen hispano o latino');

-- Razas
INSERT INTO Race (Name, Description) VALUES
('Indio Americano o Nativo de Alaska', 'Una persona con orígenes en cualquiera de los pueblos originales de Norte y Sudamérica'),
('Asiático', 'Una persona con orígenes en cualquiera de los pueblos originales del Lejano Oriente, Sudeste Asiático o el subcontinente indio'),
('Negro o Afroamericano', 'Una persona con orígenes en cualquiera de los grupos raciales negros de África'),
('Nativo de Hawái u Otras Islas del Pacífico', 'Una persona con orígenes en cualquiera de los pueblos originales de Hawái, Guam, Samoa u otras islas del Pacífico'),
('Blanco', 'Una persona con orígenes en cualquiera de los pueblos originales de Europa, Oriente Medio o África del Norte');

-- Tipos de Documentos para Elegibilidad
-- Solo agregar si no existen ya en DocumentTypes
INSERT INTO DocumentTypes (Name, Description)
SELECT t.Name, t.Description
FROM (VALUES
    ('Comprobante de Ingresos', 'Documentación que verifica los ingresos declarados en la solicitud de elegibilidad'),
    ('Certificado SNAP', 'Certificado vigente de participación en el Programa de Asistencia Nutricional Suplementaria'),
    ('Certificado TANF', 'Certificado vigente de participación en el programa de Asistencia Temporal para Familias Necesitadas'),
    ('Certificado FDPIR', 'Certificado vigente de participación en el Programa de Distribución de Alimentos en Reservas Indígenas'),
    ('Identificación Escolar', 'Documento de identificación o matrícula escolar del estudiante'),
    ('Comprobante de Residencia', 'Documento que verifica la dirección de residencia del hogar'),
    ('Certificado de Custodia', 'Documentación legal que verifica la custodia o tutela de niños en cuidado adoptivo')
) AS t(Name, Description)
WHERE NOT EXISTS (
    SELECT 1 
    FROM DocumentTypes 
    WHERE Name = t.Name
); 