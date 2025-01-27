use Documentation;

insert into docDocument(vTitle) values ('x');

Select *
from docDocument
order by dModify desc;

Update docDocument set vTitle = 'y' where 1 = 1;

Update docDocument set vTitle = 'z' where vid in
(Select max(vid) from docDocument);

Delete docDocument;

commit;