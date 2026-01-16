-- =============================================
-- Script: 116_InsertEmailLogOptions
-- Fecha: 2026-01-15
-- Descripción: Inserta opciones para Email Logs (estados y tipos de correo)
-- =============================================

-- Opciones para EmailLogStatus (Estados de correo)
-- Nota: El campo Name contiene el valor técnico que coincide con EmailLog.Status
-- Opción "Todos" para mostrar todos los estados
IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogStatus' AND Name = 'All')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('All', 'All', 'emailLogStatus', 0, 1, 0, 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogStatus' AND Name = 'Pending')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('Pending', 'Pending', 'emailLogStatus', 0, 1, 1070, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogStatus' AND Name = 'Sent')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('Sent', 'Sent', 'emailLogStatus', 0, 1, 1080, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogStatus' AND Name = 'Failed')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('Failed', 'Failed', 'emailLogStatus', 0, 1, 1090, 0);
END
GO

-- Opciones para EmailLogType (Tipos de correo)
-- Opción "Todos" para mostrar todos los tipos
IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'All')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('All', 'All', 'emailLogType', 0, 1, 0, 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'WelcomeAgency')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('WelcomeAgency', 'Welcome Agency', 'emailLogType', 0, 1, 1100, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'ApprovalSponsor')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('ApprovalSponsor', 'Approval Sponsor', 'emailLogType', 0, 1, 1110, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'DenialSponsor')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('DenialSponsor', 'Denial Sponsor', 'emailLogType', 0, 1, 1120, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'AgencyAssignment')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('AgencyAssignment', 'Agency Assignment', 'emailLogType', 0, 1, 1130, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'AgencyUnassignment')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('AgencyUnassignment', 'Agency Unassignment', 'emailLogType', 0, 1, 1140, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'PasswordChanged')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('PasswordChanged', 'Password Changed', 'emailLogType', 0, 1, 1150, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'PasswordReset')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('PasswordReset', 'Password Reset', 'emailLogType', 0, 1, 1160, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'TemporaryPassword')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('TemporaryPassword', 'Temporary Password', 'emailLogType', 0, 1, 1170, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM OptionSelection WHERE OptionKey = 'emailLogType' AND Name = 'Generic')
BEGIN
    INSERT INTO OptionSelection
        (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder, IsDefaultValue)
    VALUES
        ('Generic', 'Generic', 'emailLogType', 0, 1, 1180, 0);
END
GO
