SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Stored Procedure para eliminar lógicamente un archivo asociado a una agencia
-- =============================================
CREATE OR ALTER PROCEDURE [100_DeleteAgencyFile]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;
    
    -- Eliminación lógica (marcar como inactivo)
    UPDATE AgencyFiles
    SET IsActive = 0,
        IsDeleted = 1
    WHERE Id = @id AND IsActive = 1;
    
    SELECT @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 