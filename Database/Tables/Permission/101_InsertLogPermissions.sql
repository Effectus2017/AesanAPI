-- =============================================
-- Insertar permisos del centro de logs (sistema central de logging).
-- Ejecutar una vez; los INSERT ignoran si ValueKey ya existe (MERGE).
-- Permisos: log.view.audit, log.view.email, log.view.job, log.view.errors
-- =============================================

MERGE INTO [dbo].[Permission] AS t
USING (VALUES
    ('log.view.audit', N'Ver logs de auditoría', N'View audit logs', 1),
    ('log.view.email', N'Ver logs de correo', N'View email logs', 1),
    ('log.view.job', N'Ver logs de jobs', N'View job logs', 1),
    ('log.view.errors', N'Ver logs de errores de aplicación', N'View application error logs', 1)
) AS s (ValueKey, Name, NameEn, IsActive)
ON t.ValueKey = s.ValueKey
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), s.ValueKey, s.Name, s.NameEn, s.IsActive, GETDATE(), GETDATE());
GO
