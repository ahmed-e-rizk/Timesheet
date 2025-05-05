Create Database Timesheet;

use Timesheet;
CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    [Password] VARCHAR(255) NOT NULL,
    MobileNumber VARCHAR(20) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE AttendanceLogs  (
    Id INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    Date DATE NOT NULL,
    LoginTime DATETIME,
    LogoutTime DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE  (UserId, Date)
);

CREATE TABLE RefreshToken (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NULL,
    Jti NVARCHAR(MAX) NULL,
    Token NVARCHAR(MAX) NULL,
    ExpireDate DATETIME NULL,
    CreateDate DATETIME NULL,
    CONSTRAINT FK_RefreshToken_User FOREIGN KEY (UserId) REFERENCES Users(Id)
);