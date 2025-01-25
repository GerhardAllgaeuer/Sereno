



-- Datenbank manuell löschen

ALTER DATABASE Documentation SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Datenbank löschen
DROP DATABASE Documentation;

go