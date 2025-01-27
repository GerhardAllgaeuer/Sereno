IF OBJECT_ID('docDocument_t1', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER docDocument_t1;
END;
GO

CREATE TRIGGER docDocument_t1
ON docDocument
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Prüfen, ob `vId` überhaupt Teil des Updates ist
    IF (UPDATE(vId))
    BEGIN
        -- Prüfen, ob `vId` tatsächlich geändert wurde
        IF EXISTS (
            SELECT 1
            FROM inserted AS i
            INNER JOIN deleted AS d 
            ON d.vId <> i.vId -- Prüfe, ob sich die vId geändert hat
               AND (d.vId IS NOT NULL AND i.vId IS NOT NULL) -- Beide dürfen nicht NULL sein
        )
        BEGIN
            THROW 50001, 'The Id column cannot be updated.', 1;
        END;
    END;

    -- Temporäre Tabelle für Datenverarbeitung
    DECLARE @ProcessedData TABLE (
        vId NVARCHAR(50),
        vTitle NVARCHAR(500),
        vContent NVARCHAR(MAX),
        dCreate DATETIME2(7),
        vCreateUser NVARCHAR(500),
        dModify DATETIME2(7),
        vModifyUser NVARCHAR(500)
    );

    -- Übernehme die eingefügten/aktualisierten Daten in eine temporäre Tabelle
    INSERT INTO @ProcessedData (vId, vTitle, vContent, dCreate, vCreateUser, dModify, vModifyUser)
    SELECT 
        ISNULL(vId, NEWID()),  -- Generiere eine neue ID, falls diese nicht vorhanden ist
        vTitle,
        vContent,
        CASE 
            WHEN dCreate IS NULL THEN GETDATE() 
            ELSE dCreate 
        END,
        CASE 
            WHEN vCreateUser IS NULL THEN SUSER_NAME() 
            ELSE vCreateUser 
        END,
        GETDATE(),  -- Aktuelles Datum/Uhrzeit für dModify
        SUSER_NAME()  -- Aktueller Benutzer für vModifyUser
    FROM inserted;

    -- INSERT in die Tabelle (nur für neue Datensätze)
    INSERT INTO docDocument(vId, vTitle, vContent, dCreate, vCreateUser, dModify, vModifyUser)
    SELECT pd.vId, pd.vTitle, pd.vContent, pd.dCreate, pd.vCreateUser, pd.dModify, pd.vModifyUser
    FROM @ProcessedData AS pd
    WHERE NOT EXISTS (
        SELECT 1 FROM docDocument AS d WHERE d.vId = pd.vId
    );

    -- UPDATE nur für existierende Datensätze
    UPDATE d
    SET d.vTitle = pd.vTitle,
        d.vContent = pd.vContent,
        d.dModify = pd.dModify,
        d.vModifyUser = pd.vModifyUser
    FROM docDocument AS d
    INNER JOIN @ProcessedData AS pd ON d.vId = pd.vId;
END;
GO
commit;