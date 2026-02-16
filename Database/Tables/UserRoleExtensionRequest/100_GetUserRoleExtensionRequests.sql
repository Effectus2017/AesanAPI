-- =============================================
-- Stored Procedure: 100_GetUserRoleExtensionRequests
-- Descripción: Lista solicitudes de extensión; @statusFilter NULL = todas, 'Pending' = solo pendientes.
-- Parámetros y alias: lowercase; columnas en cuerpo: CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetUserRoleExtensionRequests]
    @statusFilter NVARCHAR(50) = 'Pending',
    @take INT = 50,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id = e.Id,
        userid = e.UserId,
        roleid = e.RoleId,
        rolename = r.Name,
        requestedvalidto = e.RequestedValidTo,
        reason = e.Reason,
        status = e.Status,
        requestedat = e.RequestedAt,
        processedat = e.ProcessedAt,
        processedby = e.ProcessedBy,
        useremail = u.Email,
        username = s.FirstName + ' ' + ISNULL(s.FatherLastName, '')
    FROM UserRoleExtensionRequest e
    INNER JOIN AspNetUsers u ON u.Id = e.UserId
    INNER JOIN AspNetRoles r ON r.Id = e.RoleId
    LEFT JOIN Staff s ON s.UserId = e.UserId
    WHERE (@statusFilter IS NULL OR e.Status = @statusFilter)
    ORDER BY e.RequestedAt DESC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT total = COUNT(*)
    FROM UserRoleExtensionRequest e
    WHERE (@statusFilter IS NULL OR e.Status = @statusFilter);
END;
GO
