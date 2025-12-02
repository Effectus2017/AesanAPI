-- Template para cuando un auspiciador completa su registro
INSERT INTO [dbo].[MessageTemplate]
    (TemplateKey, TitleES, TitleEN, BodyES, BodyEN, Icon, Link, UseRouter, PurposeES, PurposeEN, IsActive)
VALUES
    (
        'SponsorRegistrationCompleted',
        'Registro de Auspiciador Completado',
        'Sponsor Registration Completed',
        'El auspiciador {SponsorName} (Código: {SponsorCode}) ha completado su registro el {CompletionDate}. Por favor, revise la información.',
        'Sponsor {SponsorName} (Code: {SponsorCode}) has completed their registration on {CompletionDate}. Please review the information.',
        'heroicons_outline:check-circle',
        '/sponsor-evaluation',
        1,
        'Mensaje enviado al evaluador cuando un auspiciador completa su registro',
        'Message sent to evaluator when a sponsor completes their registration',
        1
    );
GO

-- Template para cuando un sitio es inactivado
INSERT INTO [dbo].[MessageTemplate]
    (TemplateKey, TitleES, TitleEN, BodyES, BodyEN, Icon, Link, UseRouter, PurposeES, PurposeEN, IsActive)
VALUES
    (
        'SiteInactivated',
        'Sitio Inactivado',
        'Site Inactivated',
        'El sitio {SiteName} (Código: {SiteCode}) de la agencia {AgencyName} ha sido inactivado el {InactiveDate}. Justificación: {InactiveJustification}',
        'Site {SiteName} (Code: {SiteCode}) from agency {AgencyName} has been inactivated on {InactiveDate}. Justification: {InactiveJustification}',
        'heroicons_outline:exclamation-triangle',
        '/sites',
        1,
        'Mensaje enviado al evaluador cuando un sitio es inactivado',
        'Message sent to evaluator when a site is inactivated',
        1
    );
GO

-- Template para cuando un personal es inactivado
INSERT INTO [dbo].[MessageTemplate]
    (TemplateKey, TitleES, TitleEN, BodyES, BodyEN, Icon, Link, UseRouter, PurposeES, PurposeEN, IsActive)
VALUES
    (
        'StaffInactivated',
        'Personal Inactivado',
        'Staff Inactivated',
        'El personal {StaffName} de la agencia {AgencyName} ha sido inactivado el {InactiveDate}.',
        'Staff member {StaffName} from agency {AgencyName} has been inactivated on {InactiveDate}.',
        'heroicons_outline:exclamation-triangle',
        '/staff',
        1,
        'Mensaje enviado al evaluador cuando un personal es inactivado',
        'Message sent to evaluator when a staff member is inactivated',
        1
    );
GO

