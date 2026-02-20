-- =============================================
-- Stored Procedure: 100_DeleteStaffContractByClassificationByStaffId
-- =============================================
-- Borrado físico de todas las filas de contratos por clasificación de un staff.
-- Se usa antes de reinsertar al guardar (Insert/Update staff).

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteStaffContractByClassificationByStaffId]
    @staffid INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM StaffContractByClassification
    WHERE StaffId = @staffid;
END
