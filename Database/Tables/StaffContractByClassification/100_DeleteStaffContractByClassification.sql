-- =============================================
-- Stored Procedure: 100_DeleteStaffContractByClassification
-- =============================================
-- Borrado físico de todas las filas de contratos por clasificación de un staff.
-- Parámetros lowercase; cuerpo con columnas CapitalCase.

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteStaffContractByClassification]
    @staffid INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM StaffContractByClassification
    WHERE StaffId = @staffid;
END
