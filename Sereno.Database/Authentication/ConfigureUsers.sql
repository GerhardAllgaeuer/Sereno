Select *
from AspNetUsers;

Select *
from AspNetRoles;

INSERT INTO AspNetUserRoles 
VALUES ('66af9b46-08ec-4494-b52d-a3efe1b23c47','32c4e1fb-0b50-4352-9f5c-0825ddba8603')

commit;


Delete AspNetUsers where id in 
('283e2c22-e4a8-41a0-8bd0-1e391633491d',
'431cdbf0-d4cb-4e06-81ff-3781897534c1',
'4d9266c3-3158-4f00-9452-00c895273edc',
'6be91ceb-3cb4-4a4d-85c9-9d8583d7d622',
'8d4de3d6-4235-4092-aac9-d228512c4736',
'b1586724-b003-4a2c-990d-b72857ba92ba',
'b21fb999-363c-4089-8deb-385e6e34dce4')
;

commit