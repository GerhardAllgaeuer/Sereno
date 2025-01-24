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

    -- Temporäre Tabelle für Datenverarbeitung
    DECLARE @ProcessedData TABLE (
        Id UNIQUEIDENTIFIER NOT NULL,
        Title NVARCHAR(MAX),
        Content NVARCHAR(MAX)
    );

    -- INSERT-Logik: Fehlende Ids durch NEWID() ersetzen
    INSERT INTO @ProcessedData (Id, Title, Content)
    SELECT 
        ISNULL(Id, NEWID()), -- Ersetze NULL-Ids durch NEWID()
        Title,
        Content
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

    -- INSERT/UPDATE in die Tabelle
    -- Führe ein INSERT durch, wenn die Id nicht existiert
    -- und ein UPDATE, wenn die Id bereits vorhanden ist
    INSERT INTO Documents (Id, Title, Content)
    SELECT pd.Id, pd.Title, pd.Content
    FROM @ProcessedData AS pd
    WHERE NOT EXISTS (
        SELECT 1 FROM Documents AS d WHERE d.Id = pd.Id
    );

    UPDATE Documents
    SET Title = pd.Title,
        Content = pd.Content
    FROM Documents AS d
    INNER JOIN @ProcessedData AS pd ON d.Id = pd.Id;
END;
GO

commit;