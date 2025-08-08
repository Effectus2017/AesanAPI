-- =============================================
-- Author:      Development Team
-- Create date: 2025-03-07
-- Description: Script para actualizar las referencias a los estados de agencia en la tabla Agency
-- =============================================

-- Actualizamos las referencias a los estados de agencia en la tabla Agency
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Agency')
BEGIN
    -- Mapeamos los estados antiguos a los nuevos
    -- Estado 1: 'Pendiente de validar' -> 1: 'Pendiente a validar'
    -- Estado 2: 'Orientación' -> 3: 'Orientación de Programa'
    -- Estado 3: 'Visita Pre-operacional' -> 2: 'Coordinar Visita Pre-Operacional'
    -- Estado 4: 'No cumple con los requisitos' -> 6: 'No cumple con los requisitos'
    -- Estado 5: 'Cumple con los requisitos' -> 5: 'Cumple con los requisitos'
    -- Estado 6: 'Rechazado' -> 6: 'No cumple con los requisitos'
    -- Estado 7: 'Aprobado' -> 5: 'Cumple con los requisitos'

    -- Actualizamos las agencias con estado 1 (Pendiente de validar)
    UPDATE Agency SET AgencyStatusId = 1 WHERE AgencyStatusId = 1;
    
    -- Actualizamos las agencias con estado 2 (Orientación)
    UPDATE Agency SET AgencyStatusId = 3 WHERE AgencyStatusId = 2;
    
    -- Actualizamos las agencias con estado 3 (Visita Pre-operacional)
    UPDATE Agency SET AgencyStatusId = 2 WHERE AgencyStatusId = 3;
    
    -- Actualizamos las agencias con estado 4 (No cumple con los requisitos)
    UPDATE Agency SET AgencyStatusId = 6 WHERE AgencyStatusId = 4;
    
    -- Actualizamos las agencias con estado 5 (Cumple con los requisitos)
    UPDATE Agency SET AgencyStatusId = 5 WHERE AgencyStatusId = 5;
    
    -- Actualizamos las agencias con estado 6 (Rechazado)
    UPDATE Agency SET AgencyStatusId = 6 WHERE AgencyStatusId = 6;
    
    -- Actualizamos las agencias con estado 7 (Aprobado)
    UPDATE Agency SET AgencyStatusId = 5 WHERE AgencyStatusId = 7;
    
    -- Actualizamos las agencias con estados inválidos
    UPDATE Agency SET AgencyStatusId = 1 WHERE AgencyStatusId NOT IN (1, 2, 3, 4, 5, 6);
    
    PRINT 'Las referencias a los estados de agencia en la tabla Agency han sido actualizadas.';
END
ELSE
BEGIN
    PRINT 'La tabla Agency no existe.';
END 