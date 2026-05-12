# FAST Societies Management System

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQLServer-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

A robust, multi-role desktop application built in C# to digitize and manage university student societies. This system replaces manual spreadsheet tracking with a centralized database, handling everything from student registrations and society approvals to task delegation and event ticketing.

## Key Features

* **Role-Based Access Control (RBAC):** Secure routing and distinct dashboards for Students, Society Heads, and System Administrators.
* **Student Portal:** Browse active societies, submit membership applications, and register for university events.
* **Society Head Dashboard:** Review pending memberships, assign tasks to members, and create/manage upcoming events.
* **Admin Control Center:** God-mode oversight to suspend/activate users, approve newly created societies, and monitor university-wide statistics.
* **Programmatic UI:** All user interfaces and dynamic data grids were constructed programmatically via code (bypassing drag-and-drop designers) for absolute control over rendering and state.

## Screenshots
*(Note: You can drag and drop your screenshots here later!)*
* **Admin Dashboard:** Showing active users, societies, and university-wide stats.
* **Student Portal:** Showing event registrations and generated tickets.

## Technical Architecture

This project was strictly engineered using a **3-Tier Architecture** to ensure separation of concerns:
1. **Presentation Layer (UI):** C# WinForms handling user inputs and displaying data. Contains zero database logic.
2. **Business Logic Layer (BLL):** Enforces business rules (e.g., preventing duplicate applications, validating event capacities, enforcing minimum password lengths). 
3. **Data Access Layer (DAL):** The only layer permitted to interact with the database. Utilizes strictly parameterized SQL queries to prevent SQL injection attacks.

## Engineering & Security Highlights

* **Secure Authentication:** Implemented SHA-256 password hashing. Passwords are never stored or transmitted in plain text.
* **Exception Handling:** Aggressive trapping of `SqlExceptions` at the DAL layer, bubbled up as user-friendly `AppExceptions` to prevent exposing raw stack traces.
* **Data Integrity:** Database schema enforces `UNIQUE` constraints and Foreign Key relationships to prevent orphaned records (e.g., users applying to deleted societies).
* **Software Metrics:** Analyzed via Cyclomatic Complexity and evaluated using the Poisson Distribution Reliability Model for fault injection resilience. 

## Tech Stack
* **Language:** C#
* **Framework:** .NET Framework
* **Database:** Microsoft SQL Server (LocalDB)
* **Testing:** Cyclomatic Complexity Analysis, Fault Injection

## How to Run Locally

1. Clone the repository: `git clone https://github.com/Ubaid-ur-RehmanGondal/FAST-Societies-Management-System.git`
2. Open the `.sln` file in Visual Studio.
3. Open **SQL Server Object Explorer**.
4. Run the provided `database_schema.sql` script to generate the tables and seed the initial Admin account.
5. Build and Run the application!
