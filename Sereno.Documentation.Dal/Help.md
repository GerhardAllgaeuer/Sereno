
// bestehende Migrationen löschen
dotnet ef migrations remove

// Neu Erstellung der gesamten Migration
dotnet ef migrations add InitialCreate


// manuelles Datenbank Update 
dotnet ef database update

// Löschen der aktuellen Datenbank
dotnet ef database drop







-- Datenbank manuell löschen

ALTER DATABASE Documentation SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Datenbank löschen
DROP DATABASE Documentation;

go