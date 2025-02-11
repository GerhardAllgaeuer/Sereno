
Alter trigger [dbo].[tstSimple_t1] on [dbo].[tstSimple] 
AFTER INSERT, UPDATE, DELETE AS

Declare @ChangeTime datetime2;
Declare @PrimaryKey varchar(50);
DECLARE @SessionUser NVARCHAR(500);

Begin

    Set nocount on

    -- Benutzer aus SESSION_CONTEXT auslesen und in NVARCHAR konvertieren
    SET @SessionUser = CONVERT(NVARCHAR(500), SESSION_CONTEXT(N'UserName'));

    -- Fallback auf SUSER_NAME(), falls SESSION_CONTEXT keinen Wert liefert
    IF @SessionUser IS NULL
        SET @SessionUser = SUSER_NAME();

    -- INSERT
    IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
		Begin
			Begin
				Declare cursorInserted cursor for
					Select vId
					from inserted

				Open cursorInserted
				Fetch next from cursorInserted into @PrimaryKey
				While @@fetch_status = 0
					Begin
						Select @ChangeTime = GetDate()

						UPDATE tstSimple
						SET dCreate = @ChangeTime,
							vCreateUser = @SessionUser,
							dModify = @ChangeTime,
							vModifyUser = @SessionUser
						WHERE tstSimple.vId = @PrimaryKey

						Insert into SerenoTlDb1TestLog.[dbo].tstSimple
						(
							  vId
    						, vChangeType
							, dChange
							, vUserName 
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						)
						Select 
							  vId
							, 'I'
							, @ChangeTime
							, @SessionUser
                            , vTitle
                            , vDescription
							, @ChangeTime
							, @SessionUser
							, @ChangeTime
							, @SessionUser
						from inserted
						where inserted.vId = @PrimaryKey


						Fetch next from cursorInserted INTO @PrimaryKey

					End
				Close cursorInserted
				Deallocate cursorInserted
			End
		End

	-- Update
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
		Begin
			Begin
				Declare cursorInserted cursor for
					Select vId
					from inserted

				Open cursorInserted
				Fetch next from cursorInserted into @PrimaryKey
				While @@fetch_status = 0
					Begin
						Select @ChangeTime = GetDate()

						UPDATE tstSimple
						SET dModify = @ChangeTime,
							vModifyUser = @SessionUser
						WHERE tstSimple.vId = @PrimaryKey

						Insert into SerenoTlDb1TestLog.[dbo].tstSimple
						(
							  vId
    						, vChangeType
							, dChange
							, vUserName 
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						)
						Select 
							  vId
							, 'UO'
							, @ChangeTime
							, @SessionUser
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						from deleted
						where deleted.vId = @PrimaryKey


						Insert into SerenoTlDb1TestLog.[dbo].tstSimple
						(
							  vId
    						, vChangeType
							, dChange
							, vUserName 
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						)
						Select 
							  vId
							, 'U'
							, @ChangeTime
							, @SessionUser
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						from tstSimple
						where vId = @PrimaryKey


						Fetch next from cursorInserted INTO @PrimaryKey

					End
				Close cursorInserted
				Deallocate cursorInserted
			End
			return;
		End


    -- DELETE
    IF EXISTS (SELECT 1 FROM deleted) AND NOT EXISTS (SELECT 1 FROM inserted)
		Begin
			Begin
				Declare cursorDeleted cursor for
					Select vId
					from deleted

				Open cursorDeleted
				Fetch next from cursorDeleted into @PrimaryKey
				While @@fetch_status = 0
					Begin
						Select @ChangeTime = GetDate()

						Insert into SerenoTlDb1TestLog.[dbo].tstSimple
						(
							  vId
    						, vChangeType
							, dChange
							, vUserName
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						)
						Select 
							  vId
							, 'D'
							, @ChangeTime
							, @SessionUser
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						from deleted
						where deleted.vId = @PrimaryKey

						Fetch next from cursorDeleted INTO @PrimaryKey

					End
				Close cursorDeleted
				Deallocate cursorDeleted
			End
		End
		return;
End

GO

ALTER TABLE [dbo].[tstSimple] ENABLE TRIGGER [tstSimple_t1]
GO


commit

