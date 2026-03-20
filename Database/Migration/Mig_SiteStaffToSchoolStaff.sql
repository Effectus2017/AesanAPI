-- =============================================
-- Mig_SiteStaffToSchoolStaff
-- Migración de datos SiteStaff → SchoolStaff vía SchoolSite (SiteId único por escuela activa).
-- Prerrequisito: tabla SchoolStaff creada (ver Tables/SchoolStaff/SchoolStaff-Table.sql).
-- Política huérfanos: falla si existe SiteStaff sin fila activa en SchoolSite para ese SiteId.
-- =============================================

SET NOCOUNT ON;

IF OBJECT_ID('dbo.SchoolStaff', 'U') IS NULL
BEGIN
    RAISERROR ('SchoolStaff no existe. Ejecutar primero SchoolStaff-Table.sql.', 16, 1);
    RETURN;
END

IF OBJECT_ID('dbo.SiteStaff', 'U') IS NULL
BEGIN
    PRINT 'Mig_SiteStaffToSchoolStaff: SiteStaff no existe; nada que migrar.';
    RETURN;
END

IF EXISTS (SELECT 1 FROM dbo.SchoolStaff)
BEGIN
    RAISERROR ('SchoolStaff ya tiene datos. Vacíe manualmente o omita la migración.', 16, 1);
    RETURN;
END

IF EXISTS (
    SELECT 1
    FROM dbo.SiteStaff sst
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.SchoolSite sc
        WHERE sc.SiteId = sst.SiteId AND sc.IsActive = 1
    )
)
BEGIN
    RAISERROR ('SiteStaff tiene filas cuyo SiteId no tiene SchoolSite activo. Asocie el sitio a una escuela antes de migrar.', 16, 1);
    RETURN;
END

;WITH Ranked AS (
    SELECT
        schs.SchoolId,
        sst.StaffId,
        sst.AssignmentDate,
        sst.AssignmentTypeId,
        sst.IsPrimary,
        sst.StartDate,
        sst.EndDate,
        sst.Comments,
        sst.IsActive,
        sst.CreatedAt,
        sst.UpdatedAt,
        ROW_NUMBER() OVER (
            PARTITION BY sst.StaffId, schs.SchoolId
            ORDER BY sst.IsActive DESC, sst.IsPrimary DESC, sst.Id DESC
        ) AS rn
    FROM dbo.SiteStaff sst
        INNER JOIN dbo.SchoolSite schs ON schs.SiteId = sst.SiteId AND schs.IsActive = 1
)
INSERT INTO dbo.SchoolStaff (
    SchoolId, StaffId, AssignmentDate, AssignmentTypeId,
    IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt, UpdatedAt
)
SELECT
    SchoolId, StaffId, AssignmentDate, AssignmentTypeId,
    IsPrimary, StartDate, EndDate, Comments, IsActive, CreatedAt, UpdatedAt
FROM Ranked
WHERE rn = 1;

PRINT 'Mig_SiteStaffToSchoolStaff: insertados ' + CAST(@@ROWCOUNT AS VARCHAR(20)) + ' registros en SchoolStaff.';

-- Eliminar tabla SiteStaff (FKs primero)
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SiteStaff_Site')
    ALTER TABLE dbo.SiteStaff DROP CONSTRAINT FK_SiteStaff_Site;
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SiteStaff_Staff')
    ALTER TABLE dbo.SiteStaff DROP CONSTRAINT FK_SiteStaff_Staff;
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SiteStaff_AssignmentType')
    ALTER TABLE dbo.SiteStaff DROP CONSTRAINT FK_SiteStaff_AssignmentType;

DROP TABLE IF EXISTS dbo.SiteStaff;
PRINT 'Mig_SiteStaffToSchoolStaff: tabla SiteStaff eliminada.';
