CREATE OR ALTER PROCEDURE [110_ValidatePasswordResetToken]
    @email NVARCHAR(256),
    @token NVARCHAR(128)
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

        -- Verificar si el usuario existe y está activo
        IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = @email AND IsActive = 1)
        BEGIN
            RETURN -3; -- Usuario no encontrado o inactivo
        END

        -- Verificar si existe un token válido
        DECLARE @tokenRecord TABLE (
            Id INT,
            ExpirationDate DATETIME,
            CreatedAt DATETIME
        );

        INSERT INTO @tokenRecord
        SELECT TOP 1 Id, ExpirationDate, CreatedAt
        FROM PasswordResetTokens 
        WHERE Email = @email 
        AND Token = @token 
        AND IsUsed = 0 
        ORDER BY CreatedAt DESC;

        IF NOT EXISTS (SELECT 1 FROM @tokenRecord)
        BEGIN
            RETURN -4; -- Token no encontrado
        END

        -- Verificar si el token ha expirado
        IF EXISTS (SELECT 1 FROM @tokenRecord WHERE ExpirationDate <= GETDATE())
        BEGIN
            -- Marcar como usado los tokens expirados
            UPDATE PasswordResetTokens
            SET IsUsed = 1,
                UsedAt = GETDATE()
            WHERE Email = @email 
            AND Token = @token 
            AND IsUsed = 0;

            RETURN -5; -- Token expirado
        END

        BEGIN TRANSACTION;
            -- Marcar el token como usado
            UPDATE PasswordResetTokens
            SET IsUsed = 1,
                UsedAt = GETDATE()
            WHERE Id IN (SELECT Id FROM @tokenRecord);

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