-- =============================================
-- Tabla: Staff (Personal)
-- =============================================
-- Registra todos los miembros del personal del sistema.
-- Un miembro del staff puede tener un usuario asociado para convertirse en usuario del sistema.

CREATE TABLE Staff
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental

    -- Información Personal
    FirstName NVARCHAR(100) NOT NULL,
    -- Nombre del personal
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
    -- Referencia a OptionSelection con optionKey = 'staffPosition'
    StaffTypeId INT NOT NULL DEFAULT 1,
    -- Referencia a StaffType

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
    -- Referencia a AspNetUsers para convertir personal en usuario

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
    FOREIGN KEY (StaffTypeId) REFERENCES StaffType(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Índices para mejorar el rendimiento
CREATE INDEX IX_Staff_FirstName ON Staff(FirstName);
CREATE INDEX IX_Staff_FatherLastName ON Staff(FatherLastName);
CREATE INDEX IX_Staff_Email ON Staff(Email);
CREATE INDEX IX_Staff_StatusId ON Staff(StatusId);
CREATE INDEX IX_Staff_PositionId ON Staff(PositionId);
CREATE INDEX IX_Staff_StaffTypeId ON Staff(StaffTypeId);
CREATE INDEX IX_Staff_CityId ON Staff(CityId);
CREATE INDEX IX_Staff_RegionId ON Staff(RegionId);
CREATE INDEX IX_Staff_UserId ON Staff(UserId);
CREATE INDEX IX_Staff_IsActive ON Staff(IsActive);
CREATE INDEX IX_Staff_CreatedAt ON Staff(CreatedAt);