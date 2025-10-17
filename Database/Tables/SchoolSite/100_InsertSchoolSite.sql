-- =============================================
-- Stored Procedure: 100_InsertSchoolSite
-- Descripción: Asigna un Site a una School
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchoolSite]
    @schoolId INT,
    @siteId INT,
    @comment NVARCHAR(500) = NULL,
    @isActive BIT = 1,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Verificar que la School existe y está activa
        IF NOT EXISTS (SELECT 1
    FROM [School]
    WHERE [Id] = @schoolId AND [IsActive] = 1)
        BEGIN
        RAISERROR('La escuela con ID %d no existe o no está activa', 16, 1, @schoolId);
        RETURN;
    END
        
        -- Verificar que el Site existe y está activo
        IF NOT EXISTS (SELECT 1
    FROM [Site]
    WHERE [Id] = @siteId AND [IsActive] = 1)
        BEGIN
        RAISERROR('El sitio con ID %d no existe o no está activo', 16, 1, @siteId);
        RETURN;
    END
        
        -- Verificar que el Site no esté ya asignado a otra School
        IF EXISTS (SELECT 1
    FROM [SchoolSite]
    WHERE [SiteId] = @siteId AND [IsActive] = 1)
        BEGIN
        RAISERROR('El sitio ya está asignado a otra escuela', 16, 1);
        RETURN;
    END
        
        -- Insertar nueva asignación
        INSERT INTO [SchoolSite]
        (
        [SchoolId],
        [SiteId],
        [AssignmentDate],
        [Comment],
        [IsActive],
        [CreatedAt]
        )
    VALUES
        (
            @schoolId,
            @siteId,
            GETDATE(),
            @comment,
            @isActive,
            GETDATE()
        );
        
        -- Actualizar el SiteCode del Site usando SchoolCode-SiteNumber
        UPDATE site
        SET [SiteCode] = sch.[SchoolCode] + '-' + CAST(site.[SiteNumber] AS VARCHAR(10))
        FROM [Site] site
        INNER JOIN [School] sch ON sch.[Id] = @schoolId
        WHERE site.[Id] = @siteId;
        
        SET @id = SCOPE_IDENTITY();
        SET @rowsAffected = @@ROWCOUNT;
        
        COMMIT TRANSACTION;
        
        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
