CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchoolChildGroup]
    @schoolId INT,
    @groupName NVARCHAR(100),
    @numberOfChildren INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la escuela existe
    IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @schoolId)
    BEGIN
        RAISERROR('La escuela especificada no existe.', 16, 1);
        RETURN;
    END

    -- Validar que el nombre del grupo no esté duplicado para esta escuela
    IF EXISTS (SELECT 1
    FROM SchoolChildGroup
    WHERE SchoolId = @schoolId AND GroupName = @groupName)
    BEGIN
        RAISERROR('Ya existe un grupo con este nombre para esta escuela.', 16, 1);
        RETURN;
    END

    -- Validar que la cantidad de niños sea positiva
    IF @numberOfChildren <= 0
    BEGIN
        RAISERROR('La cantidad de niños debe ser mayor a 0.', 16, 1);
        RETURN;
    END

    INSERT INTO SchoolChildGroup
        (
        SchoolId, GroupName, NumberOfChildren, CreatedAt
        )
    VALUES
        (
            @schoolId, @groupName, @numberOfChildren, GETDATE()
        );

    SET @id = SCOPE_IDENTITY();
END;
