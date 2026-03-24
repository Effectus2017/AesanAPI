-- =============================================
-- SP: 100_UpdateSiteVisit
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteVisit]
    @id INT,
    @agencyid INT,
    @siteid INT,
    @visittypeid INT,
    @visitdate DATE,
    @starttime TIME,
    @endtime TIME,
    @comments NVARCHAR(1000) = NULL,
    @userid NVARCHAR(255) = NULL,
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @rowsAffected = 0;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[SiteVisit] sv
        INNER JOIN [dbo].[Site] s ON s.Id = sv.SiteId
        WHERE sv.Id = @id AND sv.IsDeleted = 0 AND s.AgencyId = @agencyid AND s.Id = @siteid
    )
    BEGIN
        RAISERROR('La visita no existe o no pertenece a la agencia/sitio indicados.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[VisitType] vt WHERE vt.Id = @visittypeid AND vt.IsActive = 1
    )
    BEGIN
        RAISERROR('El tipo de visita no es válido o está inactivo.', 16, 1);
        RETURN;
    END;

    UPDATE [dbo].[SiteVisit]
    SET
        [VisitTypeId] = @visittypeid,
        [VisitDate] = @visitdate,
        [StartTime] = @starttime,
        [EndTime] = @endtime,
        [Comments] = @comments,
        [UpdatedAt] = GETDATE(),
        [UpdatedBy] = @userid
    WHERE [Id] = @id AND [IsDeleted] = 0;

    SET @rowsAffected = @@ROWCOUNT;
END
GO
