-- =============================================
-- ServiceTypeProgram inserts
-- AESAN-257: PDAM (1), PSAV (2), PACNA (3)
-- ServiceType: 1=Breakfast, 2=Lunch, 3=SnackAM, 4=Dinner, 5=SnackPM, 6=SnackNight, 7=DinnerExtended, 8=DinnerAtRisk, 9=SnackExtended, 10=SnackAtRisk
-- MinimumMinutesToNextService: 10 min por regulación (configurable por programa/servicio)
-- =============================================

-- PDAM (ProgramId=1): Desayuno(1), Almuerzo(2) fuerte, Merienda Vespertina(5)
INSERT INTO [dbo].[ServiceTypeProgram] (ServiceTypeId, ProgramId, IsStrongService, MinimumMinutesToNextService, DisplayOrder, IsActive)
VALUES (1, 1, 0, 10, 1, 1), (2, 1, 1, 10, 2, 1), (5, 1, 0, 10, 3, 1);

-- PSAV (ProgramId=2): Desayuno(1), Almuerzo(2) fuerte, SnackAM(3), Cena(4) fuerte, SnackPM(5)
INSERT INTO [dbo].[ServiceTypeProgram] (ServiceTypeId, ProgramId, IsStrongService, MinimumMinutesToNextService, DisplayOrder, IsActive)
VALUES (1, 2, 0, 10, 1, 1), (2, 2, 1, 10, 2, 1), (3, 2, 0, 10, 3, 1), (4, 2, 1, 10, 4, 1), (5, 2, 0, 10, 5, 1);

-- PACNA (ProgramId=3): todos 1-10; Almuerzo(2) y Cena(4) fuertes; específicos PACNA 7,8,9,10
INSERT INTO [dbo].[ServiceTypeProgram] (ServiceTypeId, ProgramId, IsStrongService, MinimumMinutesToNextService, DisplayOrder, IsActive)
VALUES (1, 3, 0, 10, 1, 1), (2, 3, 1, 10, 2, 1), (3, 3, 0, 10, 3, 1), (4, 3, 1, 10, 4, 1), (5, 3, 0, 10, 5, 1),
       (6, 3, 0, 10, 6, 1), (7, 3, 0, 10, 7, 1), (8, 3, 0, 10, 8, 1), (9, 3, 0, 10, 9, 1), (10, 3, 0, 10, 10, 1);

GO
