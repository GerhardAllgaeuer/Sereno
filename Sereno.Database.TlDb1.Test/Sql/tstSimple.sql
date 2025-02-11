
Delete tstSimple;

Insert into tstSimple(vId, vTitle, vDescription)
values (newid(), 'asfb', 'asdf');

Delete SerenoTlDb1TestLog.dbo.tstSimple;

Update tstSimple set vTitle = vTitle + ' ' + convert(nvarchar(50), GETDATE());

Delete SerenoTlDb1TestLog.dbo.tstSimple;

Delete tstSimple;

Select *
from tstSimple;

Select *
from SerenoTlDb1TestLog.dbo.tstSimple;


commit;