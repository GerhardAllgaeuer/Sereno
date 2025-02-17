CREATE TABLE logChange
(
	vChangeId nvarchar(50) NOT NULL PRIMARY KEY,
	vChangeType nvarchar(10) NOT NULL,
	vPrimaryKey nvarchar(50) NOT NULL,
	vTable nvarchar(50) NOT NULL,
	dChange datetime2 NOT NULL,
	vUserName nvarchar(400),
	tTimestamp timestamp NOT NULL,
	dLog datetime2,
);