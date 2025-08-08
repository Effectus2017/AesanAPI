-- Script para agregar nuevos campos a la tabla School
-- Campos adicionales para servicios y horarios
-- Campos adicionales para información específica

-- Agregar campos para cena
ALTER TABLE School
ADD Dinner BIT NULL;

ALTER TABLE School
ADD DinnerFrom TIME NULL;

ALTER TABLE School
ADD DinnerTo TIME NULL;

-- Agregar campos para merienda nocturna
ALTER TABLE School
ADD SnackNight BIT NULL;

ALTER TABLE School
ADD SnackNightFrom TIME NULL;

ALTER TABLE School
ADD SnackNightTo TIME NULL;

-- Agregar campos adicionales
ALTER TABLE School
ADD CommunityId INT NULL;

ALTER TABLE School
ADD WalkersId INT NULL;

ALTER TABLE School
ADD SiteTypeId INT NULL;

ALTER TABLE School
ADD ExperienceId INT NULL;

ALTER TABLE School
ADD ReviewResultId INT NULL;

ALTER TABLE School
ADD ReviewDate DATETIME NULL;

ALTER TABLE School
ADD ReviewJustification NVARCHAR(500) NULL;
