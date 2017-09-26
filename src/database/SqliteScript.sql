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