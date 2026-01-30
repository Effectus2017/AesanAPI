-- =============================================
-- Stored Procedure: 100_InsertSiteChildGroupService
-- Descripción: Inserta un slot de servicio para un grupo
-- Fecha: 2026-01-27
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteChildGroupService]
    @childgroupid INT,
    @servicetypeid INT,
    @isoffered BIT = 0,
    @fromtime TIME = NULL,
    @totime TIME = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteChildGroupService
        (ChildGroupId, ServiceTypeId, IsOffered, FromTime, ToTime, CreatedAt)
    VALUES
        (@childgroupid, @servicetypeid, @isoffered, @fromtime, @totime, GETDATE());

    SET @id = SCOPE_IDENTITY();
END;
