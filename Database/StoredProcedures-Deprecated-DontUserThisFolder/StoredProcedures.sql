CREATE OR ALTER PROCEDURE [dbo].[100_GetAllProgramInscriptions]
    @take INT,
    @skip INT,
    @agencyId INT = NULL,
    @programId INT = NULL,
    @userId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @totalRecords INT;

    WITH
        CTE_Results
        AS
        (
            SELECT
                pi.Id,
                pi.AgencyId,
                a.Name AS AgencyName,
                pi.ProgramId,
                p.Name AS ProgramName,
                pi.StatusId,
                s.Name AS StatusName,
                pi.Comment,
                pi.AppointmentCoordinated,
                pi.AppointmentDate,
                pi.CreatedAt,
                pi.UpdatedAt
            FROM ProgramInscription pi
                INNER JOIN Agency a ON pi.AgencyId = a.Id
                INNER JOIN Program p ON pi.ProgramId = p.Id
                INNER JOIN Status s ON pi.StatusId = s.Id
                LEFT JOIN UserAgencyAssignment uaa ON a.Id = uaa.AgencyId AND (@userId IS NULL OR uaa.UserId = @userId)
            WHERE 
            (@agencyId IS NULL OR pi.AgencyId = @agencyId)
                AND (@programId IS NULL OR pi.ProgramId = @programId)
                AND (@userId IS NULL OR uaa.IsActive = 1)
        )
    SELECT @totalRecords = COUNT(*)
    FROM CTE_Results;

    SELECT
        *,
        @totalRecords AS TotalRecords
    FROM CTE_Results
    ORDER BY CreatedAt DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;
END;

GO

-- Stored Procedure para asignar una agencia a un usuario
CREATE OR ALTER PROCEDURE [dbo].[100_AssignAgencyToUser]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1
    FROM UserAgencyAssignment
    WHERE UserId = @userId AND AgencyId = @agencyId)
    BEGIN
        INSERT INTO UserAgencyAssignment
            (UserId, AgencyId, AssignedBy)
        VALUES
            (@userId, @agencyId, @assignedBy);

        SELECT 'Agency assigned successfully' AS Message;
    END
    ELSE
    BEGIN
        UPDATE UserAgencyAssignment
        SET IsActive = 1,
            AssignedDate = GETDATE(),
            AssignedBy = @assignedBy
        WHERE UserId = @userId AND AgencyId = @agencyId;

        SELECT 'Agency assignment updated successfully' AS Message;
    END
END;

GO

-- Stored Procedure para desasignar una agencia de un usuario
CREATE OR ALTER PROCEDURE [dbo].[100_UnassignAgencyFromUser]
    @userId NVARCHAR(450),
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE UserAgencyAssignment
    SET IsActive = 0
    WHERE UserId = @userId AND AgencyId = @agencyId;

    SELECT 'Agency unassigned successfully' AS Message;
END;

GO

