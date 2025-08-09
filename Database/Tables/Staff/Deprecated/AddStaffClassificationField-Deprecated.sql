-- =============================================
-- Script: AddStaffClassificationField
-- =============================================
-- Agrega el campo StaffClassificationId a la tabla Staff
-- Este campo solo se usa cuando StaffTypeId = 1 (Empleado)

-- Agregar campo de clasificación de staff
ALTER TABLE Staff
ADD StaffClassificationId INT NULL;

-- Crear índice para el nuevo campo
CREATE INDEX IX_Staff_StaffClassificationId ON Staff(StaffClassificationId);

-- Agregar foreign key constraint
ALTER TABLE Staff
ADD CONSTRAINT FK_Staff_StaffClassification 
FOREIGN KEY (StaffClassificationId) REFERENCES StaffClassification(Id);