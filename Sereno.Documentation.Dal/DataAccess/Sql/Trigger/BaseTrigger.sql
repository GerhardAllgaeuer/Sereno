IF OBJECT_ID('Documents_t1', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER Documents_t1;
END;
GO

CREATE TRIGGER Documents_t1
ON Documents
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Temporäre Tabelle für die Verarbeitung der Daten
    DECLARE @ProcessedData TABLE (
        Id UNIQUEIDENTIFIER NOT NULL,
        Title NVARCHAR(MAX),
        Content NVARCHAR(MAX),
        Created DATETIME,
        Updated DATETIME
    );

    -- INSERT-Logik: Fehlende Werte für Id, Created und Updated auffüllen
    INSERT INTO @ProcessedData (Id, Title, Content, Created, Updated)
    SELECT 
        ISNULL(Id, NEWID()),                  -- Ersetze NULL-Ids durch NEWID()
        Title,
        Content,
        ISNULL(Created, GETDATE()),          -- Setze Created, falls NULL
        GETDATE()                            -- Setze Updated auf aktuelle Uhrzeit
    FROM inserted;

    -- Verhindere Änderungen an der Id bei Updates
    IF EXISTS (
        SELECT 1 
        FROM @ProcessedData AS pd
        INNER JOIN deleted AS d ON pd.Id <> d.Id
    )
    BEGIN
        THROW 50001, 'The Id column cannot be updated.', 1;
    END;

    -- INSERT in die Tabelle, wenn die Id nicht existiert
    INSERT INTO Documents (Id, Title, Content, Created, Updated)
    SELECT pd.Id, pd.Title, pd.Content, pd.Created, pd.Updated
    FROM @ProcessedData AS pd
    WHERE NOT EXISTS (
        SELECT 1 FROM Documents AS d WHERE d.Id = pd.Id
    );

    -- UPDATE der bestehenden Zeilen
    UPDATE Documents
    SET 
        Title = pd.Title,
        Content = pd.Content,
        Updated = GETDATE() -- Aktualisiere Updated bei Änderungen
    FROM Documents AS d
    INNER JOIN @ProcessedData AS pd ON d.Id = pd.Id;
END;
GO
commit