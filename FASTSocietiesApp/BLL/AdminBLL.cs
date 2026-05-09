using FASTSocietiesApp.DAL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.BLL
{
    /// <summary>
    /// Business Logic Layer for Admin operations.
    /// Strictly validates that the current user is an Admin.
    /// </summary>
    public class AdminBLL
    {
        private readonly AdminDAL _adminDal;

        public AdminBLL()
        {
            _adminDal = new AdminDAL();
        }

        private void ValidateAdminRole()
        {
            if (SessionManager.LoggedInUserRole != UserRole.Admin)
            {
                throw new AppException("Access denied: Admin privileges required.");
            }
        }

        public List<UserModel> GetAllUsers()
        {
            ValidateAdminRole();
            return _adminDal.GetAllUsers();
        }

        public bool UpdateUserStatus(int userId, bool isActive)
        {
            ValidateAdminRole();

            if (userId <= 0)
                throw new AppException("Invalid user details.");

            if (userId == SessionManager.LoggedInUserId)
                throw new AppException("You cannot change your own status.");

            bool success = _adminDal.UpdateUserStatus(userId, isActive);
            if (!success)
            {
                string statStr = isActive ? "Active" : "Suspended";
                throw new AppException($"Failed to update user status to {statStr}.");
            }
            return true;
        }

        public List<SocietyModel> GetAllSocieties()
        {
            ValidateAdminRole();
            return _adminDal.GetAllSocieties();
        }

        public bool CreateSociety(SocietyModel s)
        {
            ValidateAdminRole();

            if (string.IsNullOrWhiteSpace(s.Name) || s.HeadUserId <= 0)
                throw new AppException("Society Name and a valid Head User are required.");

            if (string.IsNullOrWhiteSpace(s.Status))
                s.Status = "Active";

            return _adminDal.CreateSociety(s);
        }

        public bool UpdateSocietyStatus(int societyId, string status)
        {
            ValidateAdminRole();

            if (societyId <= 0 || string.IsNullOrWhiteSpace(status))
                throw new AppException("Invalid society details.");

            bool success = _adminDal.UpdateSocietyStatus(societyId, status);
            if (!success)
            {
                throw new AppException($"Failed to update society status to {status}.");
            }
            return true;
        }

        public List<EventModel> GetPendingEvents()
        {
            ValidateAdminRole();
            return _adminDal.GetPendingEvents();
        }

        public bool ApproveEvent(int eventId)
        {
            ValidateAdminRole();

            if (eventId <= 0)
                throw new AppException("Invalid event details.");

            bool success = _adminDal.ApproveEvent(eventId);
            if (!success)
            {
                throw new AppException("Failed to approve event.");
            }
            return true;
        }

        public UniversityReportModel GetUniversityWideReport()
        {
            ValidateAdminRole();
            return _adminDal.GetUniversityWideReport();
        }
    }
}
