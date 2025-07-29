-- =============================================
-- Tabla: Employee (Empleados)
-- =============================================
-- Registra todos los empleados del sistema.
-- Un empleado puede tener un usuario asociado para convertirse en usuario del sistema.

CREATE TABLE Employee
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental

    -- Información Personal
    FirstName NVARCHAR(100) NOT NULL,
    -- Nombre del empleado
    MiddleName NVARCHAR(100) NULL,
    -- Segundo nombre (opcional)
    FatherLastName NVARCHAR(100) NOT NULL,
    -- Apellido paterno
    MotherLastName NVARCHAR(100) NOT NULL,
    -- Apellido materno

    -- Información Laboral
    StatusId INT NOT NULL DEFAULT 1,
    -- Referencia a OptionSelection con optionKey = 'isActive'
    PositionId INT NOT NULL DEFAULT 0,
    -- Referencia a OptionSelection con optionKey = 'employeePosition'

    -- Información Personal Adicional
    BirthDate DATETIME NOT NULL,
    -- Fecha de nacimiento
    Email NVARCHAR(255) NOT NULL,
    -- Correo electrónico

    -- Dirección
    PostalAddress NVARCHAR(500) NOT NULL,
    -- Dirección postal
    CityId INT NOT NULL DEFAULT 0,
    -- Referencia a la tabla City
    RegionId INT NOT NULL DEFAULT 0,
    -- Referencia a la tabla Region
    AreaCode NVARCHAR(10) NOT NULL,
    -- Código de área

    -- Información Adicional
    Comments NVARCHAR(1000) NULL,
    -- Comentarios adicionales

    -- Relación con Usuario
    UserId NVARCHAR(450) NULL,
    -- Referencia a AspNetUsers para convertir empleado en usuario

    -- Auditoría
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,
    -- Fecha y hora de última actualización
    IsActive BIT NOT NULL DEFAULT 1,
    -- Indica si el registro está activo

    -- Restricciones
    FOREIGN KEY (StatusId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (PositionId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Índices para mejorar el rendimiento
CREATE INDEX IX_Employee_FirstName ON Employee(FirstName);
CREATE INDEX IX_Employee_FatherLastName ON Employee(FatherLastName);
CREATE INDEX IX_Employee_Email ON Employee(Email);
CREATE INDEX IX_Employee_StatusId ON Employee(StatusId);
CREATE INDEX IX_Employee_PositionId ON Employee(PositionId);
CREATE INDEX IX_Employee_CityId ON Employee(CityId);
CREATE INDEX IX_Employee_RegionId ON Employee(RegionId);
CREATE INDEX IX_Employee_UserId ON Employee(UserId);
CREATE INDEX IX_Employee_IsActive ON Employee(IsActive);
CREATE INDEX IX_Employee_CreatedAt ON Employee(CreatedAt); 