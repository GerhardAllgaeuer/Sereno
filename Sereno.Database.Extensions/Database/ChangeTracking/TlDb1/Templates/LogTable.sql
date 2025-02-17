CREATE TABLE ctrLog
(
	vChangeId nvarchar(50) NOT NULL,
	vChangeType nvarchar(10) NOT NULL,
	vPrimaryKey nvarchar(50) NOT NULL,
	vTable nvarchar(50) NOT NULL,
	vUserName nvarchar(400),
	dChange datetime2 NOT NULL,
	dLog datetime2,
	tTimestamp timestamp NOT NULL,
);