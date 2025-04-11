
ALTER TABLE Agency
DROP COLUMN NonProfit,
DROP COLUMN FederalFundsDenied,
DROP COLUMN StateFundsDenied,
DROP COLUMN OrganizedAthleticPrograms,
DROP COLUMN AtRiskService,
DROP COLUMN BasicEducationRegistry,
DROP COLUMN ServiceTime,
DROP COLUMN TaxExemptionStatus,
DROP COLUMN TaxExemptionType,
DROP COLUMN IsPropietary;

ALTER TABLE Agency
ADD AgencyInscriptionId int NULL;

ALTER TABLE Agency
ADD FOREIGN KEY (AgencyInscriptionId) REFERENCES AgencyInscription(Id);
GO

-- Migración de datos de Agency a AgencyInscription
BEGIN TRANSACTION;
BEGIN TRY
    -- Primero, insertamos los registros en AgencyInscription
    INSERT INTO AgencyInscription (
        NonProfit,
        FederalFundsDenied,
        StateFundsDenied,
        OrganizedAthleticPrograms,
        AtRiskService,
        BasicEducationRegistry,
        ServiceTime,
        TaxExemptionStatus,
        TaxExemptionType,
        IsPropietary,
        AgencyId
    )
    SELECT 
        NonProfit,
        FederalFundsDenied,
        StateFundsDenied,
        OrganizedAthleticPrograms,
        AtRiskService,
        BasicEducationRegistry,
        ServiceTime,
        TaxExemptionStatus,
        TaxExemptionType,
        IsPropietary,
        Id
    FROM Agency;

    -- Actualizamos la referencia en Agency
    UPDATE a
    SET AgencyInscriptionId = ai.Id
    FROM Agency a
    INNER JOIN AgencyInscription ai ON a.Id = ai.AgencyId;

    -- Verificamos que la migración fue exitosa
    DECLARE @AgencyCount int,
            @InscriptionCount int,
            @UnlinkedAgencies int,
            @UnlinkedInscriptions int;

    SELECT @AgencyCount = COUNT(*) FROM Agency;
    SELECT @InscriptionCount = COUNT(*) FROM AgencyInscription;
    
    SELECT @UnlinkedAgencies = COUNT(*)
    FROM Agency a
    WHERE NOT EXISTS (
        SELECT 1 
        FROM AgencyInscription ai 
        WHERE ai.AgencyId = a.Id
    );

    SELECT @UnlinkedInscriptions = COUNT(*)
    FROM AgencyInscription ai
    WHERE NOT EXISTS (
        SELECT 1 
        FROM Agency a 
        WHERE a.Id = ai.AgencyId
    );

    -- Si hay discrepancias, lanzamos un error
    IF @AgencyCount != @InscriptionCount OR @UnlinkedAgencies > 0 OR @UnlinkedInscriptions > 0
    BEGIN
        DECLARE @ErrorMsg nvarchar(500) = CONCAT(
            'Error en la migración. Agencias: ', @AgencyCount,
            ', Inscripciones: ', @InscriptionCount,
            ', Agencias sin inscripción: ', @UnlinkedAgencies,
            ', Inscripciones sin agencia: ', @UnlinkedInscriptions
        );
        THROW 51000, @ErrorMsg, 1;
    END

    -- Si todo está bien, hacemos commit
    COMMIT TRANSACTION;

    -- Mostramos un resumen de la migración
    SELECT 
        'Migración completada exitosamente' as Status,
        @AgencyCount as TotalAgencias,
        @InscriptionCount as TotalInscripciones;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE(),
            @ErrorSeverity int = ERROR_SEVERITY(),
            @ErrorState int = ERROR_STATE();

    -- Registramos el error detallado
    INSERT INTO ErrorLog (
        ErrorMessage,
        ErrorSeverity,
        ErrorState,
        ErrorDate,
        ErrorProcedure
    )
    VALUES (
        @ErrorMessage,
        @ErrorSeverity,
        @ErrorState,
        GETDATE(),
        'MigrateAgencyToAgencyInscription'
    );

    -- Relanzamos el error
    THROW;
END CATCH;
GO 