CREATE TABLE Clubs(
	ClubId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	Name varchar(250) NOT NULL,
	Url varchar(250) NULL    
);

CREATE TABLE Squads(
	ClubId INTEGER NOT NULL,
	SquadId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	Name varchar(100) NOT NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE Players(
	SquadId INTEGER NOT NULL,
	PlayerId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	DateOfBirth date NOT NULL,
	DominantFoot varchar(1) NOT NULL,
	FirstName varchar(100) NOT NULL,
	LastName varchar(100) NOT NULL,
	Email varchar(320) NOT NULL,
	Nationality varchar(50) NOT NULL,
	SquadNumber SMALLINT NULL,
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId)
);

CREATE TABLE Coaches (
	ClubId INTEGER NOT NULL,
	CoachId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	FirstName varchar(100) NOT NULL,
	LastName varchar(100) NOT NULL,
	DateOfBirth date NOT NULL,
	Email varchar(320) NOT NULL,
	Phone varchar(20) NOT NULL,
	Qualifications varchar(2000) NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE SquadCoaches
(
	SquadId INTEGER NOT NULL,
	CoachId INTEGER NOT NULL,
	PRIMARY KEY (SquadId, CoachId),
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId),
	FOREIGN KEY(CoachId) REFERENCES Coaches (CoachId)
);  

CREATE TABLE Events(
	ClubId INTEGER NOT NULL,
	EventId INTEGER AUTOINCREMENT PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	Title varchar(250) NOT NULL,
	StartDate datetime2(7) NOT NULL,
	EndDate datetime2(7) NOT NULL,
	Location varchar(400) NOT NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE SquadEvents
(
	SquadId INTEGER NOT NULL,
	EventId INTEGER NOT NULL,
	PRIMARY KEY (SquadId, EventId),
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId),
	FOREIGN KEY(EventId) REFERENCES Events (EventId)
); 

CREATE TABLE TrainingMaterials (		
	TrainingMaterialId INTEGER AUTOINCREMENT PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	ClubId bigint NOT NULL,
	Title varchar(250) NOT NULL,
	"Description" varchar(1000) NOT NULL,
	ContentType varchar(100) NOT NULL,
	ExternalId varchar(100) NULL,
	Url Varchar(1000) NULL,
	ThumbnailUrl Varchar(1000) NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE EventTrainingMaterials
(
	EventId INTEGER NOT NULL,
	TrainingMaterialId INTEGER NOT NULL,
	PRIMARY KEY (EventId, TrainingMaterialId),
	FOREIGN KEY(TrainingMaterialId) REFERENCES TrainingMaterials (TrainingMaterialId),
	FOREIGN KEY(EventId) REFERENCES Events (EventId)
); 

CREATE TABLE AspNetRoleClaims (
	Id INTEGER NOT NULL,
	ClaimType nvarchar(2147483647),
	ClaimValue nvarchar(2147483647),
	RoleId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
	CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE ON UPDATE RESTRICT
);

CREATE INDEX IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims (RoleId);

CREATE TABLE AspNetRoles (
	Id nvarchar(450) NOT NULL,
	ConcurrencyStamp nvarchar(2147483647),
	Name nvarchar(256),
	NormalizedName nvarchar(256),
	CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);

CREATE INDEX RoleNameIndex ON AspNetRoles (NormalizedName);

CREATE TABLE AspNetUserClaims (
	Id INTEGER NOT NULL,
	ClaimType nvarchar(2147483647),
	ClaimValue nvarchar(2147483647),
	UserId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
	CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE ON UPDATE RESTRICT
);

CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims (UserId);

CREATE TABLE AspNetUserLogins (
	LoginProvider nvarchar(450) NOT NULL,
	ProviderKey nvarchar(450) NOT NULL,
	ProviderDisplayName nvarchar(2147483647),
	UserId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider,ProviderKey),
	CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE ON UPDATE RESTRICT
);

CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins (UserId);

CREATE TABLE AspNetUserRoles (
	UserId nvarchar(450) NOT NULL,
	RoleId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId,RoleId),
	CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE ON UPDATE RESTRICT,
	CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE ON UPDATE RESTRICT
);

CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles (RoleId);
CREATE INDEX IX_AspNetUserRoles_UserId ON AspNetUserRoles (UserId);

CREATE TABLE AspNetUserTokens (
	UserId nvarchar(450) NOT NULL,
	LoginProvider nvarchar(450) NOT NULL,
	Name nvarchar(450) NOT NULL,
	Value nvarchar(2147483647),
	CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId,LoginProvider,Name)
);

CREATE TABLE AspNetUsers (
	Id nvarchar(450) NOT NULL,
	AccessFailedCount INTEGER NOT NULL,
	ConcurrencyStamp nvarchar(2147483647),
	Email nvarchar(256),
	EmailConfirmed bit NOT NULL,
	LockoutEnabled bit NOT NULL,
	LockoutEnd datetimeoffset,
	NormalizedEmail nvarchar(256),
	NormalizedUserName nvarchar(256),
	PasswordHash nvarchar(2147483647),
	PhoneNumber nvarchar(2147483647),
	PhoneNumberConfirmed bit NOT NULL,
	SecurityStamp nvarchar(2147483647),
	TwoFactorEnabled bit NOT NULL,
	UserName nvarchar(256),
	CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
);

CREATE INDEX EmailIndex ON AspNetUsers (NormalizedEmail);
CREATE UNIQUE INDEX UserNameIndex ON AspNetUsers (NormalizedUserName);
