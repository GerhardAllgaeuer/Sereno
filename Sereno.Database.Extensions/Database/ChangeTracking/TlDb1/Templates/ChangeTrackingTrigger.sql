
CREATE TRIGGER {{TableName}}_t1 ON {{TableName}} 
AFTER INSERT, UPDATE, DELETE AS

DECLARE @ChangeTime DATETIME2;
DECLARE @PrimaryKey NVARCHAR(50);
DECLARE @ChangeId NVARCHAR(50);
DECLARE @SessionUser NVARCHAR(500);

BEGIN

    SET NOCOUNT ON

    SET @SessionUser = CONVERT(NVARCHAR(500), SESSION_CONTEXT(N'UserName'));

    -- Fallback auf SUSER_NAME(), falls SESSION_CONTEXT keinen Wert liefert
    IF @SessionUser IS NULL
        SET @SessionUser = SUSER_NAME();

    -- INSERT
    IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
		BEGIN
			BEGIN
				DECLARE cursorInserted CURSOR FOR
					SELECT vId
					FROM inserted

				OPEN cursorInserted
				FETCH NEXT FROM cursorInserted into @PrimaryKey
				While @@fetch_status = 0
					BEGIN
						SELECT @ChangeTime = GETDATE()
						SELECT @ChangeId = NEWID()

						UPDATE {{TableName}}
						SET dCreate = @ChangeTime,
							vCreateUser = @SessionUser,
							dModify = @ChangeTime,
							vModifyUser = @SessionUser
						WHERE {{TableName}}.vId = @PrimaryKey

						INSERT INTO {{LogDatabaseName}}.[dbo].{{TableName}}
						(
    						vChangeId,
							vChangeType, 
							dChange, 
							vUserName,  
							vId, 
{{DataColumns}}
							dCreate, 
							vCreateUser,
							dModify,
							vModifyUser
						)
						SELECT 
							@ChangeId,
							'I',
							@ChangeTime,
							@SessionUser,
							vId,
{{DataColumns}}
							@ChangeTime,
							@SessionUser,
							@ChangeTime,
							@SessionUser
						FROM inserted
						WHERE inserted.vId = @PrimaryKey


						FETCH NEXT FROM cursorInserted INTO @PrimaryKey

					END
				CLOSE cursorInserted
				DEALLOCATE cursorInserted
			END
		END

	-- Update
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
		BEGIN
			BEGIN
				DECLARE cursorInserted CURSOR FOR
					SELECT vId
					FROM inserted

				OPEN cursorInserted
				FETCH NEXT FROM cursorInserted into @PrimaryKey
				While @@fetch_status = 0
					BEGIN
						SELECT @ChangeTime = GETDATE()
						SELECT @ChangeId = NEWID()

						UPDATE {{TableName}}
						SET dModify = @ChangeTime,
							vModifyUser = @SessionUser
						WHERE {{TableName}}.vId = @PrimaryKey

						INSERT INTO {{LogDatabaseName}}.[dbo].{{TableName}}
						(
    						vChangeId,
    						vChangeType,
							dChange,
							vUserName,
							vId,
{{DataColumns}}
							dCreate,
							vCreateUser,
							dModify,
							vModifyUser
						)
						SELECT 
							@ChangeId,
							'UO',
							@ChangeTime,
							@SessionUser,
							vId,
{{DataColumns}}
							dCreate,
							vCreateUser,
							dModify,
							vModifyUser
						FROM deleted
						WHERE deleted.vId = @PrimaryKey


						INSERT INTO {{LogDatabaseName}}.[dbo].{{TableName}}
						(
    						vChangeId,
    						vChangeType,
							dChange,
							vUserName,
							vId,
{{DataColumns}}
							dCreate,
							vCreateUser,
							dModify,
							vModifyUser
						)
						SELECT 
							@ChangeId,
							'U',
							@ChangeTime,
							@SessionUser,
							vId,
{{DataColumns}}
							dCreate,
							vCreateUser,
							dModify,
							vModifyUser
						FROM {{TableName}}
						WHERE vId = @PrimaryKey


						FETCH NEXT FROM cursorInserted INTO @PrimaryKey

					END
				CLOSE cursorInserted
				DEALLOCATE cursorInserted
			END
			RETURN;
		END


    -- DELETE
    IF EXISTS (SELECT 1 FROM deleted) AND NOT EXISTS (SELECT 1 FROM inserted)
		BEGIN
			BEGIN
				DECLARE cursorDeleted CURSOR FOR
					SELECT vId
					FROM deleted

				OPEN cursorDeleted
				FETCH NEXT FROM cursorDeleted into @PrimaryKey
				While @@fetch_status = 0
					BEGIN
						SELECT @ChangeTime = GETDATE()
						SELECT @ChangeId = NEWID()

						INSERT INTO {{LogDatabaseName}}.[dbo].{{TableName}}
						(
    						vChangeId,
    						vChangeType,
							dChange,
							vUserName,
							vId,
{{DataColumns}}
							dCreate,
							vCreateUser,
							dModify,
							vModifyUser
						)
						SELECT 
							@ChangeId,
							'D',
							@ChangeTime,
							@SessionUser,
							vId,
{{DataColumns}}
							dCreate,
							vCreateUser,
							dModify,
							vModifyUser
						FROM deleted
						WHERE deleted.vId = @PrimaryKey

						FETCH NEXT FROM cursorDeleted INTO @PrimaryKey

					END
				CLOSE cursorDeleted
				DEALLOCATE cursorDeleted
			END
		END
		RETURN;
END

GO

ALTER TABLE [dbo].[{{TableName}}] ENABLE TRIGGER [{{TableName}}_t1]
GO

