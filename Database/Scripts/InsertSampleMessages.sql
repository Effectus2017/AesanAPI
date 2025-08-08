-- =============================================
-- Script: InsertSampleMessages.sql
-- Descripción: Insertar datos de ejemplo para la tabla Messages
-- =============================================

-- Limpiar datos existentes (opcional)
-- DELETE FROM Messages WHERE IsDeleted = 0;

-- Insertar mensajes de ejemplo
INSERT INTO Messages
    (Icon, Image, Title, Description, Time, Link, UseRouter, [Read], UserId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
    ('heroicons_outline:user', 'assets/images/avatars/gary-peters.jpg', 'Gary Peters', 'We should talk about that at lunch!', DATEADD(HOUR, -2, GETDATE()), '/profile', 1, 0, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:document', 'assets/images/avatars/leo-gill.jpg', 'Leo Gill (Client #8817)', 'You can download the latest invoices now. Please check and let me know if you need any clarification.', DATEADD(HOUR, -3, GETDATE()), '/invoices', 1, 0, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:home', 'assets/images/avatars/sarah.jpg', 'Sarah', 'Don''t forget to pickup Jeremy after school!', DATEADD(HOUR, -5, GETDATE()), '/calendar', 1, 1, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:document-text', 'assets/images/avatars/nancy-salazar.jpg', 'Nancy Salazar • Joy Publishing', 'I''ll proof read your bio on next Monday.', DATEADD(HOUR, -8, GETDATE()), '/documents', 1, 1, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:bell', 'assets/images/avatars/system.jpg', 'System Notification', 'Your account has been successfully verified.', DATEADD(HOUR, -1, GETDATE()), '/settings', 1, 0, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:check-circle', 'assets/images/avatars/success.jpg', 'Success Alert', 'Your form has been submitted successfully.', DATEADD(MINUTE, -30, GETDATE()), '/dashboard', 1, 1, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:exclamation-triangle', 'assets/images/avatars/warning.jpg', 'Warning Notice', 'Please update your profile information.', DATEADD(HOUR, -4, GETDATE()), '/profile', 1, 0, NULL, GETDATE(), GETDATE(), 0),

    ('heroicons_outline:information-circle', 'assets/images/avatars/info.jpg', 'Information Update', 'New features are available in your dashboard.', DATEADD(HOUR, -6, GETDATE()), '/dashboard', 1, 1, NULL, GETDATE(), GETDATE(), 0);

-- Verificar la inserción
SELECT
    Id,
    Title,
    Description,
    Time,
    [Read],
    IsDeleted
FROM Messages
WHERE IsDeleted = 0
ORDER BY Time DESC; 