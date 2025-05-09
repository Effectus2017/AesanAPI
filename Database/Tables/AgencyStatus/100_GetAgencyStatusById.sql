-- 100_GetAgencyStatusById.sql
-- Obtiene un estado de agencia por id
-- Cumple convención: los SPs van en la raíz de la carpeta de la tabla
-- No usar subcarpeta SP
-- Última actualización: 2024-06-10
CREATE OR ALTER PROCEDURE [100_GetAgencyStatusById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           NameEN,
           IsActive,
           CreatedAt,
           UpdatedAt,
           DisplayOrder
    FROM AgencyStatus
    WHERE Id = @id;
END; 