-- Insertar relaciones entre AreaType y City
-- Basado en las reglas de negocio:
-- Rural: Coamo, Culebra, Jayuya, Maricao, Salinas, Santa Isabel, Vieques
-- Urbana: Todas las demás ciudades

-- Obtener IDs de AreaType
DECLARE @AreaTypeRural INT = (SELECT Id
FROM AreaType
WHERE Name = 'Rural');
DECLARE @AreaTypeUrbana INT = (SELECT Id
FROM AreaType
WHERE Name = 'Urbana');

-- Obtener IDs de ciudades Rurales
DECLARE @CityCoamo INT = (SELECT Id
FROM City
WHERE Name = 'Coamo');
DECLARE @CityCulebra INT = (SELECT Id
FROM City
WHERE Name = 'Culebra');
DECLARE @CityJayuya INT = (SELECT Id
FROM City
WHERE Name = 'Jayuya');
DECLARE @CityMaricao INT = (SELECT Id
FROM City
WHERE Name = 'Maricao');
DECLARE @CitySalinas INT = (SELECT Id
FROM City
WHERE Name = 'Salinas');
DECLARE @CitySantaIsabel INT = (SELECT Id
FROM City
WHERE Name = 'Santa Isabel');
DECLARE @CityVieques INT = (SELECT Id
FROM City
WHERE Name = 'Vieques');

-- Insertar relaciones para ciudades Rurales
IF @AreaTypeRural IS NOT NULL AND @CityCoamo IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CityCoamo);
END

IF @AreaTypeRural IS NOT NULL AND @CityCulebra IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CityCulebra);
END

IF @AreaTypeRural IS NOT NULL AND @CityJayuya IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CityJayuya);
END

IF @AreaTypeRural IS NOT NULL AND @CityMaricao IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CityMaricao);
END

IF @AreaTypeRural IS NOT NULL AND @CitySalinas IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CitySalinas);
END

IF @AreaTypeRural IS NOT NULL AND @CitySantaIsabel IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CitySantaIsabel);
END

IF @AreaTypeRural IS NOT NULL AND @CityVieques IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    VALUES
        (@AreaTypeRural, @CityVieques);
END

-- Insertar relaciones para TODAS las demás ciudades como Urbanas
-- Esto incluye todas las ciudades que NO están en la lista de rurales
IF @AreaTypeUrbana IS NOT NULL
BEGIN
    INSERT INTO AreaTypeCity
        (AreaTypeId, CityId)
    SELECT @AreaTypeUrbana, c.Id
    FROM City c
    WHERE c.Name NOT IN ('Coamo', 'Culebra', 'Jayuya', 'Maricao', 'Salinas', 'Santa Isabel', 'Vieques')
        AND c.IsActive = 1;
END
