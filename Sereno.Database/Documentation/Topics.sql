With docTopics as
(
	Select left(vLibraryPath, charindex('\', vLibraryPath) - 1) as vTopic
	from docDocument
)
select *
from docTopics
group by vTopic
;