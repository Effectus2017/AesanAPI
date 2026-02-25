-- Permiso para ver historial de estados de agencia.
MERGE INTO [dbo].[Permission] AS t
USING (VALUES
    ('agencystatushistory.view', N'Ver historial de estados de agencia', N'View agency status history', 1)
) AS s (ValueKey, Name, NameEn, IsActive)
ON t.ValueKey = s.ValueKey
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), s.ValueKey, s.Name, s.NameEn, s.IsActive, GETDATE(), GETDATE());
GO
