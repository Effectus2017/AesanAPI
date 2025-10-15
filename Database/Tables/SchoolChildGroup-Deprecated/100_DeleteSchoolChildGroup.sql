CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchoolChildGroup]
    @id INT
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

    -- Verificar si hay servicios asociados a este grupo
    IF EXISTS (SELECT 1
    FROM SchoolService
    WHERE ChildGroupId = @id)
    BEGIN
        RAISERROR('No se puede eliminar el grupo porque tiene servicios asociados. Elimine primero los servicios.', 16, 1);
        RETURN;
    END

    DELETE FROM SchoolChildGroup WHERE Id = @id;
END;
