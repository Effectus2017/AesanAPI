-- =============================================
-- Migration: Copy existing Staff contract data to StaffContractByClassification
-- =============================================
-- Para cada Staff con StaffTypeId = empleado y StaffClassificationId IN (1, 2),
-- inserta una fila en StaffContractByClassification.
-- Requiere que la tabla StaffContractByClassification exista y Mig_StaffClassificationAmbos no obligatorio para esta migración.

-- StaffTypeId = 1 se asume como "empleado" (verificar en StaffType si el Id es correcto)
INSERT INTO StaffContractByClassification
    (StaffId, StaffClassificationId, PositionId, ContractStartDate, ContractEndDate, ScheduleFrom, ScheduleTo, CreatedAt, IsActive)
SELECT
    s.Id,
    s.StaffClassificationId,
    s.PositionId,
    s.ContractStartDate,
    s.ContractEndDate,
    NULL,
    NULL,
    GETDATE(),
    1
FROM Staff s
WHERE s.StaffClassificationId IN (1, 2)
    AND s.StaffTypeId = 1
    AND NOT EXISTS (
        SELECT 1 FROM StaffContractByClassification c
        WHERE c.StaffId = s.Id AND c.StaffClassificationId = s.StaffClassificationId AND c.IsActive = 1
    );
