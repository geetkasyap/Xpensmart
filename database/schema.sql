-- Schema-only export. Create/select your own empty Xpensmart database first.
CREATE TABLE Expensmart_Tbl_Users(
UserID INT IDENTITY(1,1) PRIMARY KEY,
UserName VARCHAR(20) UNIQUE,
Email VARCHAR(100) UNIQUE,
Password VARCHAR(255),
FullName VARCHAR(50),
GroupID INT DEFAULT 0,
IsActive BIT DEFAULT 1,
CreatedOn DATETIME,
ModifiedOn DATETIME,
);
GO

CREATE TABLE Expensmart_Tbl_Categories(
CategoryID INT IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(255),
IsActive BIT DEFAULT 1,
CreatedOn DATETIME,
ModifiedOn DATETIME
);
GO

CREATE TABLE Expensmart_Tbl_Expenses(
ExpenseID INT IDENTITY(1,1) PRIMARY KEY,
UserID INT,
Amount DECIMAL(18,2),
Date DATETIME,
CategoryID INT,
ReceiptUrl VARCHAR(255),
Description	VARCHAR(500),
ExpenseType VARCHAR(255),
IsActive	BIT DEFAULT 1,
IsRecurring BIT DEFAULT 0,
CreatedOn	DATETIME,
ModifiedOn	DATETIME,
CreatedBy VARCHAR(255),
ModifiedBy VARCHAR(255),
);
GO

ALTER TABLE Expensmart_Tbl_Expenses ADD CONSTRAINT UserIDFK1 FOREIGN KEY (UserID) REFERENCES Expensmart_Tbl_Users(UserID);
GO

ALTER TABLE Expensmart_Tbl_Expenses ADD CONSTRAINT CategoryID1 FOREIGN KEY (CategoryID) REFERENCES Expensmart_Tbl_Categories(CategoryID);
GO

CREATE TABLE Expensmart_Tbl_Budgets(
BudgetID INT IDENTITY(1,1) PRIMARY KEY,
UserID INT,
Amount DECIMAL(18,2), 
StartDate DATETIME,
EndDate DATETIME,
IsActive BIT DEFAULT 1,
Frequency VARCHAR(30),
CreatedOn DATETIME,
ModifiedOn DATETIME,
CreatedBy VARCHAR(255),
ModifiedBy VARCHAR(255)
);
GO

ALTER TABLE Expensmart_Tbl_Budgets ADD CONSTRAINT UserIDFK2 FOREIGN KEY (UserID) REFERENCES Expensmart_Tbl_Users(UserID);
GO

CREATE TABLE Expensmart_Tbl_Notifications(
NotificationID INT IDENTITY(1,1) PRIMARY KEY,
UserID INT,
Type VARCHAR(255),
Message TEXT,
IsActive BIT DEFAULT 1,
ReadMessage BIT DEFAULT 0,
CreatedOn DATETIME,
ModifiedOn DATETIME
);
GO

ALTER TABLE Expensmart_Tbl_Notifications ADD CONSTRAINT UserIDFK3 FOREIGN KEY (UserID) REFERENCES Expensmart_Tbl_Users(UserID);
GO

CREATE TABLE Expensmart_Tbl_SharedAccounts
(
SharedAccountID INT IDENTITY(1,1) PRIMARY KEY,
UserID1	INT,
UserID2	INT,
IsActive BIT DEFAULT 1,
CreatedOn DATETIME,
ModifiedOn DATETIME,
CreatedBy VARCHAR(255),
ModifiedBy VARCHAR(255),
);
GO

ALTER TABLE Expensmart_Tbl_SharedAccounts ADD CONSTRAINT UserIDFK9 FOREIGN KEY (UserID1) REFERENCES Expensmart_Tbl_Users(UserID);
GO

ALTER TABLE Expensmart_Tbl_SharedAccounts ADD CONSTRAINT UserIDFK10 FOREIGN KEY (UserID2) REFERENCES Expensmart_Tbl_Users(UserID);
GO

CREATE TABLE Expensmart_Tbl_CurrencyExchange
(
ExchangeID INT IDENTITY(1,1) PRIMARY KEY,
FromCurrency VARCHAR(10),
ToCurrency VARCHAR(10),
Rate DECIMAL(18,4),
IsActive BIT DEFAULT 1,
CreatedOn DATETIME,
ModifiedOn DATETIME
);
GO

CREATE TABLE Expensmart_Tbl_Groups
(
GroupID INT IDENTITY(1,1) PRIMARY KEY,
GroupName VARCHAR(255),
IsActive BIT DEFAULT 1,
CreatedOn DATETIME,
ModifiedOn DATETIME,
CreatedBy VARCHAR(255),
ModifiedBy VARCHAR(255)
);
GO

CREATE TABLE Expensmart_Tbl_CategoryKeywords (
    CategoryKeywordID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL,
    Keyword VARCHAR(255) NOT NULL,
);
GO

ALTER TABLE Expensmart_Tbl_CategoryKeywords ADD CONSTRAINT FKCategoryID2 FOREIGN KEY (CategoryID) REFERENCES Expensmart_Tbl_Categories(CategoryID);
GO

CREATE TABLE Expensmart_Tbl_GroupMembers (
    GroupMemberID INT IDENTITY(1,1) PRIMARY KEY,
    GroupID INT,
    UserID INT,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME,
    ModifiedOn DATETIME,
    CreatedBy VARCHAR(255),
    ModifiedBy VARCHAR(255)
);
GO

ALTER TABLE Expensmart_Tbl_GroupMembers ADD CONSTRAINT FK_GroupID FOREIGN KEY (GroupID) REFERENCES Expensmart_Tbl_Groups(GroupID);
GO

ALTER TABLE Expensmart_Tbl_GroupMembers ADD CONSTRAINT FK_UserID FOREIGN KEY (UserID) REFERENCES Expensmart_Tbl_Users(UserID);
GO

CREATE TABLE Expensmart_Tbl_GroupExpenses (
    GroupExpenseID INT IDENTITY(1,1) PRIMARY KEY,
    GroupID INT,
	ExpenseID INT,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME,
    ModifiedOn DATETIME,
    CreatedBy VARCHAR(255),
    ModifiedBy VARCHAR(255)
);
GO

ALTER TABLE Expensmart_Tbl_GroupExpenses ADD CONSTRAINT FK_GroupID1 FOREIGN KEY (GroupID) REFERENCES Expensmart_Tbl_Groups(GroupID);
GO

ALTER TABLE Expensmart_Tbl_GroupExpenses ADD CONSTRAINT FK_ExpenseID1 FOREIGN KEY (ExpenseID) REFERENCES Expensmart_Tbl_Expenses(ExpenseID);
GO

CREATE TABLE Expensmart_Tbl_AggregateBudget (
    CategoryId INT PRIMARY KEY,
    MinimumPercentage DECIMAL(5, 2),
    MaximumPercentage DECIMAL(5, 2),
    DesiredPercentage DECIMAL(5, 2)
);
GO
