CREATE TABLE Clubs(
	ClubId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL,
	Name varchar(250) NOT NULL,
	LogoUrl varchar(250) NULL    
);

CREATE TABLE Squads(
	ClubId INTEGER NOT NULL,
	SquadId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL,
	Name varchar(100) NOT NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE Players(
	SquadId INTEGER NOT NULL,
	PlayerId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL,
	DateOfBirth date NOT NULL,
	DominantFoot varchar(1) NOT NULL,
	FirstName varchar(100) NOT NULL,
	LastName varchar(100) NOT NULL,
	Nationality varchar(50) NOT NULL,
	SquadNumber SMALLINT NULL,
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId)
);