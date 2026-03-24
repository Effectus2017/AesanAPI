-- =============================================
-- SP: 100_InsertSiteVisit
-- Valida sitio ∈ agencia y tipo activo.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteVisit]
    @agencyid INT,
    @siteid INT,
    @visittypeid INT,
    @visitdate DATE,
    @starttime TIME,
    @endtime TIME,
    @comments NVARCHAR(1000) = NULL,
    @userid NVARCHAR(255) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[Site] s
        WHERE s.Id = @siteid AND s.AgencyId = @agencyid AND s.IsActive = 1
    )
    BEGIN
        RAISERROR('El sitio no pertenece a la agencia o no está activo.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[VisitType] vt WHERE vt.Id = @visittypeid AND vt.IsActive = 1
    )
    BEGIN
        RAISERROR('El tipo de visita no es válido o está inactivo.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[SiteVisit] (
        [SiteId], [VisitTypeId], [VisitDate], [StartTime], [EndTime], [Comments],
        [CreatedBy], [UpdatedBy]
    )
    VALUES (
        @siteid, @visittypeid, @visitdate, @starttime, @endtime, @comments,
        @userid, @userid
    );

    SET @id = SCOPE_IDENTITY();
END
GO
