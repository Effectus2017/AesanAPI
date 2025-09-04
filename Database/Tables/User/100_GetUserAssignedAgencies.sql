-- =============================================
-- Stored Procedure: 100_GetUserAssignedAgencies
-- =============================================
-- Obtiene las agencias asignadas a un usuario específico con paginación
-- Parámetros:
--   @userId: ID del usuario (obligatorio)
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @alls: Si es true, retorna todas las agencias sin filtrar por usuario

CREATE OR ALTER PROCEDURE [dbo].[100_GetUserAssignedAgencies]
    @userId NVARCHAR(450),
    @take INT,
    @skip INT,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que se proporcione userId
    IF @userId IS NULL
    BEGIN
        RAISERROR ('El parámetro @userId es obligatorio', 16, 1);
        RETURN;
    END

    -- Obtener resultados paginados
    SELECT
        a.Id,
        a.Name,
        a.Address,
        a.Phone,
        a.Email,
        au.IsActive,
        au.IsOwner,
        au.IsMonitor,
        a.CreatedAt,
        ISNULL(a.UpdatedAt, a.CreatedAt) AS UpdatedAt
    FROM Agency a
        INNER JOIN AgencyUsers au ON a.Id = au.AgencyId
    WHERE au.IsActive = 1
        AND (@alls = 1
        OR (au.UserId = @userId)
    )
    ORDER BY au.AssignedDate DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener total de registros para paginación
    SELECT COUNT(DISTINCT a.Id) AS TotalRecords
    FROM Agency a
        INNER JOIN AgencyUsers au ON a.Id = au.AgencyId
    WHERE au.IsActive = 1
        AND (@alls = 1
        OR (au.UserId = @userId)
    );
END;