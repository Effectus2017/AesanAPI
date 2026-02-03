-- =============================================
-- Stored Procedure: 101_UpsertSiteChildGroupService
-- Descripción: Inserta o actualiza un slot de servicio para un grupo (sincronización desde calendario).
-- Si existe (ChildGroupId, ServiceTypeId), actualiza IsOffered, FromTime, ToTime.
-- Si no existe, inserta una nueva fila.
-- Fecha: 2026-02-02
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_UpsertSiteChildGroupService]
    @childgroupid INT,
    @servicetypeid INT,
    @isoffered BIT = 1,
    @fromtime TIME = NULL,
    @totime TIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [dbo].[SiteChildGroupService] AS t
    USING (
        SELECT
            @childgroupid AS ChildGroupId,
            @servicetypeid AS ServiceTypeId,
            @isoffered AS IsOffered,
            @fromtime AS FromTime,
            @totime AS ToTime
    ) AS s
    ON t.ChildGroupId = s.ChildGroupId AND t.ServiceTypeId = s.ServiceTypeId
    WHEN MATCHED THEN
        UPDATE SET
            IsOffered = s.IsOffered,
            FromTime = s.FromTime,
            ToTime = s.ToTime,
            UpdatedAt = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ChildGroupId, ServiceTypeId, IsOffered, FromTime, ToTime, CreatedAt)
        VALUES (s.ChildGroupId, s.ServiceTypeId, s.IsOffered, s.FromTime, s.ToTime, GETDATE());
END;
