CREATE TABLE Clubs(
	ClubId bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	Name varchar(250) NOT NULL,
	Url varchar(250) NULL
);

CREATE TABLE Squads(
	ClubId BIGINT NOT NULL,
	SquadId BIGINT IDENTITY(1,1) PRIMARY KEY  NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	Name varchar(100) NOT NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE Players(
	SquadId bigint NOT NULL,
	PlayerId bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
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
	ClubId bigint NOT NULL,
	CoachId bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	FirstName varchar(100) NOT NULL,
	LastName varchar(100) NOT NULL,
	DateOfBirth date NOT NULL,
	Email varchar(320) NOT NULL,
	Phone varchar(20) NOT NULL,
	Qualifications varchar(MAX) NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE SquadCoaches
(
	SquadId bigint NOT NULL,
	CoachId bigint NOT NULL,
	PRIMARY KEY (SquadId, CoachId),
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId),
	FOREIGN KEY(CoachId) REFERENCES Coaches (CoachId)
);  

CREATE TABLE Events(
	ClubId bigint NOT NULL,
	EventId bigint IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Guid uniqueidentifier NOT NULL UNIQUE,
	Title varchar(250) NOT NULL,
	StartDate datetime2(7) NOT NULL,
	EndDate datetime2(7) NOT NULL,
	Location varchar(400) NOT NULL,
	FOREIGN KEY(ClubId) REFERENCES Clubs (ClubId)
);

CREATE TABLE SquadEvents
(
	SquadId bigint NOT NULL,
	EventId bigint NOT NULL,
	PRIMARY KEY (SquadId, EventId),
	FOREIGN KEY(SquadId) REFERENCES Squads (SquadId),
	FOREIGN KEY(EventId) REFERENCES Events (EventId)
); 

-------------------------------------------------------------------------------------------------------------------------------------------------------------
-----Identity tables
CREATE TABLE AspNetRoleClaims (
	Id int NOT NULL,
	ClaimType nvarchar(2147483647),
	ClaimValue nvarchar(2147483647),
	RoleId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
	CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE ON UPDATE RESTRICT
) go
CREATE INDEX IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims (RoleId) go;

CREATE TABLE AspNetRoles (
	Id nvarchar(450) NOT NULL,
	ConcurrencyStamp nvarchar(2147483647),
	Name nvarchar(256),
	NormalizedName nvarchar(256),
	CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
) go
CREATE INDEX RoleNameIndex ON AspNetRoles (NormalizedName) go;

CREATE TABLE AspNetUserClaims (
	Id int NOT NULL,
	ClaimType nvarchar(2147483647),
	ClaimValue nvarchar(2147483647),
	UserId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
	CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE ON UPDATE RESTRICT
) go
CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims (UserId) go;

CREATE TABLE AspNetUserLogins (
	LoginProvider nvarchar(450) NOT NULL,
	ProviderKey nvarchar(450) NOT NULL,
	ProviderDisplayName nvarchar(2147483647),
	UserId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider,ProviderKey),
	CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE ON UPDATE RESTRICT
) go
CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins (UserId) go;

CREATE TABLE AspNetUserRoles (
	UserId nvarchar(450) NOT NULL,
	RoleId nvarchar(450) NOT NULL,
	CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId,RoleId),
	CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE ON UPDATE RESTRICT,
	CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE ON UPDATE RESTRICT
) go
CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles (RoleId) go
CREATE INDEX IX_AspNetUserRoles_UserId ON AspNetUserRoles (UserId) go;

CREATE TABLE AspNetUserTokens (
	UserId nvarchar(450) NOT NULL,
	LoginProvider nvarchar(450) NOT NULL,
	Name nvarchar(450) NOT NULL,
	Value nvarchar(2147483647),
	CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId,LoginProvider,Name)
) go;

CREATE TABLE AspNetUsers (
	Id nvarchar(450) NOT NULL,
	AccessFailedCount int NOT NULL,
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
) go
CREATE INDEX EmailIndex ON AspNetUsers (NormalizedEmail) go
CREATE UNIQUE INDEX UserNameIndex ON AspNetUsers (NormalizedUserName) go;
