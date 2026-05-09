# MASTER IMPLEMENTATION PLAN
# AI INSTRUCTION: Build ONLY the current phase task. Do not jump ahead.
# Always ask "what phase am I on?" before generating code.

---

## PHASE 0: Project Setup (Do This First)
- Task 0.1: Create Visual Studio 2022 Windows Forms (.NET 8) solution named "FASTSocietiesApp"
- Task 0.2: Create folder structure: /Forms/Student, /Forms/Society, /Forms/Admin, /Forms/Shared, /BLL, /DAL, /Models, /Helpers
- Task 0.3: Add App.config with connection string key "FASTSocietiesDB"
- Task 0.4: Create SessionManager.cs in /Helpers/ with static properties: LoggedInUserId, LoggedInUserRole, LoggedInUserName
- Task 0.5: Create AppException.cs custom exception class in /Helpers/
- Task 0.6: Create Logger.cs static class in /Helpers/ that writes errors to a local log.txt file
- Task 0.7: Run database_schema.sql to create and seed the database

## PHASE 1: Authentication (Dependency: Phase 0 complete)
- Task 1.1: Create UserModel.cs in /Models/ with properties matching Users table
- Task 1.2: Create UserDAL.cs in /DAL/ with method: UserModel GetUserByEmailAndPassword(string email, string passwordHash)
- Task 1.3: Create UserBLL.cs in /BLL/ with method: UserModel AuthenticateUser(string email, string password) — handles hashing
- Task 1.4: Build LoginForm.cs in /Forms/Shared/ — controls: txtEmail, txtPassword, btnLogin, lblError
- Task 1.5: Wire LoginForm btnLogin to UserBLL.AuthenticateUser(); on success, set SessionManager and open correct dashboard based on Role
- Task 1.6: Build RegisterForm.cs for student self-registration

## PHASE 2: Student Features (Dependency: Phase 1 complete)
- Task 2.1: Create SocietyModel.cs and MembershipModel.cs in /Models/
- Task 2.2: Create SocietyDAL.cs with methods: List<SocietyModel> GetAllActiveSocieties(), bool ApplyForMembership(int userId, int societyId)
- Task 2.3: Create SocietyBLL.cs with wrapper methods and validation
- Task 2.4: Build StudentDashboardForm.cs — controls: dgvSocieties, btnApply, dgvMyMemberships, lblWelcome, btnViewEvents
- Task 2.5: Create EventModel.cs and EventRegistrationModel.cs in /Models/
- Task 2.6: Create EventDAL.cs with: List<EventModel> GetApprovedEvents(), bool RegisterForEvent(int userId, int eventId)
- Task 2.7: Build StudentEventsForm.cs — controls: dgvEvents, btnRegister, dgvMyTickets
- Task 2.8: Build StudentTicketForm.cs showing ticket details (TicketCode, EventTitle, Date, Venue)

## PHASE 3: Society Head Features (Dependency: Phase 2 complete)
- Task 3.1: Create TaskModel.cs and AnnouncementModel.cs in /Models/
- Task 3.2: Add to SocietyDAL.cs: List<MembershipModel> GetPendingRequests(int societyId), bool UpdateMembershipStatus(int membershipId, string status)
- Task 3.3: Create EventDAL.cs methods: bool CreateEvent(EventModel e), bool UpdateEvent(EventModel e), bool CancelEvent(int eventId)
- Task 3.4: Create TaskDAL.cs with: bool AssignTask(TaskModel t), List<TaskModel> GetTasksBySociety(int societyId)
- Task 3.5: Build SocietyDashboardForm.cs — controls: dgvMembers, btnApprove, btnReject, dgvEvents, btnCreateEvent, dgvTasks, btnAssignTask
- Task 3.6: Build CreateEventForm.cs — controls: txtTitle, txtDescription, dtpEventDate, txtVenue, txtCapacity, btnSubmit
- Task 3.7: Build MembershipRequestsForm.cs — controls: dgvRequests, btnApprove, btnReject

## PHASE 4: Admin Features (Dependency: Phase 3 complete)
- Task 4.1: Add to UserDAL.cs: List<UserModel> GetAllUsers(), bool ToggleUserStatus(int userId)
- Task 4.2: Add to SocietyDAL.cs: bool ApproveSociety(int societyId), bool SuspendSociety(int societyId)
- Task 4.3: Add to EventDAL.cs: bool ApproveEvent(int eventId), List<EventModel> GetAllPendingEvents()
- Task 4.4: Build AdminDashboardForm.cs — controls: tabControl with tabs: Users, Societies, Events, Reports
- Task 4.5: Build AdminReportsForm.cs with methods to generate: total members per society, events per month, top active students
- Task 4.6: Export reports to DataGridView with a "Print/Export" button

## PHASE 5: Polish & Integration (Dependency: All phases complete)
- Task 5.1: Add input validation to ALL forms (empty checks, email format, date logic)
- Task 5.2: Add consistent error handling — all btnXxx_Click handlers wrapped in try/catch showing MessageBox
- Task 5.3: Add a MainMenuForm.cs as a navigation hub for each role
- Task 5.4: Add logout functionality that clears SessionManager and returns to LoginForm
- Task 5.5: Final review: check all DAL methods use parameterized queries
- Task 5.6: Add XML documentation comments (///) to every public method (needed for Task 8 Documentation Ratio)