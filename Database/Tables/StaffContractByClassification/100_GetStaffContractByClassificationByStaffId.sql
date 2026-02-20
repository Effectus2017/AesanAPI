-- =============================================
-- Stored Procedure: 100_GetStaffContractByClassificationByStaffId
-- =============================================
-- Parámetros y alias en lowercase; cuerpo con columnas CapitalCase.

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffContractByClassificationByStaffId]
    @staffid INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id = c.Id,
        staffid = c.StaffId,
        staffclassificationid = c.StaffClassificationId,
        sc.Name AS staffclassificationname,
        sc.NameEn AS staffclassificationnameen,
        positionid = c.PositionId,
        os.Name AS positionname,
        os.NameEN AS positionnameen,
        contractstartdate = c.ContractStartDate,
        contractenddate = c.ContractEndDate,
        schedulefrom = c.ScheduleFrom,
        scheduleto = c.ScheduleTo,
        createdat = c.CreatedAt,
        updatedat = c.UpdatedAt,
        isactive = c.IsActive
    FROM StaffContractByClassification c
        INNER JOIN StaffClassification sc ON c.StaffClassificationId = sc.Id
        INNER JOIN OptionSelection os ON c.PositionId = os.Id
    WHERE c.StaffId = @staffid
        AND c.IsActive = 1
    ORDER BY c.StaffClassificationId;
END
