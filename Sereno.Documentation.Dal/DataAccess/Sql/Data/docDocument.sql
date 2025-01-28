use Documentation;

Select *
from docDocument
order by dModify desc;


insert into docDocument(vTitle) values ('x');

Update docDocument set vTitle = 'y' where 1 = 1;

Update docDocument set vTitle = 'z' where vid in
(Select max(vid) from docDocument);

Delete docDocument;

commit;





use DocumentationLog;

Select *
from docDocument
order by dModify desc;

insert into docDocument(vChangeType, dChange, vId, vTitle) values ('U', GETDATE(), NEWID(), 'x');