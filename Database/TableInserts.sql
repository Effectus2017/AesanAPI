
-- Insertar estados de la agencia
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Pendiente de validar');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Orientación');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Visita Pre-operacional');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('No cumple con los requisitos');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Cumple con los requisitos');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Rechazado');
INSERT INTO AgencyStatus
    (Name)
VALUES
    ('Aprobado');

-- Insertar la agencia AESAN por defecto
INSERT INTO Agency
    (Name, StatusId, CityId, RegionId, ProgramId, SdrNumber, UieNumber, EinNumber, Address, ZipCode, PostalAddress, Latitude, Longitude, Phone, Email, ImageURL, IsActive, IsListable)
VALUES
    ('AESAN', 5, 1, 1, 7, 123456, 123456, 123456, 'Calle Principal #123', 00123, 'Calle Principal #123', 18.220833, -66.590149, '787-123-4567', 'admin@aesan.pr.gov', NULL, 1, 1);

INSERT INTO UserProgram
    (UserId, ProgramId, CreatedAt)
VALUES
    ('87654321-4321-4321-4321-210987654321', 1, GETDATE());

INSERT INTO Program
    (Name, Description)
VALUES
    ('PDAM', 'Programa de Desayuno Escolar');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PSAV', 'Programa de Servicios de Alimentos de Verano');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PACNA', 'Programa de Alimentos para el Cuidado de Niños y Adulto');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PFHF', 'Programa de Frutas y Hortalizas Frescas');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PAF', 'Programa de Distribución de Alimentos Federales');
INSERT INTO Program
    (Name, Description)
VALUES
    ('PDFE', 'Programa de la Finca a la Escuela');
INSERT INTO Program
    (Name, Description)
VALUES
    ('AESAN', 'Agencia de Servicios de Alimentos Nutritivos');

INSERT INTO City
    (Name)
VALUES
    ('Arecibo');
INSERT INTO City
    (Name)
VALUES
    ('Bayamón');
INSERT INTO City
    (Name)
VALUES
    ('Mayagüez');
INSERT INTO City
    (Name)
VALUES
    ('Ponce');
INSERT INTO City
    (Name)
VALUES
    ('Caguas');
INSERT INTO City
    (Name)
VALUES
    ('San Juan');
INSERT INTO City
    (Name)
VALUES
    ('Humacao');

INSERT INTO Region
    (Name, CityId)
VALUES
    ('Arecibo', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aguada', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Barceloneta', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Camuy', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Ciales', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Dorado', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Florida', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Hatillo', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Lares', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Manatí', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Quebradillas', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Vega Alta', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Vega Baja', 1);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Bayamón', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cataño', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Corozal', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cabo Rojo', 2);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aguadilla', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Isabela', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Lajas', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Las Marías', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Maricao', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Mayagüez', 3);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Adjuntas', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Coamo', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guayanilla', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guánica', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Jayuya', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Ponce', 4);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aguas Buenas', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Aibonito', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Arroyo', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Barranquitas', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Caguas', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cayey', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Cidra', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guayama', 5);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Carolina', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Guaynabo', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('San Juan', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Trujillo Alto', 6);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Ceiba', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Fajardo', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Juncos', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Las Piedras', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Rio Grande', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('San Lorenzo', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Vieques', 7);
INSERT INTO Region
    (Name, CityId)
VALUES
    ('Yabucoa', 7);

--INSERT INTO Agency (AgencyStatusId, CityId, RegionId, Name, StateDepartmentRegistration, UieNumber, EinNumber, Address, PostalCode, Latitude, Longitude, Phone, CreatedAt, UpdatedAt)
--VALUES (1, 1, 1, 'Nombre de la Agencia', 'Registro del Departamento de Estado', 123456, 789012, 'Dirección de la Agencia', 'Código Postal', 37.7749, -122.4194, 'Teléfono de la Agencia', GETDATE(), NULL);
