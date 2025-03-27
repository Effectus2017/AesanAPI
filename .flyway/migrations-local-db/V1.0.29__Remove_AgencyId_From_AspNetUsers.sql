-- Primero, migrar los datos de AspNetUsers.AgencyId a AgencyUsers
INSERT INTO AgencyUsers (UserId, AgencyId, IsOwner, IsMonitor, IsActive, CreatedAt, AssignedBy)
SELECT u.Id, u.AgencyId, 1, 0, 1, GETUTCDATE(), u.Id
FROM AspNetUsers u
LEFT JOIN AgencyUsers au ON u.Id = au.UserId AND u.AgencyId = au.AgencyId
WHERE u.AgencyId IS NOT NULL AND au.Id IS NULL;

-- Verificar que todos los usuarios con AgencyId tienen una entrada en AgencyUsers
IF EXISTS (
    SELECT u.Id
    FROM AspNetUsers u
    LEFT JOIN AgencyUsers au ON u.Id = au.UserId AND u.AgencyId = au.AgencyId
    WHERE u.AgencyId IS NOT NULL AND au.Id IS NULL
)
BEGIN
    RAISERROR ('Error: No se pudieron migrar todos los datos de AgencyId a AgencyUsers', 16, 1);
    RETURN;
END

-- Si llegamos aquí, significa que la migración fue exitosa
-- Ahora podemos eliminar la restricción de clave foránea si existe
IF EXISTS (
    SELECT * 
    FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('AspNetUsers')
    AND referenced_object_id = OBJECT_ID('Agency')
)
BEGIN
    DECLARE @FKName nvarchar(1000);
    SELECT @FKName = name
    FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('AspNetUsers')
    AND referenced_object_id = OBJECT_ID('Agency');
    
    DECLARE @SQL nvarchar(1000);
    SET @SQL = 'ALTER TABLE AspNetUsers DROP CONSTRAINT ' + @FKName;
    EXEC sp_executesql @SQL;
END

-- Finalmente, eliminar la columna AgencyId
ALTER TABLE AspNetUsers DROP COLUMN AgencyId;
GO 