
Create trigger [dbo].[tstSimple_ti2] on [dbo].[tstSimple] for update, delete as

Declare  @vChangeType varchar(2);
Declare  @vChangeTypeOri varchar(2);
Declare  @dChange datetime;
Declare  @vPrimaryKeyOld varchar(50);
DECLARE @SessionUser NVARCHAR(500);
Declare  @lcLngSeq   char(14);
Declare  @lcPKHeader char(4);

Begin

    Set nocount on

    -- Benutzer aus SESSION_CONTEXT auslesen und in NVARCHAR konvertieren
    SET @SessionUser = CONVERT(NVARCHAR(500), SESSION_CONTEXT(N'UserName'));

    -- Fallback auf SUSER_NAME(), falls SESSION_CONTEXT keinen Wert liefert
    IF @SessionUser IS NULL
        SET @SessionUser = SUSER_NAME();


	-- Update
	If exists (Select * from inserted) and exists (Select * from deleted)
		Begin
			Begin
				Declare cursorInserted cursor for
					Select vId
					from inserted

				Open cursorInserted
				Fetch next from cursorInserted into @vPrimaryKeyOld
				While @@fetch_status = 0
					Begin
						Select @vChangeType = 'U'
						Select @vChangeTypeOri = 'UO'
						Select @dChange = GetDate()

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
							, @vChangeTypeOri
							, @dChange
							, @SessionUser
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						from deleted
						where deleted.vId = @vPrimaryKeyOld


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
							, @vChangeType
							, @dChange
							, @SessionUser
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						from tstSimple
						where vId = @vPrimaryKeyOld


						Fetch next from cursorInserted INTO @vPrimaryKeyOld

					End
				Close cursorInserted
				Deallocate cursorInserted
			End
		End


	-- Delete
	Else
		Begin
			Begin
				Declare cursorDeleted cursor for
					Select vId
					from deleted

				Open cursorDeleted
				Fetch next from cursorDeleted into @vPrimaryKeyOld
				While @@fetch_status = 0
					Begin
						Select @vChangeType = 'D'
						Select @dChange = GetDate()

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
							, @vChangeType
							, @dChange
							, @SessionUser
                            , vTitle
                            , vDescription
							, dCreate
							, vCreateUser
							, dModify
							, vModifyUser
						from deleted
						where deleted.vId = @vPrimaryKeyOld

						Fetch next from cursorDeleted INTO @vPrimaryKeyOld

					End
				Close cursorDeleted
				Deallocate cursorDeleted
			End
		End
End

GO

ALTER TABLE [dbo].[tstSimple] ENABLE TRIGGER [tstSimple_ti2]
GO


commit

