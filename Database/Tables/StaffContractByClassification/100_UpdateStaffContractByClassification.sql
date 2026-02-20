-- =============================================
-- Stored Procedure: 100_UpdateStaffContractByClassification
-- =============================================
-- Actualiza por Id. Parámetros lowercase; cuerpo con columnas CapitalCase.

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaffContractByClassification]
    @id INT,
    @positionid INT,
    @contractstartdate DATETIME = NULL,
    @contractenddate DATETIME = NULL,
    @schedulefrom TIME = NULL,
    @scheduleto TIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE StaffContractByClassification
    SET
        PositionId = @positionid,
        ContractStartDate = @contractstartdate,
        ContractEndDate = @contractenddate,
        ScheduleFrom = @schedulefrom,
        ScheduleTo = @scheduleto,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END
