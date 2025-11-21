/*
===========================================
Script de Inserción: EmailTemplate
===========================================
Inserta los templates iniciales de correo electrónico basados en los templates
actuales del EmailService.cs, convertidos a formato con placeholders.

Versión: 1.0
Fecha: 2025-01-15

Placeholders disponibles:
- {FullName} - Nombre completo del usuario
- {Email} - Correo electrónico del usuario
- {TemporaryPassword} - Contraseña temporera
- {WebUrl} - URL de la plataforma web
- {AgencyName} - Nombre de la agencia
- {AgencyCode} - Código de la agencia
- {ResetLink} - Enlace de restablecimiento de contraseña
- {RejectionReason} - Razón de rechazo
- {NewPassword} - Nueva contraseña
*/

-- 1. WelcomeAgency - Email de bienvenida a agencias
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'WelcomeAgency',
        '¡Gracias por su interés en formar parte del programa de AESAN!',
        'Thank you for your interest in being part of the AESAN program!',
        '<div style=''font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;''>
            <p>Estimado/a {FullName},</p>
            
            <p>Nos complace enormemente saber que está interesado/a en formar parte de nuestros programas en AESAN. 
            Su apoyo y participación son fundamentales para continuar con nuestra misión de ofrecer servicios de alimentos 
            y educación nutricional a los participantes, cubriendo todas las edades desde la niñez hasta la vejez.</p>
            
            <p>Para continuar con su proceso de registro, validación y aprobación final, le pedimos que siga los siguientes pasos:</p>
            
            <ul>
                <li>Haz clic en el siguiente enlace para acceder a la plataforma <strong>NUTRE</strong>: <a href=''{WebUrl}'' style=''color: #0066cc; text-decoration: none; font-weight: bold;''>{WebUrl}</a></li>
                <li>Ingrese su correo electrónico como nombre de usuario: <strong>{Email}</strong></li>
                <li>Luego coloque la contraseña temporera <strong>{TemporaryPassword}</strong></li>
                <li>Deberá completar la sección en el menú principal llamada <strong>""Sitios""</strong>. Aquí deberá incluir todos los Sitios asociados a su Organización o Institución que estarán participando del programa de su interés.</li>
                <li>Deberá completar la sección en el menú principal llamada <strong>""Personal""</strong> donde listará todos los empleados administrativos y operacionales, así como los miembros de la junta directiva.</li>
            </ul>
            
            <p>Si tiene alguna pregunta o necesita asistencia adicional, no dude en contactarnos. 
            Estamos aquí para ayudarle en cada paso del camino.</p>
            
            <p>Una vez más, gracias por su interés y confianza en AESAN.<br>
            Juntos podemos lograr grandes cosas.</p>
            
            <p>Saludos cordiales,<br>
            <strong>Agencia Estatal Servicios de Alimentos y Nutrición</strong><br>
            (787) 759-2000 / Exts. 4625751, 4625753</p>
        </div>',
        '<div style=''font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;''>
            <p>Dear {FullName},</p>
            
            <p>We are very pleased to know that you are interested in being part of our programs at AESAN. 
            Your support and participation are fundamental to continue with our mission of offering food services 
            and nutritional education to participants, covering all ages from childhood to old age.</p>
            
            <p>To continue with your registration, validation and final approval process, we ask you to follow these steps:</p>
            
            <ul>
                <li>Click on the following link to access the <strong>NUTRE</strong> platform: <a href=''{WebUrl}'' style=''color: #0066cc; text-decoration: none; font-weight: bold;''>{WebUrl}</a></li>
                <li>Enter your email address as username: <strong>{Email}</strong></li>
                <li>Then enter the temporary password <strong>{TemporaryPassword}</strong></li>
                <li>You must complete the section in the main menu called <strong>""Sites""</strong>. Here you must include all Sites associated with your Organization or Institution that will be participating in the program of your interest.</li>
                <li>You must complete the section in the main menu called <strong>""Staff""</strong> where you will list all administrative and operational employees, as well as board members.</li>
            </ul>
            
            <p>If you have any questions or need additional assistance, do not hesitate to contact us. 
            We are here to help you every step of the way.</p>
            
            <p>Once again, thank you for your interest and trust in AESAN.<br>
            Together we can achieve great things.</p>
            
            <p>Best regards,<br>
            <strong>State Agency for Food Services and Nutrition</strong><br>
            (787) 759-2000 / Exts. 4625751, 4625753</p>
        </div>',
        'Email de bienvenida enviado a nuevas agencias cuando se registran en el sistema',
        1
    );

-- 2. ApprovalSponsor - Email de aprobación de auspiciador
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'ApprovalSponsor',
        '¡Gracias por su interés en formar parte del programa de AESAN!',
        'Thank you for your interest in being part of the AESAN program!',
        '<div style=''font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;''>
            <p>Estimado/a {FullName},</p>
            
            <p>Nos complace enormemente saber que está interesado/a en formar parte de nuestros programas en AESAN. 
            Su apoyo y participación son fundamentales para continuar con nuestra misión de ofrecer servicios de alimentos 
            y educación nutricional a los participantes, cubriendo todas las edades desde la niñez hasta la vejez.</p>
            
            <p>Para continuar con su proceso de registro, validación y aprobación final, le pedimos que siga los siguientes pasos:</p>
            
            <ul>
                <li>Haz clic en el siguiente enlace para acceder a la plataforma <strong>NUTRE</strong>: <a href=''{WebUrl}'' style=''color: #0066cc; text-decoration: none; font-weight: bold;''>{WebUrl}</a></li>
                <li>Luego coloque la contraseña temporera <strong>{TemporaryPassword}</strong></li>
                <li>Deberá completar la sección en el menú principal llamada <strong>""Sitios""</strong>. Aquí deberá incluir todos los Sitios asociados a su Organización o Institución que estarán participando del programa de su interés.</li>
                <li>Deberá completar la sección en el menú principal llamada <strong>""Personal""</strong> donde listará todos los empleados administrativos y operacionales, así como los miembros de la junta directiva.</li>
            </ul>
            
            <p>Si tiene alguna pregunta o necesita asistencia adicional, no dude en contactarnos. 
            Estamos aquí para ayudarle en cada paso del camino.</p>
            
            <p>Una vez más, gracias por su interés y confianza en AESAN.<br>
            Juntos podemos lograr grandes cosas.</p>
            
            <p>Saludos cordiales,<br>
            <strong>Agencia Estatal Servicios de Alimentos y Nutrición</strong><br>
            (787) 759-2000 / Exts. 4625751, 4625753</p>
        </div>',
        '<div style=''font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;''>
            <p>Dear {FullName},</p>
            
            <p>We are very pleased to know that you are interested in being part of our programs at AESAN. 
            Your support and participation are fundamental to continue with our mission of offering food services 
            and nutritional education to participants, covering all ages from childhood to old age.</p>
            
            <p>To continue with your registration, validation and final approval process, we ask you to follow these steps:</p>
            
            <ul>
                <li>Click on the following link to access the <strong>NUTRE</strong> platform: <a href=''{WebUrl}'' style=''color: #0066cc; text-decoration: none; font-weight: bold;''>{WebUrl}</a></li>
                <li>Then enter the temporary password <strong>{TemporaryPassword}</strong></li>
                <li>You must complete the section in the main menu called <strong>""Sites""</strong>. Here you must include all Sites associated with your Organization or Institution that will be participating in the program of your interest.</li>
                <li>You must complete the section in the main menu called <strong>""Staff""</strong> where you will list all administrative and operational employees, as well as board members.</li>
            </ul>
            
            <p>If you have any questions or need additional assistance, do not hesitate to contact us. 
            We are here to help you every step of the way.</p>
            
            <p>Once again, thank you for your interest and trust in AESAN.<br>
            Together we can achieve great things.</p>
            
            <p>Best regards,<br>
            <strong>State Agency for Food Services and Nutrition</strong><br>
            (787) 759-2000 / Exts. 4625751, 4625753</p>
        </div>',
        'Email enviado cuando se aprueba un auspiciador',
        1
    );

-- 3. DenialSponsor - Email de rechazo de auspiciador
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'DenialSponsor',
        'Actualización sobre tu aplicación al programa de AESAN',
        'Update on your application to the AESAN program',
        '<div style=''font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;''>
            <p>Estimado/a {FullName},</p>
            
            <p>Lamentamos informarte que tu aplicación para participar en el programa de AESAN ha sido rechazada por la siguiente razón: {RejectionReason}</p>
            
            <p>Si tienes alguna pregunta o necesitas aclaraciones adicionales, no dudes en contactarnos. 
            Estamos aquí para ayudarte y proporcionar más información si lo necesitas.</p>
            
            <p>Agradecemos sinceramente tu interés en AESAN y te deseamos éxito en tus futuros proyectos.</p>
            
            <p>Atentamente,<br>
            Equipo de AESAN<br>
            {Email}<br>
            </p>
        </div>',
        '<div style=''font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;''>
            <p>Dear {FullName},</p>
            
            <p>We regret to inform you that your application to participate in the AESAN program has been rejected for the following reason: {RejectionReason}</p>
            
            <p>If you have any questions or need additional clarifications, do not hesitate to contact us. 
            We are here to help you and provide more information if needed.</p>
            
            <p>We sincerely appreciate your interest in AESAN and wish you success in your future projects.</p>
            
            <p>Sincerely,<br>
            AESAN Team<br>
            {Email}<br>
            </p>
        </div>',
        'Email enviado cuando se rechaza un auspiciador',
        1
    );

-- 4. AgencyAssignment - Email de asignación de agencia
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'AgencyAssignment',
        'Asignación de Agencia en NUTRE',
        'Agency Assignment in NUTRE',
        '<h2>Asignación de Agencia</h2>
        <p>Estimado/a {FullName},</p>
        <p>Le informamos que se le ha asignado la siguiente agencia en el sistema NUTRE:</p>
        <ul>
            <li><strong>Nombre de la Agencia:</strong> {AgencyName}</li>
            <li><strong>Código de la Agencia:</strong> {AgencyCode}</li>
        </ul>
        <p>Ya puede acceder a la información de esta agencia a través de su cuenta en el sistema.</p>
        <p>Si tiene alguna pregunta o necesita asistencia, no dude en contactarnos.</p>
        <p>Atentamente,<br>El equipo de NUTRE</p>',
        '<h2>Agency Assignment</h2>
        <p>Dear {FullName},</p>
        <p>We inform you that the following agency has been assigned to you in the NUTRE system:</p>
        <ul>
            <li><strong>Agency Name:</strong> {AgencyName}</li>
            <li><strong>Agency Code:</strong> {AgencyCode}</li>
        </ul>
        <p>You can now access the information of this agency through your account in the system.</p>
        <p>If you have any questions or need assistance, do not hesitate to contact us.</p>
        <p>Sincerely,<br>The NUTRE team</p>',
        'Email enviado cuando se asigna una agencia a un usuario',
        1
    );

-- 5. AgencyUnassignment - Email de desasignación de agencia
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'AgencyUnassignment',
        'Desasignación de Agencia',
        'Agency Unassignment',
        '<p>Estimado/a {FullName},</p>
        <p>Le informamos que ha sido desasignado/a como monitor de la agencia {AgencyName}.</p>
        <p>Gracias por su atención.</p>',
        '<p>Dear {FullName},</p>
        <p>We inform you that you have been unassigned as monitor of the agency {AgencyName}.</p>
        <p>Thank you for your attention.</p>',
        'Email enviado cuando se desasigna una agencia a un usuario',
        1
    );

-- 6. PasswordChanged - Email de cambio de contraseña
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'PasswordChanged',
        'Tu contraseña temporera ha sido generada',
        'Your temporary password has been generated',
        '<h2>Se ha generado una contraseña temporera para tu cuenta</h2>
        <p>Estimado/a {FullName},</p>
        <p>Un administrador ha generado una contraseña temporera para tu cuenta en el sistema.</p>
        <p>Tu contraseña temporera es: <strong>{NewPassword}</strong></p>
        <p><strong>Importante:</strong></p>
        <ul>
            <li>Esta es una contraseña temporera que debes cambiar en tu próximo inicio de sesión.</li>
            <li>Al ingresar con esta contraseña, el sistema te guiará automáticamente para crear una nueva contraseña segura.</li>
            <li>Por razones de seguridad, no compartas esta contraseña con nadie.</li>
        </ul>
        <p>Si no has solicitado este cambio o tienes alguna pregunta, por favor contacta al administrador del sistema.</p>',
        '<h2>A temporary password has been generated for your account</h2>
        <p>Dear {FullName},</p>
        <p>An administrator has generated a temporary password for your account in the system.</p>
        <p>Your temporary password is: <strong>{NewPassword}</strong></p>
        <p><strong>Important:</strong></p>
        <ul>
            <li>This is a temporary password that you must change on your next login.</li>
            <li>When logging in with this password, the system will automatically guide you to create a new secure password.</li>
            <li>For security reasons, do not share this password with anyone.</li>
        </ul>
        <p>If you did not request this change or have any questions, please contact the system administrator.</p>',
        'Email enviado cuando un administrador genera una nueva contraseña temporera',
        1
    );

-- 7. PasswordReset - Email de restablecimiento de contraseña (con link)
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'PasswordReset',
        'Restablecimiento de Contraseña - AESAN',
        'Password Reset - AESAN',
        '<h2>Solicitud de Restablecimiento de Contraseña</h2>
        <p>Has solicitado restablecer tu contraseña en el sistema AESAN.</p>
        <p>Para continuar con el proceso, haz clic en el siguiente enlace:</p>
        <p><a href=''{ResetLink}''>Restablecer Contraseña</a></p>
        <p><strong>Importante:</strong></p>
        <ul>
            <li>Este enlace expirará en 30 minutos por razones de seguridad.</li>
            <li>Si no has solicitado este cambio, puedes ignorar este correo.</li>
            <li>Tu contraseña actual seguirá siendo válida hasta que completes el proceso de restablecimiento.</li>
        </ul>
        <p>Si tienes alguna pregunta, por favor contacta al administrador del sistema.</p>',
        '<h2>Password Reset Request</h2>
        <p>You have requested to reset your password in the AESAN system.</p>
        <p>To continue with the process, click on the following link:</p>
        <p><a href=''{ResetLink}''>Reset Password</a></p>
        <p><strong>Important:</strong></p>
        <ul>
            <li>This link will expire in 30 minutes for security reasons.</li>
            <li>If you did not request this change, you can ignore this email.</li>
            <li>Your current password will remain valid until you complete the reset process.</li>
        </ul>
        <p>If you have any questions, please contact the system administrator.</p>',
        'Email enviado cuando un usuario solicita restablecer su contraseña',
        1
    );

-- 8. TemporaryPassword - Email de contraseña temporera simple
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'TemporaryPassword',
        'Tu contraseña temporera',
        'Your temporary password',
        '<p>Tu contraseña temporera es: <strong>{TemporaryPassword}</strong></p>',
        '<p>Your temporary password is: <strong>{TemporaryPassword}</strong></p>',
        'Email simple con contraseña temporera',
        1
    );

-- 9. GenericEmail - Email genérico
INSERT INTO [dbo].[EmailTemplate]
    (TemplateKey, SubjectES, SubjectEN, BodyES, BodyEN, Description, IsActive)
VALUES
    (
        'GenericEmail',
        'Notificación del Sistema AESAN',
        'AESAN System Notification',
        '<p>{Message}</p>',
        '<p>{Message}</p>',
        'Email genérico para notificaciones del sistema',
        1
    );

GO

