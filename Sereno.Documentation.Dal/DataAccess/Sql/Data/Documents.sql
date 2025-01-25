
INSERT INTO Documents (Id, Title, Content, Created)
VALUES (null, 'Test Title', 'Test Content', null);


INSERT INTO Documents (Title, Content, Created, Updated)
VALUES ('Test Title', 'Test Content', convert(datetime, '2016-10-23 23:59:59.999', 120), convert(datetime, '2016-10-23 23:59:59.999', 120));

commit



Select *
from Documents;
