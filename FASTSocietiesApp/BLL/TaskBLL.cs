using FASTSocietiesApp.DAL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.BLL
{
    /// <summary>
    /// Business Logic Layer for Tasks.
    /// Validates society head permissions and input before calling DAL.
    /// </summary>
    public class TaskBLL
    {
        private readonly TaskDAL _taskDal;

        public TaskBLL()
        {
            _taskDal = new TaskDAL();
        }

        /// <summary>
        /// Retrieves tasks for a given society.
        /// </summary>
        public List<TaskModel> GetTasksBySociety(int societyId)
        {
            if (societyId <= 0)
                throw new AppException("Invalid society selection.");

            return _taskDal.GetTasksBySociety(societyId);
        }

        /// <summary>
        /// Assigns a new task. Validates that the current user is a Society Head.
        /// </summary>
        public bool AssignTask(TaskModel t)
        {
            if (SessionManager.LoggedInUserRole != UserRole.SocietyHead)
            {
                throw new AppException("Only a Society Head can assign tasks.");
            }

            if (t.SocietyId <= 0 || t.AssignedToUserId <= 0 || string.IsNullOrWhiteSpace(t.Title))
            {
                throw new AppException("Invalid task details. Title and assignee are required.");
            }

            if (t.DueDate.HasValue && t.DueDate.Value < DateTime.Today)
            {
                throw new AppException("Due date cannot be in the past.");
            }

            // Ensure the Society Head assigning the task is the logged-in user
            t.AssignedByUserId = SessionManager.LoggedInUserId;

            bool success = _taskDal.AssignTask(t);
            if (!success)
            {
                throw new AppException("Failed to assign the task. Please try again.");
            }
            return true;
        }
    }
}
