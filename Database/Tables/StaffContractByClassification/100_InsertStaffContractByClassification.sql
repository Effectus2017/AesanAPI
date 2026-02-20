-- =============================================
-- Stored Procedure: 100_InsertStaffContractByClassification
-- =============================================
-- Parámetros lowercase; cuerpo con columnas CapitalCase.

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaffContractByClassification]
    @staffid INT,
    @staffclassificationid INT,
    @positionid INT,
    @contractstartdate DATETIME = NULL,
    @contractenddate DATETIME = NULL,
    @schedulefrom TIME = NULL,
    @scheduleto TIME = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO StaffContractByClassification
        (StaffId, StaffClassificationId, PositionId, ContractStartDate, ContractEndDate, ScheduleFrom, ScheduleTo, CreatedAt, IsActive)
    VALUES
        (@staffid, @staffclassificationid, @positionid, @contractstartdate, @contractenddate, @schedulefrom, @scheduleto, GETDATE(), 1);

    SET @id = SCOPE_IDENTITY();
END
