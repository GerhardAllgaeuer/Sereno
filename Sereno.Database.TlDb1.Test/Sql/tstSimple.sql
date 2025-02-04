
Insert into tstSimple
(vId, vTitle, vDescription)
values
(newid(), 'Title 1', 'Beschreibung 1');

commit;


Delete tstSimple;

commit;


Update tstSimple
set vTitle = 'Altered'
where vId in (Select max(vId) from tstSimple);

commit;


Select *
from tstSimple;


Select *
from SerenoTlDb1TestLog.dbo.tstSimple;

Delete SerenoTlDb1TestLog.dbo.tstSimple;

commit;



