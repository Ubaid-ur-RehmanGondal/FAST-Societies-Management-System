# AI RULEBOOK — FAST Societies Management System
# This file is the permanent law for all code generation. Never deviate from it.

## TECH STACK (Absolute — Do Not Change)
- Language: C# (.NET 8, Windows Forms)
- Database: SQL Server 2022
- ORM/Data Access: ADO.NET with SqlCommand (NO Entity Framework, NO inline SQL)
- IDE Target: Visual Studio 2022

## ARCHITECTURE — THREE-TIER (Mandatory)
All code MUST follow this exact layer separation:

1. PRESENTATION LAYER (UI)
   - Location: /Forms/ folder
   - Contains ONLY: Form files (.cs + .Designer.cs), event handlers, and UI control logic
   - Must NEVER contain SQL queries, business rules, or direct DB calls
   - Forms call the BLL (Business Logic Layer) only

2. BUSINESS LOGIC LAYER (BLL)
   - Location: /BLL/ folder
   - Contains: All validation logic, calculations, and business rules
   - Calls only DAL methods; never touches SqlConnection directly

3. DATA ACCESS LAYER (DAL)
   - Location: /DAL/ folder
   - Contains: All database operations using parameterized SqlCommand
   - NEVER returns raw DataTable to UI; always returns typed C# objects or Lists

## STRICT CODING RULES
- ALL SQL queries must use parameterized queries (SqlParameter). Concatenated SQL = instant rejection.
- ALL database connections must use using() blocks to ensure disposal.
- NEVER expose raw stack traces in the UI. Catch exceptions in DAL/BLL; show user-friendly messages in Forms.
- Every method must have a single responsibility.
- No method should exceed 40 lines of code. If it does, break it up.
- Use Models (/Models/ folder) for all data transfer between layers (e.g., StudentModel, SocietyModel).

## ERROR HANDLING STANDARD
- DAL methods: wrap in try/catch, log to a static Logger class, re-throw a custom AppException
- BLL methods: catch AppException, add context, re-throw
- Forms: catch at the top level, show MessageBox.Show() with a clean message. Never show exception.Message directly.

## NAMING CONVENTIONS (Strictly Enforced)
- Forms:            PascalCase + "Form" suffix         → LoginForm, StudentDashboardForm
- BLL Classes:      PascalCase + "BLL" suffix          → StudentBLL, SocietyBLL
- DAL Classes:      PascalCase + "DAL" suffix          → StudentDAL, EventDAL
- Model Classes:    PascalCase + "Model" suffix        → StudentModel, EventModel
- Buttons:          "btn" prefix + PascalCase          → btnLogin, btnRegister
- TextBoxes:        "txt" prefix + PascalCase          → txtEmail, txtPassword
- Labels:           "lbl" prefix + PascalCase          → lblStatus, lblWelcome
- DataGridViews:    "dgv" prefix + PascalCase          → dgvEvents, dgvMembers
- ComboBoxes:       "cmb" prefix + PascalCase          → cmbSociety, cmbStatus
- Private fields:   camelCase with underscore prefix   → _studentId, _currentUser
- Local variables:  camelCase                          → studentName, eventDate

## DATABASE CONNECTION
- Connection string stored ONLY in App.config under <connectionStrings>
- Key name: "FASTSocietiesDB"
- Accessed via: ConfigurationManager.ConnectionStrings["FASTSocietiesDB"].ConnectionString
- Never hardcode connection strings anywhere else.

## SESSION/STATE MANAGEMENT
- Create a static class: SessionManager.cs in /Helpers/
- Store: LoggedInUserId (int), LoggedInUserRole (enum: Student, SocietyHead, Admin), LoggedInUserName (string)
- All forms check SessionManager before loading

## FOLDER STRUCTURE
/FASTSocietiesApp
  /Forms
    /Student
    /Society
    /Admin
    /Shared
  /BLL
  /DAL
  /Models
  /Helpers
  /Resources
  App.config