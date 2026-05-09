using FASTSocietiesApp.DAL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.BLL
{
    /// <summary>
    /// Business Logic Layer for societies and student membership applications.
    /// Owns: input/state validation, duplicate-application prevention.
    /// Calls SocietyDAL only; never opens a SqlConnection itself.
    /// </summary>
    public class SocietyBLL
    {
        private readonly SocietyDAL _societyDal;

        public SocietyBLL()
        {
            _societyDal = new SocietyDAL();
        }

        /// <summary>
        /// Returns the list of societies a student may browse and apply to.
        /// Wraps DAL errors as AppException with BLL-level context.
        /// </summary>
        public List<SocietyModel> GetActiveSocieties()
        {
            try
            {
                return _societyDal.GetAllActiveSocieties();
            }
            catch (AppException ex)
            {
                throw new AppException("Could not load active societies: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns every membership belonging to the given student, ordered
        /// newest first. Validates the user id before delegating to the DAL.
        /// </summary>
        public List<MembershipModel> GetMyMemberships(int userId)
        {
            if (userId <= 0)
                throw new AppException("A valid user must be logged in.");

            try
            {
                return _societyDal.GetMembershipsByUser(userId);
            }
            catch (AppException ex)
            {
                throw new AppException("Could not load your memberships: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Submits a membership application for (userId, societyId).
        /// Validates inputs, ensures the society is Active, and prevents
        /// duplicate applications regardless of prior status.
        /// </summary>
        public bool ApplyForMembership(int userId, int societyId)
        {
            if (userId <= 0)
                throw new AppException("A valid user must be logged in.");
            if (societyId <= 0)
                throw new AppException("Please select a society.");

            try
            {
                if (!_societyDal.IsSocietyActive(societyId))
                {
                    throw new AppException(
                        "This society is not currently accepting applications.");
                }

                if (_societyDal.MembershipExists(userId, societyId))
                {
                    throw new AppException(
                        "You have already applied to this society.");
                }

                bool inserted = _societyDal.ApplyForMembership(userId, societyId);
                if (!inserted)
                {
                    throw new AppException(
                        "Could not submit your application. Please try again.");
                }
                return true;
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyBLL.ApplyForMembership failed.", ex);
                throw new AppException(
                    "Membership application failed due to an unexpected error.", ex);
            }
        }
        /// <summary>
        /// Retrieves pending membership applications for the society.
        /// Validates that the society id is valid.
        /// </summary>
        public List<MembershipModel> GetPendingRequests(int societyId)
        {
            if (societyId <= 0)
                throw new AppException("Invalid society selection.");

            try
            {
                return _societyDal.GetPendingRequests(societyId);
            }
            catch (AppException ex)
            {
                throw new AppException("Could not load pending requests: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Approves or rejects a membership application.
        /// </summary>
        public bool UpdateMembershipStatus(int membershipId, string status)
        {
            if (membershipId <= 0)
                throw new AppException("Invalid membership selection.");

            if (status != "Approved" && status != "Rejected")
                throw new AppException("Invalid status provided.");

            try
            {
                bool updated = _societyDal.UpdateMembershipStatus(membershipId, status);
                if (!updated)
                {
                    throw new AppException($"Could not update application to {status}. It may have already been processed.");
                }
                return true;
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyBLL.UpdateMembershipStatus failed.", ex);
                throw new AppException("Status update failed due to an unexpected error.", ex);
            }
        }
        /// <summary>
        /// Retrieves approved members of the society.
        /// Validates that the society id is valid.
        /// </summary>
        public List<MembershipModel> GetApprovedMembers(int societyId)
        {
            if (societyId <= 0)
                throw new AppException("Invalid society selection.");

            try
            {
                return _societyDal.GetApprovedMembers(societyId);
            }
            catch (AppException ex)
            {
                throw new AppException("Could not load approved members: " + ex.Message, ex);
            }
        }
    }
}
