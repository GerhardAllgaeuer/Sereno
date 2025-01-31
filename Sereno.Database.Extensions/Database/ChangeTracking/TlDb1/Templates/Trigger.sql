IF OBJECT_ID('{{TableName}}_t1', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER {{TableName}}_t1;
END;
GO

CREATE TRIGGER {{TableName}}_t1
ON {{TableName}}
AFTER INSERT, UPDATE
AS
BEGIN

    -- Benutzer aus SESSION_CONTEXT auslesen und in NVARCHAR konvertieren
    DECLARE @SessionUser NVARCHAR(500);
    SET @SessionUser = CONVERT(NVARCHAR(500), SESSION_CONTEXT(N'UserName'));

    -- Fallback auf SUSER_NAME(), falls SESSION_CONTEXT keinen Wert liefert
    IF @SessionUser IS NULL
        SET @SessionUser = SUSER_NAME();

    -- Aktualisierung für INSERT (setzt sowohl dCreate als auch dModify)
    UPDATE {{TableName}}
    SET dCreate = GetDate(),
        vCreateUser = @SessionUser,
        dModify = GetDate(),
        vModifyUser = @SessionUser
    FROM inserted
    WHERE {{TableName}}.vId = inserted.vId
    AND NOT EXISTS (SELECT 1 FROM {{TableName}} WHERE {{TableName}}.vId = inserted.vId AND dCreate IS NOT NULL);

    -- Aktualisierung für UPDATE (setzt nur dModify)
    UPDATE {{TableName}}
    SET dModify = GetDate(),
        vModifyUser = @SessionUser
    FROM inserted
    WHERE {{TableName}}.vId = inserted.vId;
END;