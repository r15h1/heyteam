CREATE TABLE [Clubs](
	[ClubId] [bigint] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[LogoUrl] [varchar](250) NULL,
    CONSTRAINT [PK_Clubs] PRIMARY KEY 
    (
        [ClubId] ASC
    )
)