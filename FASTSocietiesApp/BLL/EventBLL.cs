using FASTSocietiesApp.DAL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.BLL
{
    /// <summary>
    /// Business Logic Layer for Events. Handles validation, formatting,
    /// and business rules before interacting with the DAL.
    /// </summary>
    public class EventBLL
    {
        private readonly EventDAL _eventDal;

        public EventBLL()
        {
            _eventDal = new EventDAL();
        }

        /// <summary>
        /// Retrieves all approved events.
        /// </summary>
        public List<EventModel> GetApprovedEvents()
        {
            return _eventDal.GetApprovedEvents();
        }

        /// <summary>
        /// Retrieves all event tickets for the specified user.
        /// </summary>
        public List<EventRegistrationModel> GetMyTickets(int userId)
        {
            if (userId <= 0)
            {
                throw new AppException("Invalid user session.");
            }
            return _eventDal.GetMyTickets(userId);
        }

        /// <summary>
        /// Registers a user for an event after validating capacity and duplicate registration.
        /// Generates a unique ticket code.
        /// </summary>
        public void RegisterForEvent(int userId, int eventId)
        {
            if (userId <= 0 || eventId <= 0)
            {
                throw new AppException("Invalid user or event data.");
            }

            if (_eventDal.RegistrationExists(userId, eventId))
            {
                throw new AppException("You are already registered for this event.");
            }

            var (currentRegs, capacity) = _eventDal.GetEventCapacityData(eventId);
            if (currentRegs >= capacity)
            {
                throw new AppException("This event has reached its maximum capacity. No more tickets available.");
            }

            string randomChars = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            string ticketCode = $"TKT-EV{eventId}-USR{userId}-{randomChars}";

            bool success = _eventDal.RegisterForEvent(userId, eventId, ticketCode);
            if (!success)
            {
                throw new AppException("Could not complete the registration at this time.");
            }
        }
        /// <summary>
        /// Creates a new event. Validates that the current user is a Society Head.
        /// </summary>
        public bool CreateEvent(EventModel e)
        {
            if (SessionManager.LoggedInUserRole != UserRole.SocietyHead)
            {
                throw new AppException("Only a Society Head can create an event.");
            }

            if (e.SocietyId <= 0 || string.IsNullOrWhiteSpace(e.Title) || e.Capacity <= 0)
            {
                throw new AppException("Invalid event details. Title and positive capacity are required.");
            }

            if (e.EventDate <= DateTime.Now)
            {
                throw new AppException("Event date must be in the future.");
            }

            return _eventDal.CreateEvent(e);
        }

        /// <summary>
        /// Updates an existing event. Validates user role.
        /// </summary>
        public bool UpdateEvent(EventModel e)
        {
            if (SessionManager.LoggedInUserRole != UserRole.SocietyHead)
            {
                throw new AppException("Only a Society Head can update an event.");
            }

            if (e.EventId <= 0 || e.SocietyId <= 0 || string.IsNullOrWhiteSpace(e.Title) || e.Capacity <= 0)
            {
                throw new AppException("Invalid event details.");
            }

            bool success = _eventDal.UpdateEvent(e);
            if (!success)
            {
                throw new AppException("Could not update the event. It may not exist.");
            }
            return true;
        }

        /// <summary>
        /// Cancels an event. Validates user role.
        /// </summary>
        public bool CancelEvent(int eventId)
        {
            if (SessionManager.LoggedInUserRole != UserRole.SocietyHead)
            {
                throw new AppException("Only a Society Head can cancel an event.");
            }

            if (eventId <= 0)
            {
                throw new AppException("Invalid event ID.");
            }

            bool success = _eventDal.CancelEvent(eventId);
            if (!success)
            {
                throw new AppException("Could not cancel the event. It may not exist.");
            }
            return true;
        }
    }
}
