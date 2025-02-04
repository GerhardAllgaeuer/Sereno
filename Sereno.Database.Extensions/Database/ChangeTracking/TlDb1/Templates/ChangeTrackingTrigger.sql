CREATE TRIGGER {{TableName}}_ti1 ON {{TableName}}
AFTER INSERT, UPDATE, DELETE AS
BEGIN
    -- Benutzer aus SESSION_CONTEXT auslesen und in NVARCHAR konvertieren
    DECLARE @SessionUser NVARCHAR(500);
    SET @SessionUser = CONVERT(NVARCHAR(500), SESSION_CONTEXT(N'UserName'));

    -- Fallback auf SUSER_NAME(), falls SESSION_CONTEXT keinen Wert liefert
    IF @SessionUser IS NULL
        SET @SessionUser = SUSER_NAME();


    -- DELETE
    IF EXISTS (SELECT 1 FROM deleted) AND NOT EXISTS (SELECT 1 FROM inserted)
    BEGIN
		RETURN;
    END

    -- INSERT
    IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
		UPDATE {{TableName}}
		SET dCreate = GetDate(),
			vCreateUser = @SessionUser,
			dModify = GetDate(),
			vModifyUser = @SessionUser
		FROM inserted
		WHERE {{TableName}}.vId = inserted.vId
		AND NOT EXISTS (SELECT 1 FROM {{TableName}} WHERE {{TableName}}.vId = inserted.vId AND dCreate IS NOT NULL);
        RETURN;
    END

    -- UPDATE
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
		UPDATE {{TableName}}
		SET dModify = GetDate(),
			vModifyUser = @SessionUser
		FROM inserted
		WHERE {{TableName}}.vId = inserted.vId;
		RETURN;
    END
END;
GO

commit