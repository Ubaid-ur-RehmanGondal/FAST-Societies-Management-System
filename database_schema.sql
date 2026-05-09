-- ============================================================
-- FAST SOCIETIES MANAGEMENT SYSTEM — DATABASE SCHEMA
-- Source of Truth: This schema is FINAL.
-- AI INSTRUCTION: Do NOT invent new tables, columns, or 
-- relationships when writing C# code. Use ONLY what is here.
-- ============================================================

USE master;
GO
CREATE DATABASE FASTSocietiesDB;
GO
USE FASTSocietiesDB;
GO

-- ============================================================
-- TABLE 1: Users (all roles in one table)
-- ============================================================
CREATE TABLE Users (
    UserId        INT           IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(100) NOT NULL,
    Email         NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(256) NOT NULL,  -- Store hashed passwords only
    Role          NVARCHAR(20)  NOT NULL CHECK (Role IN ('Student', 'SocietyHead', 'Admin')),
    IsActive      BIT           NOT NULL DEFAULT 1,
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- TABLE 2: Societies
-- ============================================================
CREATE TABLE Societies (
    SocietyId     INT           IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(100) NOT NULL UNIQUE,
    Description   NVARCHAR(500) NULL,
    HeadUserId    INT           NOT NULL,  -- FK to Users
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Pending' 
                                CHECK (Status IN ('Pending', 'Active', 'Suspended', 'Deleted')),
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Society_Head FOREIGN KEY (HeadUserId) REFERENCES Users(UserId)
);

-- ============================================================
-- TABLE 3: Memberships (Students joining Societies)
-- ============================================================
CREATE TABLE Memberships (
    MembershipId  INT           IDENTITY(1,1) PRIMARY KEY,
    UserId        INT           NOT NULL,   -- FK to Users (must be Student role)
    SocietyId     INT           NOT NULL,   -- FK to Societies
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Pending'
                                CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    AppliedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdatedAt     DATETIME      NULL,
    CONSTRAINT FK_Membership_User    FOREIGN KEY (UserId)    REFERENCES Users(UserId),
    CONSTRAINT FK_Membership_Society FOREIGN KEY (SocietyId) REFERENCES Societies(SocietyId),
    CONSTRAINT UQ_Membership UNIQUE (UserId, SocietyId)  -- No duplicate memberships
);

-- ============================================================
-- TABLE 4: Events
-- ============================================================
CREATE TABLE Events (
    EventId       INT           IDENTITY(1,1) PRIMARY KEY,
    SocietyId     INT           NOT NULL,   -- FK to Societies
    Title         NVARCHAR(150) NOT NULL,
    Description   NVARCHAR(1000) NULL,
    EventDate     DATETIME      NOT NULL,
    Venue         NVARCHAR(200) NULL,
    Capacity      INT           NOT NULL DEFAULT 100,
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Pending'
                                CHECK (Status IN ('Pending', 'Approved', 'Cancelled')),
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Event_Society FOREIGN KEY (SocietyId) REFERENCES Societies(SocietyId)
);

-- ============================================================
-- TABLE 5: EventRegistrations (Students registering for Events)
-- ============================================================
CREATE TABLE EventRegistrations (
    RegistrationId INT          IDENTITY(1,1) PRIMARY KEY,
    EventId        INT          NOT NULL,   -- FK to Events
    UserId         INT          NOT NULL,   -- FK to Users
    RegisteredAt   DATETIME     NOT NULL DEFAULT GETDATE(),
    TicketCode     NVARCHAR(50) NOT NULL UNIQUE,  -- Auto-generated unique ticket
    CONSTRAINT FK_EventReg_Event FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_EventReg_User  FOREIGN KEY (UserId)  REFERENCES Users(UserId),
    CONSTRAINT UQ_EventReg UNIQUE (EventId, UserId)  -- One registration per event
);

-- ============================================================
-- TABLE 6: Tasks (assigned by Society Heads to Members)
-- ============================================================
CREATE TABLE Tasks (
    TaskId        INT           IDENTITY(1,1) PRIMARY KEY,
    SocietyId     INT           NOT NULL,
    AssignedToUserId INT        NOT NULL,   -- FK to Users
    AssignedByUserId INT        NOT NULL,   -- FK to Users (Society Head)
    Title         NVARCHAR(200) NOT NULL,
    Description   NVARCHAR(500) NULL,
    DueDate       DATETIME      NULL,
    Status        NVARCHAR(20)  NOT NULL DEFAULT 'Pending'
                                CHECK (Status IN ('Pending', 'InProgress', 'Completed')),
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Task_Society     FOREIGN KEY (SocietyId)       REFERENCES Societies(SocietyId),
    CONSTRAINT FK_Task_AssignedTo  FOREIGN KEY (AssignedToUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Task_AssignedBy  FOREIGN KEY (AssignedByUserId) REFERENCES Users(UserId)
);

-- ============================================================
-- TABLE 7: Announcements
-- ============================================================
CREATE TABLE Announcements (
    AnnouncementId INT          IDENTITY(1,1) PRIMARY KEY,
    SocietyId      INT          NOT NULL,
    PostedByUserId INT          NOT NULL,
    Title          NVARCHAR(200) NOT NULL,
    Content        NVARCHAR(2000) NOT NULL,
    PostedAt       DATETIME     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Ann_Society FOREIGN KEY (SocietyId)      REFERENCES Societies(SocietyId),
    CONSTRAINT FK_Ann_User    FOREIGN KEY (PostedByUserId) REFERENCES Users(UserId)
);

-- ============================================================
-- SEED DATA: Default Admin Account
-- ============================================================
INSERT INTO Users (FullName, Email, PasswordHash, Role)
VALUES ('System Admin', 'admin@fast.edu.pk', 
        'AQAAAAEAACcQAAAAE...', -- Replace with actual hash of 'Admin@1234'
        'Admin');
GO