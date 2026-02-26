CREATE TABLE [dbo].[AgencyAppointment] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [AgencyId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Agency]([Id]),
    [AppointmentDate] DATE NOT NULL,
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [Comments] NVARCHAR(1000) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(255) NULL,
    [UpdatedBy] NVARCHAR(255) NULL
);
GO
