-- =============================================
-- Stored Procedure: 102_SyncUsersToStaff
-- =============================================
-- Sincroniza usuarios de AspNetUsers con la tabla Staff
-- Para cada usuario que no tenga Staff asociado:
--   1. Intenta extraer nombre y apellido del UserName
--   2. Completa los datos faltantes del Staff si existe
--   3. Crea un nuevo registro Staff si no existe
--
-- Formato esperado del UserName: nombre.apellido o nombre_apellido o nombre apellido
-- Parámetros:
--   @UserId: (Opcional) Si se proporciona, procesa solo ese usuario. Si es NULL, procesa todos los usuarios activos.

CREATE OR ALTER PROCEDURE [dbo].[102_SyncUsersToStaff]
    @UserId NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @currentUserId NVARCHAR(450);
    DECLARE @userName NVARCHAR(256);
    DECLARE @email NVARCHAR(256);
    DECLARE @phoneNumber NVARCHAR(MAX);
    DECLARE @agencyId INT;
    DECLARE @staffId INT;
    DECLARE @firstName NVARCHAR(100);
    DECLARE @fatherLastName NVARCHAR(100);
    DECLARE @motherLastName NVARCHAR(100);
    DECLARE @parsedName NVARCHAR(100);
    DECLARE @parsedLastName NVARCHAR(100);
    DECLARE @defaultBirthDate DATETIME = '1900-01-01';
    DECLARE @defaultPostalAddress NVARCHAR(500) = 'Dirección no especificada';
    DECLARE @defaultZipCode NVARCHAR(10) = '00000';

    -- Cursor para recorrer usuarios
    -- Si @UserId se proporciona, procesa solo ese usuario
    -- Si @UserId es NULL, procesa todos los usuarios activos
    -- Obtener AgencyId desde AgencyUsers (puede haber múltiples, tomamos el primero activo)
    DECLARE user_cursor CURSOR FOR
    SELECT
        u.Id,
        u.UserName,
        u.Email,
        u.PhoneNumber,
        (SELECT TOP 1
            AgencyId
        FROM AgencyUsers
        WHERE UserId = u.Id AND IsActive = 1
        ORDER BY AssignedDate DESC) AS AgencyId
    FROM AspNetUsers u
    WHERE u.IsActive = 1
        AND (@UserId IS NULL OR u.Id = @UserId);

    OPEN user_cursor;
    FETCH NEXT FROM user_cursor INTO @currentUserId, @userName, @email, @phoneNumber, @agencyId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Verificar si el usuario ya tiene Staff asociado
        SELECT @staffId = Id
        FROM Staff
        WHERE UserId = @currentUserId;

        IF @staffId IS NULL
        BEGIN
            -- El usuario no tiene Staff, necesitamos crearlo
            -- Intentar parsear el UserName para extraer nombre y apellido
            SET @firstName = NULL;
            SET @fatherLastName = NULL;
            SET @motherLastName = NULL;

            -- Variable para trabajar con el nombre limpio (sin @ y dominio si es email)
            DECLARE @cleanUserName NVARCHAR(256);

            IF @userName IS NOT NULL AND LEN(LTRIM(RTRIM(@userName))) > 0
            BEGIN
                -- Si el UserName es un email, extraer solo la parte antes del @
                IF CHARINDEX('@', @userName) > 0
                BEGIN
                    SET @cleanUserName = LTRIM(RTRIM(SUBSTRING(@userName, 1, CHARINDEX('@', @userName) - 1)));
                END
                ELSE
                BEGIN
                    SET @cleanUserName = LTRIM(RTRIM(@userName));
                END

                -- Intentar diferentes formatos de UserName (ahora limpio)
                -- Formato 1: nombre.apellido
                IF CHARINDEX('.', @cleanUserName) > 0
                BEGIN
                    SET @parsedName = LTRIM(RTRIM(SUBSTRING(@cleanUserName, 1, CHARINDEX('.', @cleanUserName) - 1)));
                    SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@cleanUserName, CHARINDEX('.', @cleanUserName) + 1, LEN(@cleanUserName))));

                    -- Si hay más puntos, tomar solo el primer apellido
                    IF CHARINDEX('.', @parsedLastName) > 0
                    BEGIN
                        SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@parsedLastName, 1, CHARINDEX('.', @parsedLastName) - 1)));
                    END

                    SET @firstName = @parsedName;
                    SET @fatherLastName = @parsedLastName;
                    SET @motherLastName = '';
                -- No podemos extraer el apellido materno del UserName
                END
                -- Formato 2: nombre_apellido
                ELSE IF CHARINDEX('_', @cleanUserName) > 0
                BEGIN
                    SET @parsedName = LTRIM(RTRIM(SUBSTRING(@cleanUserName, 1, CHARINDEX('_', @cleanUserName) - 1)));
                    SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@cleanUserName, CHARINDEX('_', @cleanUserName) + 1, LEN(@cleanUserName))));

                    -- Si hay más guiones bajos, tomar solo el primer apellido
                    IF CHARINDEX('_', @parsedLastName) > 0
                    BEGIN
                        SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@parsedLastName, 1, CHARINDEX('_', @parsedLastName) - 1)));
                    END

                    SET @firstName = @parsedName;
                    SET @fatherLastName = @parsedLastName;
                    SET @motherLastName = '';
                END
                -- Formato 3: nombre apellido (con espacio)
                ELSE IF CHARINDEX(' ', @cleanUserName) > 0
                BEGIN
                    SET @parsedName = LTRIM(RTRIM(SUBSTRING(@cleanUserName, 1, CHARINDEX(' ', @cleanUserName) - 1)));
                    SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@cleanUserName, CHARINDEX(' ', @cleanUserName) + 1, LEN(@cleanUserName))));

                    -- Si hay más espacios, tomar solo el primer apellido
                    IF CHARINDEX(' ', @parsedLastName) > 0
                    BEGIN
                        SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@parsedLastName, 1, CHARINDEX(' ', @parsedLastName) - 1)));
                    END

                    SET @firstName = @parsedName;
                    SET @fatherLastName = @parsedLastName;
                    SET @motherLastName = '';
                END
                -- Si no hay separador, usar todo el UserName limpio como nombre
                ELSE
                BEGIN
                    SET @firstName = LTRIM(RTRIM(@cleanUserName));
                    SET @fatherLastName = 'Usuario';
                    SET @motherLastName = '';
                END
            END
            ELSE
            BEGIN
                -- Si no hay UserName, usar valores por defecto
                SET @firstName = 'Usuario';
                SET @fatherLastName = 'Sin Nombre';
                SET @motherLastName = '';
            END

            -- Asegurar que tenemos valores válidos
            IF @firstName IS NULL OR LEN(LTRIM(RTRIM(@firstName))) = 0
                SET @firstName = 'Usuario';

            IF @fatherLastName IS NULL OR LEN(LTRIM(RTRIM(@fatherLastName))) = 0
                SET @fatherLastName = 'Sin Apellido';

            IF @motherLastName IS NULL
                SET @motherLastName = '';

            -- Asegurar que tenemos un email válido
            IF @email IS NULL OR LEN(LTRIM(RTRIM(@email))) = 0
                SET @email = @userName + '@nutre.local';
            -- Email por defecto basado en UserName

            -- Obtener AgencyId desde AgencyUsers si no se obtuvo del cursor
            IF @agencyId IS NULL
            BEGIN
                SELECT TOP 1
                    @agencyId = AgencyId
                FROM AgencyUsers
                WHERE UserId = @currentUserId AND IsActive = 1
                ORDER BY AssignedDate DESC;
            END

            -- Crear el registro Staff
            INSERT INTO Staff
                (
                FirstName,
                MiddleName,
                FatherLastName,
                MotherLastName,
                StatusId,
                PositionId,
                StaffTypeId,
                BirthDate,
                Email,
                PhoneNumber,
                PostalAddress,
                CityId,
                RegionId,
                ZipCode,
                AgencyId,
                UserId,
                CreatedAt,
                IsActive
                )
            VALUES
                (
                    @firstName,
                    NULL, -- MiddleName
                    @fatherLastName,
                    @motherLastName,
                    1, -- StatusId por defecto (activo)
                    37, -- PositionId por defecto
                    1, -- StaffTypeId por defecto (Empleado)
                    @defaultBirthDate, -- BirthDate por defecto
                    @email,
                    @phoneNumber,
                    @defaultPostalAddress,
                    1, -- CityId por defecto
                    1, -- RegionId por defecto
                    @defaultZipCode,
                    @agencyId,
                    @currentUserId,
                    GETDATE(),
                    1 -- IsActive
                );

            PRINT 'Staff creado para usuario: ' + @userName + ' (UserId: ' + @currentUserId + ')';
        END
        ELSE
        BEGIN
            -- El usuario ya tiene Staff, verificar si hay datos faltantes y completarlos
            DECLARE @needsUpdate BIT = 0;
            DECLARE @updateFirstName NVARCHAR(100);
            DECLARE @updateFatherLastName NVARCHAR(100);
            DECLARE @updateEmail NVARCHAR(255);
            DECLARE @updatePhoneNumber NVARCHAR(50);
            DECLARE @updateAgencyId INT;

            -- Obtener datos actuales del Staff
            SELECT
                @updateFirstName = FirstName,
                @updateFatherLastName = FatherLastName,
                @updateEmail = Email,
                @updatePhoneNumber = PhoneNumber,
                @updateAgencyId = AgencyId
            FROM Staff
            WHERE Id = @staffId;

            -- Intentar parsear UserName si faltan datos
            IF (@updateFirstName IS NULL OR LEN(LTRIM(RTRIM(@updateFirstName))) = 0)
                AND @userName IS NOT NULL AND LEN(LTRIM(RTRIM(@userName))) > 0
            BEGIN
                -- Limpiar UserName: si es email, extraer solo la parte antes del @
                DECLARE @cleanUserNameForUpdate NVARCHAR(256);
                IF CHARINDEX('@', @userName) > 0
                BEGIN
                    SET @cleanUserNameForUpdate = LTRIM(RTRIM(SUBSTRING(@userName, 1, CHARINDEX('@', @userName) - 1)));
                END
                ELSE
                BEGIN
                    SET @cleanUserNameForUpdate = LTRIM(RTRIM(@userName));
                END

                -- Parsear UserName limpio
                IF CHARINDEX('.', @cleanUserNameForUpdate) > 0
                BEGIN
                    SET @parsedName = LTRIM(RTRIM(SUBSTRING(@cleanUserNameForUpdate, 1, CHARINDEX('.', @cleanUserNameForUpdate) - 1)));
                    SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@cleanUserNameForUpdate, CHARINDEX('.', @cleanUserNameForUpdate) + 1, LEN(@cleanUserNameForUpdate))));

                    IF CHARINDEX('.', @parsedLastName) > 0
                    BEGIN
                        SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@parsedLastName, 1, CHARINDEX('.', @parsedLastName) - 1)));
                    END

                    SET @updateFirstName = @parsedName;
                    IF (@updateFatherLastName IS NULL OR LEN(LTRIM(RTRIM(@updateFatherLastName))) = 0)
                        SET @updateFatherLastName = @parsedLastName;

                    SET @needsUpdate = 1;
                END
                ELSE IF CHARINDEX('_', @cleanUserNameForUpdate) > 0
                BEGIN
                    SET @parsedName = LTRIM(RTRIM(SUBSTRING(@cleanUserNameForUpdate, 1, CHARINDEX('_', @cleanUserNameForUpdate) - 1)));
                    SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@cleanUserNameForUpdate, CHARINDEX('_', @cleanUserNameForUpdate) + 1, LEN(@cleanUserNameForUpdate))));

                    IF CHARINDEX('_', @parsedLastName) > 0
                    BEGIN
                        SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@parsedLastName, 1, CHARINDEX('_', @parsedLastName) - 1)));
                    END

                    SET @updateFirstName = @parsedName;
                    IF (@updateFatherLastName IS NULL OR LEN(LTRIM(RTRIM(@updateFatherLastName))) = 0)
                        SET @updateFatherLastName = @parsedLastName;

                    SET @needsUpdate = 1;
                END
                ELSE IF CHARINDEX(' ', @cleanUserNameForUpdate) > 0
                BEGIN
                    SET @parsedName = LTRIM(RTRIM(SUBSTRING(@cleanUserNameForUpdate, 1, CHARINDEX(' ', @cleanUserNameForUpdate) - 1)));
                    SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@cleanUserNameForUpdate, CHARINDEX(' ', @cleanUserNameForUpdate) + 1, LEN(@cleanUserNameForUpdate))));

                    IF CHARINDEX(' ', @parsedLastName) > 0
                    BEGIN
                        SET @parsedLastName = LTRIM(RTRIM(SUBSTRING(@parsedLastName, 1, CHARINDEX(' ', @parsedLastName) - 1)));
                    END

                    SET @updateFirstName = @parsedName;
                    IF (@updateFatherLastName IS NULL OR LEN(LTRIM(RTRIM(@updateFatherLastName))) = 0)
                        SET @updateFatherLastName = @parsedLastName;

                    SET @needsUpdate = 1;
                END
                ELSE
                BEGIN
                    SET @updateFirstName = LTRIM(RTRIM(@cleanUserNameForUpdate));
                    SET @needsUpdate = 1;
                END
            END

            -- Completar Email si falta
            IF (@updateEmail IS NULL OR LEN(LTRIM(RTRIM(@updateEmail))) = 0)
                AND @email IS NOT NULL AND LEN(LTRIM(RTRIM(@email))) > 0
            BEGIN
                SET @updateEmail = @email;
                SET @needsUpdate = 1;
            END

            -- Completar PhoneNumber si falta
            IF (@updatePhoneNumber IS NULL OR LEN(LTRIM(RTRIM(@updatePhoneNumber))) = 0)
                AND @phoneNumber IS NOT NULL AND LEN(LTRIM(RTRIM(@phoneNumber))) > 0
            BEGIN
                SET @updatePhoneNumber = @phoneNumber;
                SET @needsUpdate = 1;
            END

            -- Completar AgencyId si falta (obtener desde AgencyUsers si no se obtuvo del cursor)
            IF (@updateAgencyId IS NULL)
            BEGIN
                -- Intentar obtener desde AgencyUsers
                SELECT TOP 1
                    @updateAgencyId = AgencyId
                FROM AgencyUsers
                WHERE UserId = @currentUserId AND IsActive = 1
                ORDER BY AssignedDate DESC;

                IF @updateAgencyId IS NOT NULL
                    SET @needsUpdate = 1;
            END
            ELSE IF @agencyId IS NOT NULL AND @updateAgencyId IS NULL
            BEGIN
                SET @updateAgencyId = @agencyId;
                SET @needsUpdate = 1;
            END

            -- Actualizar Staff si hay cambios
            IF @needsUpdate = 1
            BEGIN
                UPDATE Staff
                SET
                    FirstName = COALESCE(@updateFirstName, FirstName),
                    FatherLastName = COALESCE(@updateFatherLastName, FatherLastName),
                    Email = COALESCE(@updateEmail, Email),
                    PhoneNumber = COALESCE(@updatePhoneNumber, PhoneNumber),
                    AgencyId = COALESCE(@updateAgencyId, AgencyId),
                    UpdatedAt = GETDATE()
                WHERE Id = @staffId;

                PRINT 'Staff actualizado para usuario: ' + @userName + ' (UserId: ' + @currentUserId + ', StaffId: ' + CAST(@staffId AS NVARCHAR(10)) + ')';
            END
        END

        FETCH NEXT FROM user_cursor INTO @currentUserId, @userName, @email, @phoneNumber, @agencyId;
    END

    CLOSE user_cursor;
    DEALLOCATE user_cursor;

    PRINT 'Sincronización completada.';
END