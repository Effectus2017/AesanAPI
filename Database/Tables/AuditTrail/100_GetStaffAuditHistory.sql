-- Stored Procedure para obtener historial de auditoría de Staff
-- Versión: 1.0.0
CREATE OR ALTER PROCEDURE [100_GetStaffAuditHistory]
    @StaffId INT,
    @Limit INT = 100
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Limit)
        at.Id,
        at.OperationId,
        at.Action,
        at.ChangedBy,
        at.ChangedAt,
        at.OldValues,
        at.NewValues,
        at.ChangedFields,
        at.Reason,
        at.BusinessContext,
        at.Tags,
        aos.Description as OperationDescription,
        aos.Status as OperationStatus,
        -- Información del usuario que hizo el cambio
        s.FirstName + ' ' + s.FatherLastName as UserFullName,
        u.Email as UserEmail
    FROM AuditTrail at
        LEFT JOIN AuditOperationSummary aos ON at.OperationId = aos.OperationId
        LEFT JOIN AspNetUsers u ON at.ChangedBy = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
    WHERE at.TableName = 'Staff'
        AND at.EntityId = CAST(@StaffId AS NVARCHAR(100))
    ORDER BY at.ChangedAt DESC;
END;
GO
