# GitHub Repository: https://github.com/VCCT-CLDV6211-2025-G3/ST10280758_EventEase_Part1.git

# EventEase Venue Booking System

## Overview
EventEase is a web-based application built with ASP.NET Core and Entity Framework Core to manage venue bookings. It supports CRUD operations for venues, events, and bookings, using a SQL Server LocalDB database (`EventEaseDB`) on `(localdb)\mssqllocaldb`). The system prevents double bookings and is designed for efficient event scheduling with flexible deployment options.

### Features
- **Venues**: Add, view, edit, and delete venues (name, location, capacity, optional image URL).
- **Events**: Manage events with start/end dates, descriptions, and optional venue assignments.
- **Bookings**: Schedule events at venues with overlap prevention.
- **Database**: `EventEaseDB` with tables: `Venue`, `Event`, `Booking`.

## Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (optional)


## Setup Instructions

### 1.Download
Download the project files manually.

### 2. Restore Dependencies
Install required NuGet packages:
```
dotnet restore
```

### 3. Database Setup
The app uses `(localdb)\mssqllocaldb` with `EventEaseDB`. Choose one method:


#### Option A: Manual SQL Script
1. Open SSMS, connect to `(localdb)\mssqllocaldb`.
2. Run this script:
```
CREATE DATABASE EventEaseDB;
GO
USE EventEaseDB;
GO
CREATE TABLE Venue (
    VenueId INT IDENTITY(1,1) PRIMARY KEY,
    VenueName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl NVARCHAR(500) NULL
);
CREATE TABLE Event (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Description NVARCHAR(500) NULL,
    VenueId INT NULL,
    CONSTRAINT FK_Event_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId)
);
CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    VenueId INT NOT NULL,
    BookingDate DATETIME NOT NULL,
    CONSTRAINT FK_Booking_Event FOREIGN KEY (EventId) REFERENCES Event(EventId),
    CONSTRAINT FK_Booking_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId)
);
```

### 4. Run the App
Launch the application:
```
dotnet run
```
- Access at `https://localhost:5001/Venues` (port may vary—check console).

## Troubleshooting
- **Database Not Found**: Ensure LocalDB is running (`sqllocaldb start MSSQLLocalDB`) and the connection string is correct.
- **Table Errors**: If tables exist, drop them (`DROP TABLE Booking; DROP TABLE Event; DROP TABLE Venue;`) before recreating.
- **Server Explorer Wrong DB**: In VS, add `(localdb)\mssqllocaldb.EventEaseDB` under Data Connections.
- **Build Fails**: Run `dotnet restore` and check for missing packages (e.g., `Microsoft.EntityFrameworkCore.SqlServer`).

## Author
- Justin Fussell (ST10280758, Group 3)