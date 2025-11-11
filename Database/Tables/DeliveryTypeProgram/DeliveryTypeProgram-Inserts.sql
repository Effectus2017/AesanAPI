-- Insertar relaciones entre DeliveryType y Program
-- Basado en las reglas de negocio:
-- PDAM (Id=1): Auspiciador Entrega, Centro Comunal, Domicilio, Recogido de Padre o Encargado, Servi-Carro, Servi-Expreso, Sitio Recoge, N/A
-- PSAV (Id=2): Auspiciador Entrega, Centro Comunal, Domicilio, Recogido, Servi-Carro, Servi-Expreso, N/A
-- PACNA (Id=3): Auspiciador Entrega, Centro Comunal, Domicilio, Recogido de Padre o Encargado, Servi-Carro, Servi-Expreso, N/A

-- Obtener IDs de Program
DECLARE @ProgramPDAM INT = 1; -- PDAM
DECLARE @ProgramPSAV INT = 2; -- PSAV
DECLARE @ProgramPACNA INT = 3; -- PACNA

-- Obtener IDs de DeliveryType
DECLARE @DeliveryTypeNA INT = (SELECT Id FROM DeliveryType WHERE Name = 'N/A');
DECLARE @DeliveryTypeSitioRecoge INT = (SELECT Id FROM DeliveryType WHERE Name = 'Sitio Recoge');
DECLARE @DeliveryTypeAuspiciadorEntrega INT = (SELECT Id FROM DeliveryType WHERE Name = 'Auspiciador Entrega');
DECLARE @DeliveryTypeCentroComunal INT = (SELECT Id FROM DeliveryType WHERE Name = 'Centro Comunal');
DECLARE @DeliveryTypeDomicilio INT = (SELECT Id FROM DeliveryType WHERE Name = 'Domicilio');
DECLARE @DeliveryTypeRecogidoPadreEncargado INT = (SELECT Id FROM DeliveryType WHERE Name = 'Recogido de Padre o Encargado');
DECLARE @DeliveryTypeRecogido INT = (SELECT Id FROM DeliveryType WHERE Name = 'Recogido');
DECLARE @DeliveryTypeServiCarro INT = (SELECT Id FROM DeliveryType WHERE Name = 'Servi-Carro');
DECLARE @DeliveryTypeServiExpreso INT = (SELECT Id FROM DeliveryType WHERE Name = 'Servi-Expreso');

-- Insertar relaciones para PDAM (Id=1)
IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeNA IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeNA, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeSitioRecoge IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeSitioRecoge, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeAuspiciadorEntrega IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeAuspiciadorEntrega, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeCentroComunal IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeCentroComunal, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeDomicilio, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeRecogidoPadreEncargado IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeRecogidoPadreEncargado, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeServiCarro IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeServiCarro, @ProgramPDAM);
END

IF @ProgramPDAM IS NOT NULL AND @DeliveryTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeServiExpreso, @ProgramPDAM);
END

-- Insertar relaciones para PSAV (Id=2)
IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeNA IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeNA, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeAuspiciadorEntrega IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeAuspiciadorEntrega, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeCentroComunal IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeCentroComunal, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeDomicilio, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeRecogido IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeRecogido, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeServiCarro IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeServiCarro, @ProgramPSAV);
END

IF @ProgramPSAV IS NOT NULL AND @DeliveryTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeServiExpreso, @ProgramPSAV);
END

-- Insertar relaciones para PACNA (Id=3)
IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeNA IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeNA, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeAuspiciadorEntrega IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeAuspiciadorEntrega, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeCentroComunal IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeCentroComunal, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeDomicilio IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeDomicilio, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeRecogidoPadreEncargado IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeRecogidoPadreEncargado, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeServiCarro IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeServiCarro, @ProgramPACNA);
END

IF @ProgramPACNA IS NOT NULL AND @DeliveryTypeServiExpreso IS NOT NULL
BEGIN
    INSERT INTO DeliveryTypeProgram (DeliveryTypeId, ProgramId)
    VALUES (@DeliveryTypeServiExpreso, @ProgramPACNA);
END

