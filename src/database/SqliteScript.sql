CREATE TABLE [Clubs](
	[ClubId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[LogoUrl] [varchar](250) NULL    
)