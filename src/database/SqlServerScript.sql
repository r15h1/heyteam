CREATE TABLE Clubs(
	ClubId bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL,
	Name varchar(250) NOT NULL,
	LogoUrl varchar(250) NULL
);

CREATE TABLE Squads(
	ClubId BIGINT NOT NULL,
	SquadId BIGINT IDENTITY(1,1) PRIMARY KEY  NOT NULL,
	Guid uniqueidentifier NOT NULL,
	Name varchar(100) NOT NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE Players(
	SquadId bigint NOT NULL,
	PlayerId bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL,
	DateOfBirth date NOT NULL,
	DominantFoot char(1) NOT NULL,
	FirstName varchar(100) NOT NULL,
	LastName varchar(100) NOT NULL,
	Nationality varchar(50) NOT NULL,
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId)
);