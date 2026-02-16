-- =============================================
-- Migración: Eliminar Satélite e Institución Residencial de PSAV (Tipo de Organización)
-- Fecha: 2025-02-12
-- Descripción: Quita las opciones Satélite e Institución Residencial del programa PSAV
--              en la tabla OrganizationTypeProgram. PSAV queda solo con Escuela y Municipios.
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'OrganizationTypeProgram')
BEGIN
    PRINT 'Tabla OrganizationTypeProgram no existe. Saltando migración.';
    RETURN;
END

BEGIN TRY
    DELETE otp
    FROM OrganizationTypeProgram otp
    INNER JOIN OrganizationType ot ON ot.Id = otp.OrganizationTypeId
    WHERE otp.ProgramId = 2
      AND ot.Name IN (N'Satélite', N'Institución Residencial');

    PRINT 'Migración 119: Satélite e Institución Residencial eliminados de PSAV en OrganizationTypeProgram.';
END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR('Error en migración 119: %s', 16, 1, @ErrorMessage);
END CATCH;
