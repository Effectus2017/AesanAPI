CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolChildGroup]
    @id INT,
    @groupName NVARCHAR(100),
    @numberOfChildren INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que el grupo existe
    IF NOT EXISTS (SELECT 1
    FROM SchoolChildGroup
    WHERE Id = @id)
    BEGIN
        RAISERROR('El grupo de niños especificado no existe.', 16, 1);
        RETURN;
    END

    -- Validar que el nombre del grupo no esté duplicado para esta escuela (excluyendo el grupo actual)
    IF EXISTS (SELECT 1
    FROM SchoolChildGroup
    WHERE Id != @id AND SchoolId = (SELECT SchoolId
        FROM SchoolChildGroup
        WHERE Id = @id) AND GroupName = @groupName)
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

    UPDATE SchoolChildGroup
    SET 
        GroupName = @groupName,
        NumberOfChildren = @numberOfChildren,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END;
