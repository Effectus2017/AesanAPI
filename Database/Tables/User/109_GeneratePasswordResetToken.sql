CREATE OR ALTER PROCEDURE [109_GeneratePasswordResetToken]
    @email NVARCHAR(256),
    @token NVARCHAR(128),
    @expirationDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si el email es válido
        IF @email IS NULL OR LEN(TRIM(@email)) = 0
        BEGIN
            RETURN -1; -- Email inválido
        END

        -- Verificar si el token es válido
        IF @token IS NULL OR LEN(@token) < 32
        BEGIN
            RETURN -2; -- Token inválido o muy corto
        END

        -- Verificar si la fecha de expiración es válida (debe ser futura)
        IF @expirationDate <= GETDATE()
        BEGIN
            RETURN -3; -- Fecha de expiración inválida
        END

        -- Verificar si el usuario existe y está activo
        IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = @email AND IsActive = 1)
        BEGIN
            RETURN -4; -- Usuario no encontrado o inactivo
        END

        -- Verificar si hay demasiados intentos recientes (últimos 15 minutos)
        IF EXISTS (
            SELECT 1 
            FROM PasswordResetTokens 
            WHERE Email = @email 
            AND CreatedAt > DATEADD(MINUTE, -15, GETDATE())
            GROUP BY Email
            HAVING COUNT(*) >= 3
        )
        BEGIN
            RETURN -5; -- Demasiados intentos
        END

        BEGIN TRANSACTION;

            -- Invalidar tokens anteriores para este usuario
            UPDATE PasswordResetTokens
            SET IsUsed = 1,
                UsedAt = GETDATE()
            WHERE Email = @email AND IsUsed = 0;

            -- Insertar nuevo token
            INSERT INTO PasswordResetTokens (
                Email,
                Token,
                ExpirationDate,
                CreatedAt,
                IsUsed
            )
            VALUES (
                @email,
                @token,
                @expirationDate,
                GETDATE(),
                0
            );

        COMMIT TRANSACTION;
        RETURN 0; -- Éxito

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Log del error en una tabla de errores si existe
        -- INSERT INTO ErrorLog VALUES (ERROR_MESSAGE(), ERROR_LINE(), GETDATE());
        
        RETURN -99; -- Error general
    END CATCH
END;
GO

-- Crear tabla si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PasswordResetTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE PasswordResetTokens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(256) NOT NULL,
        Token NVARCHAR(128) NOT NULL,
        ExpirationDate DATETIME NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        IsUsed BIT NOT NULL DEFAULT 0,
        UsedAt DATETIME NULL,
        CONSTRAINT CK_PasswordResetTokens_ExpirationDate CHECK (ExpirationDate > CreatedAt)
    );

    CREATE INDEX IX_PasswordResetTokens_Email ON PasswordResetTokens(Email);
    CREATE INDEX IX_PasswordResetTokens_Token ON PasswordResetTokens(Token);
    CREATE INDEX IX_PasswordResetTokens_CreatedAt ON PasswordResetTokens(CreatedAt);
END;
GO 