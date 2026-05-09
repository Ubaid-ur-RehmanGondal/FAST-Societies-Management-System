using System.Data;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;
using Microsoft.Data.SqlClient;

namespace FASTSocietiesApp.DAL
{
    /// <summary>
    /// Data Access Layer for Events and EventRegistrations.
    /// Handles parameterized queries and wraps exceptions in AppException.
    /// </summary>
    public class EventDAL
    {
        /// <summary>
        /// Returns all events with Status = 'Approved', including the society name.
        /// Ordered by EventDate ascending.
        /// </summary>
        public List<EventModel> GetApprovedEvents()
        {
            const string sql =
                "SELECT e.EventId, e.SocietyId, e.Title, e.Description, " +
                "       e.EventDate, e.Venue, e.Capacity, e.Status, e.CreatedAt, " +
                "       s.Name AS SocietyName " +
                "FROM Events e " +
                "INNER JOIN Societies s ON s.SocietyId = e.SocietyId " +
                "WHERE e.Status = 'Approved' " +
                "ORDER BY e.EventDate ASC;";

            List<EventModel> events = new List<EventModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    events.Add(MapReaderToEvent(reader));
                }
                return events;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.GetApprovedEvents failed.", ex);
                throw new AppException("Failed to load approved events.", ex);
            }
        }

        /// <summary>
        /// Gets current registration count and total capacity for a specific event.
        /// </summary>
        public (int registrations, int capacity) GetEventCapacityData(int eventId)
        {
            const string sql =
                "SELECT " +
                "  (SELECT COUNT(1) FROM EventRegistrations WHERE EventId = @EventId) AS CurrentRegs, " +
                "  Capacity " +
                "FROM Events " +
                "WHERE EventId = @EventId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int currentRegs = reader.GetInt32(0);
                    int capacity = reader.GetInt32(1);
                    return (currentRegs, capacity);
                }
                throw new AppException("Event not found.");
            }
            catch (AppException) { throw; }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.GetEventCapacityData failed.", ex);
                throw new AppException("Failed to check event capacity.", ex);
            }
        }

        /// <summary>
        /// Checks if a user is already registered for a specific event.
        /// </summary>
        public bool RegistrationExists(int userId, int eventId)
        {
            const string sql =
                "SELECT COUNT(1) FROM EventRegistrations " +
                "WHERE UserId = @UserId AND EventId = @EventId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;

                connection.Open();
                object? result = command.ExecuteScalar();
                int count = result == null ? 0 : Convert.ToInt32(result);
                return count > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.RegistrationExists failed.", ex);
                throw new AppException("Failed to verify existing registration.", ex);
            }
        }

        /// <summary>
        /// Registers a user for an event, storing a unique ticket code.
        /// </summary>
        public bool RegisterForEvent(int userId, int eventId, string ticketCode)
        {
            const string sql =
                "INSERT INTO EventRegistrations (EventId, UserId, TicketCode) " +
                "VALUES (@EventId, @UserId, @TicketCode);";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@TicketCode", SqlDbType.NVarChar, 50).Value = ticketCode;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.RegisterForEvent failed.", ex);
                throw new AppException("Failed to register for the event.", ex);
            }
        }

        /// <summary>
        /// Retrieves all event registrations (tickets) for a specific user.
        /// Ordered by most recently registered.
        /// </summary>
        public List<EventRegistrationModel> GetMyTickets(int userId)
        {
            const string sql =
                "SELECT r.RegistrationId, r.EventId, r.UserId, r.RegisteredAt, r.TicketCode, " +
                "       e.Title AS EventTitle, e.EventDate, e.Venue " +
                "FROM EventRegistrations r " +
                "INNER JOIN Events e ON e.EventId = r.EventId " +
                "WHERE r.UserId = @UserId " +
                "ORDER BY r.RegisteredAt DESC;";

            List<EventRegistrationModel> tickets = new List<EventRegistrationModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tickets.Add(MapReaderToRegistration(reader));
                }
                return tickets;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.GetMyTickets failed.", ex);
                throw new AppException("Failed to load your event tickets.", ex);
            }
        }

        /// <summary>
        /// Inserts a new event. The initial status is 'Pending'.
        /// </summary>
        public bool CreateEvent(EventModel e)
        {
            const string sql =
                "INSERT INTO Events (SocietyId, Title, Description, EventDate, Venue, Capacity, Status) " +
                "VALUES (@SocietyId, @Title, @Description, @EventDate, @Venue, @Capacity, 'Pending');";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = e.SocietyId;
                command.Parameters.Add("@Title", SqlDbType.NVarChar, 150).Value = e.Title;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value = (object?)e.Description ?? DBNull.Value;
                command.Parameters.Add("@EventDate", SqlDbType.DateTime).Value = e.EventDate;
                command.Parameters.Add("@Venue", SqlDbType.NVarChar, 200).Value = (object?)e.Venue ?? DBNull.Value;
                command.Parameters.Add("@Capacity", SqlDbType.Int).Value = e.Capacity;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.CreateEvent failed.", ex);
                throw new AppException("Failed to create the event.", ex);
            }
        }

        /// <summary>
        /// Updates an existing event. Resets status to 'Pending' so Admin can re-approve.
        /// </summary>
        public bool UpdateEvent(EventModel e)
        {
            const string sql =
                "UPDATE Events " +
                "SET Title = @Title, Description = @Description, EventDate = @EventDate, " +
                "    Venue = @Venue, Capacity = @Capacity, Status = 'Pending' " +
                "WHERE EventId = @EventId AND SocietyId = @SocietyId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@EventId", SqlDbType.Int).Value = e.EventId;
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = e.SocietyId;
                command.Parameters.Add("@Title", SqlDbType.NVarChar, 150).Value = e.Title;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value = (object?)e.Description ?? DBNull.Value;
                command.Parameters.Add("@EventDate", SqlDbType.DateTime).Value = e.EventDate;
                command.Parameters.Add("@Venue", SqlDbType.NVarChar, 200).Value = (object?)e.Venue ?? DBNull.Value;
                command.Parameters.Add("@Capacity", SqlDbType.Int).Value = e.Capacity;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.UpdateEvent failed.", ex);
                throw new AppException("Failed to update the event.", ex);
            }
        }

        /// <summary>
        /// Cancels an event by setting its status to 'Cancelled'.
        /// </summary>
        public bool CancelEvent(int eventId)
        {
            const string sql =
                "UPDATE Events SET Status = 'Cancelled' WHERE EventId = @EventId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("EventDAL.CancelEvent failed.", ex);
                throw new AppException("Failed to cancel the event.", ex);
            }
        }

        private static EventModel MapReaderToEvent(SqlDataReader reader)
        {
            int descOrdinal = reader.GetOrdinal("Description");
            int venueOrdinal = reader.GetOrdinal("Venue");

            return new EventModel
            {
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                SocietyId = reader.GetInt32(reader.GetOrdinal("SocietyId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal),
                EventDate = reader.GetDateTime(reader.GetOrdinal("EventDate")),
                Venue = reader.IsDBNull(venueOrdinal) ? null : reader.GetString(venueOrdinal),
                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                SocietyName = reader.GetString(reader.GetOrdinal("SocietyName"))
            };
        }

        private static EventRegistrationModel MapReaderToRegistration(SqlDataReader reader)
        {
            int venueOrdinal = reader.GetOrdinal("Venue");

            return new EventRegistrationModel
            {
                RegistrationId = reader.GetInt32(reader.GetOrdinal("RegistrationId")),
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                RegisteredAt = reader.GetDateTime(reader.GetOrdinal("RegisteredAt")),
                TicketCode = reader.GetString(reader.GetOrdinal("TicketCode")),
                EventTitle = reader.GetString(reader.GetOrdinal("EventTitle")),
                EventDate = reader.GetDateTime(reader.GetOrdinal("EventDate")),
                Venue = reader.IsDBNull(venueOrdinal) ? null : reader.GetString(venueOrdinal)
            };
        }
    }
}
