CREATE OR ALTER PROCEDURE [dbo].[100_AssignEvaluatorToProgram]
    @userId NVARCHAR(450),
    @programId INT,
    @assignedBy NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;
    DECLARE @userRole NVARCHAR(50);

    -- Validar que el usuario existe
    IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = @userId)
    BEGIN
        RAISERROR('El usuario especificado no existe.', 16, 1);
        RETURN -1;
    END

    -- Validar que el programa existe
    IF NOT EXISTS (SELECT 1 FROM Program WHERE Id = @programId)
    BEGIN
        RAISERROR('El programa especificado no existe.', 16, 1);
        RETURN -1;
    END

    -- Obtener el rol del usuario
    SELECT TOP 1
        @userRole = r.Name
    FROM AspNetUserRoles ur
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId;

    -- Validar que el usuario tenga el rol "Evaluador"
    IF @userRole != 'Evaluador'
    BEGIN
        RAISERROR('Solo usuarios con rol "Evaluador" pueden ser asignados a programas.', 16, 1);
        RETURN -1;
    END

    -- Si ya existe una asignación activa para este usuario y programa, actualizarla
    IF EXISTS (
        SELECT 1
        FROM UserProgram
        WHERE UserId = @userId
            AND ProgramId = @programId
            AND IsActive = 1
    )
    BEGIN
        UPDATE UserProgram
        SET UpdatedAt = GETUTCDATE()
        WHERE UserId = @userId
            AND ProgramId = @programId
            AND IsActive = 1;

        SELECT @Id = Id
        FROM UserProgram
        WHERE UserId = @userId
            AND ProgramId = @programId
            AND IsActive = 1;
    END
    ELSE
    BEGIN
        -- Insertar la nueva asignación
        INSERT INTO UserProgram
            (UserId, ProgramId, IsActive, CreatedAt)
        VALUES
            (@userId, @programId, 1, GETUTCDATE());

        SET @Id = SCOPE_IDENTITY();
    END

    -- Retornar el ID de la asignación
    SELECT @Id AS Id;
END
GO

