
-- Insertar regiones (principales)
INSERT INTO Region
    (Name, IsActive, CreatedAt)
VALUES
    ('Arecibo', 1, GETDATE()),
    ('Bayamón', 1, GETDATE()),
    ('Mayagüez', 1, GETDATE()),
    ('Ponce', 1, GETDATE()),
    ('Caguas', 1, GETDATE()),
    ('San Juan', 1, GETDATE()),
    ('Humacao', 1, GETDATE());

-- Insertar ciudades
INSERT INTO City
    (Name, IsActive, CreatedAt)
VALUES
    -- Ciudades de Arecibo (12 municipios)
    ('Arecibo', 1, GETDATE()),
    ('Barceloneta', 1, GETDATE()),
    ('Camuy', 1, GETDATE()),
    ('Ciales', 1, GETDATE()),
    ('Dorado', 1, GETDATE()),
    ('Florida', 1, GETDATE()),
    ('Hatillo', 1, GETDATE()),
    ('Lares', 1, GETDATE()),
    ('Manatí', 1, GETDATE()),
    ('Quebradillas', 1, GETDATE()),
    ('Vega Alta', 1, GETDATE()),
    ('Vega Baja', 1, GETDATE()),
    -- Ciudades de Bayamón (8 municipios)
    ('Bayamón', 1, GETDATE()),
    ('Cataño', 1, GETDATE()),
    ('Corozal', 1, GETDATE()),
    ('Morovis', 1, GETDATE()),
    ('Naranjito', 1, GETDATE()),
    ('Orocovis', 1, GETDATE()),
    ('Toa Alta', 1, GETDATE()),
    ('Toa Baja', 1, GETDATE()),
    -- Ciudades de Mayagüez (15 municipios)
    ('Aguada', 1, GETDATE()),
    ('Aguadilla', 1, GETDATE()),
    ('Añasco', 1, GETDATE()),
    ('Cabo Rojo', 1, GETDATE()),
    ('Hormigueros', 1, GETDATE()),
    ('Isabela', 1, GETDATE()),
    ('Lajas', 1, GETDATE()),
    ('Las Marías', 1, GETDATE()),
    ('Maricao', 1, GETDATE()),
    ('Mayagüez', 1, GETDATE()),
    ('Moca', 1, GETDATE()),
    ('Rincón', 1, GETDATE()),
    ('Sabana Grande', 1, GETDATE()),
    ('San Germán', 1, GETDATE()),
    ('San Sebastián', 1, GETDATE()),
    -- Ciudades de Ponce (12 municipios)
    ('Adjuntas', 1, GETDATE()),
    ('Coamo', 1, GETDATE()),
    ('Guayanilla', 1, GETDATE()),
    ('Guánica', 1, GETDATE()),
    ('Jayuya', 1, GETDATE()),
    ('Juana Díaz', 1, GETDATE()),
    ('Peñuelas', 1, GETDATE()),
    ('Ponce', 1, GETDATE()),
    ('Santa Isabel', 1, GETDATE()),
    ('Utuado', 1, GETDATE()),
    ('Villalba', 1, GETDATE()),
    ('Yauco', 1, GETDATE()),
    -- Ciudades de Caguas (11 municipios)
    ('Aguas Buenas', 1, GETDATE()),
    ('Aibonito', 1, GETDATE()),
    ('Arroyo', 1, GETDATE()),
    ('Barranquitas', 1, GETDATE()),
    ('Caguas', 1, GETDATE()),
    ('Cayey', 1, GETDATE()),
    ('Cidra', 1, GETDATE()),
    ('Comerío', 1, GETDATE()),
    ('Guayama', 1, GETDATE()),
    ('Gurabo', 1, GETDATE()),
    ('Salinas', 1, GETDATE()),
    -- Ciudades de San Juan (4 municipios)
    ('Carolina', 1, GETDATE()),
    ('Guaynabo', 1, GETDATE()),
    ('San Juan', 1, GETDATE()),
    ('Trujillo Alto', 1, GETDATE()),
    -- Ciudades de Humacao (16 municipios)
    ('Canóvanas', 1, GETDATE()),
    ('Ceiba', 1, GETDATE()),
    ('Culebra', 1, GETDATE()),
    ('Fajardo', 1, GETDATE()),
    ('Humacao', 1, GETDATE()),
    ('Juncos', 1, GETDATE()),
    ('Las Piedras', 1, GETDATE()),
    ('Loiza', 1, GETDATE()),
    ('Luquillo', 1, GETDATE()),
    ('Maunabo', 1, GETDATE()),
    ('Naguabo', 1, GETDATE()),
    ('Patillas', 1, GETDATE()),
    ('Rio Grande', 1, GETDATE()),
    ('San Lorenzo', 1, GETDATE()),
    ('Vieques', 1, GETDATE()),
    ('Yabucoa', 1, GETDATE());

-- Insertar relaciones Ciudad-Región
INSERT INTO CityRegion
    (CityId, RegionId, IsActive, CreatedAt)
SELECT c.Id, r.Id, 1, GETDATE()
FROM City c
    CROSS APPLY (
    SELECT r.Id
    FROM Region r
    WHERE 
        (r.Name = 'Arecibo' AND c.Id BETWEEN 1 AND 12) OR
        (r.Name = 'Bayamón' AND c.Id BETWEEN 13 AND 20) OR
        (r.Name = 'Mayagüez' AND c.Id BETWEEN 21 AND 35) OR
        (r.Name = 'Ponce' AND c.Id BETWEEN 36 AND 47) OR
        (r.Name = 'Caguas' AND c.Id BETWEEN 48 AND 58) OR
        (r.Name = 'San Juan' AND c.Id BETWEEN 59 AND 62) OR
        (r.Name = 'Humacao' AND c.Id BETWEEN 63 AND 78) -- Actualizado para 16 ciudades
) r;