-- =============================================
-- Script: 101_UpdatePACNADisplayOrder
-- Descripción: Actualiza DisplayOrder de ServiceTypeProgram para PACNA (ProgramId=3)
--             según orden operativo: Desayuno, Merienda AM, Almuerzo, Merienda PM,
--             Cena, Cena Horario Extendido, Cena en Riesgo, Merienda Horario Extendido,
--             Merienda en Riesgo, Merienda Nocturna.
-- =============================================

-- PACNA (ProgramId = 3): nuevo DisplayOrder según orden operativo
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 1, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 1;  -- Desayuno
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 2, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 3;  -- Merienda AM
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 3, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 2;  -- Almuerzo
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 4, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 5;  -- Merienda PM
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 5, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 4;  -- Cena
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 6, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 7;  -- Cena Horario Extendido
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 7, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 8;  -- Cena en Riesgo
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 8, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 9;  -- Merienda Horario Extendido
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 9, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 10; -- Merienda en Riesgo
UPDATE [dbo].[ServiceTypeProgram] SET DisplayOrder = 10, UpdatedAt = GETDATE() WHERE ProgramId = 3 AND ServiceTypeId = 6; -- Merienda Nocturna

GO
