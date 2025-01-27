



-- Datenbank manuell löschen

ALTER DATABASE Documentation SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE Documentation;

go
DROP DATABASE DocumentationLog;


go