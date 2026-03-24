-- =============================================
-- Permiso: acceso al reporte semanal (reporte-semanal).
-- Ejecutar una vez; MERGE ignora si ValueKey ya existe.
-- =============================================

MERGE INTO [dbo].[Permission] AS t
USING (VALUES
    ('report.weekly', N'Reporte semanal', N'Weekly report', 1)
) AS s (ValueKey, Name, NameEn, IsActive)
ON t.ValueKey = s.ValueKey
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), s.ValueKey, s.Name, s.NameEn, s.IsActive, GETDATE(), GETDATE());
GO
