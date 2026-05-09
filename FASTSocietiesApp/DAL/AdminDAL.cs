using System.Data;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;
using Microsoft.Data.SqlClient;

namespace FASTSocietiesApp.DAL
{
    /// <summary>
    /// Data Access Layer for Admin features.
    /// Handles parameterized queries for users, societies, and reports.
    /// </summary>
    public class AdminDAL
    {
        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        public List<UserModel> GetAllUsers()
        {
            const string sql =
                "SELECT UserId, FullName, Email, Role, IsActive, CreatedAt " +
                "FROM Users " +
                "ORDER BY FullName ASC;";

            List<UserModel> users = new List<UserModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UserModel
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    });
                }
                return users;
            }
            catch (Exception ex)
            {
                Logger.Error("AdminDAL.GetAllUsers failed.", ex);
                throw new AppException("Failed to load users.", ex);
            }
        }

        /// <summary>
        /// Updates the status of a user (e.g., Active, Suspended).
        /// </summary>
        public bool UpdateUserStatus(int userId, bool isActive)
        {
            const string sql = "UPDATE Users SET IsActive = @IsActive WHERE UserId = @UserId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("AdminDAL.UpdateUserStatus failed.", ex);
                string statStr = isActive ? "Active" : "Suspended";
                throw new AppException($"Failed to update user status to {statStr}.", ex);
            }
        }

        /// <summary>
        /// Retrieves all societies regardless of status.
        /// </summary>
        public List<SocietyModel> GetAllSocieties()
        {
            const string sql =
                "SELECT s.SocietyId, s.Name, s.Description, s.HeadUserId, s.Status, " +
                "       s.CreatedAt, u.FullName AS HeadFullName " +
                "FROM Societies s " +
                "INNER JOIN Users u ON u.UserId = s.HeadUserId " +
                "ORDER BY s.Name ASC;";

            List<SocietyModel> societies = new List<SocietyModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int descOrdinal = reader.GetOrdinal("Description");
                    societies.Add(new SocietyModel
                    {
                        SocietyId = reader.GetInt32(reader.GetOrdinal("SocietyId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal),
                        HeadUserId = reader.GetInt32(reader.GetOrdinal("HeadUserId")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        HeadFullName = reader.GetString(reader.GetOrdinal("HeadFullName"))
                    });
                }
                return societies;
            }
            catch (Exception ex)
            {
                Logger.Error("AdminDAL.GetAllSocieties failed.", ex);
                throw new AppException("Failed to load societies.", ex);
            }
        }

        /// <summary>
        /// Creates a new society. Initial status can be Pending or Active.
        /// </summary>
        public bool CreateSociety(SocietyModel s)
        {
            const string sql =
                "INSERT INTO Societies (Name, Description, HeadUserId, Status) " +
                "VALUES (@Name, @Description, @HeadUserId, @Status);";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = s.Name;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = (object?)s.Description ?? DBNull.Value;
                command.Parameters.Add("@HeadUserId", SqlDbType.Int).Value = s.HeadUserId;
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = s.Status;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("AdminDAL.CreateSociety failed.", ex);
                throw new AppException("Failed to create the society.", ex);
            }
        }

        /// <summary>
        /// Updates a society's status (Active, Suspended, etc.).
        /// </summary>
        public bool UpdateSocietyStatus(int societyId, string status)
        {
            const string sql = "UPDATE Societies SET Status = @Status WHERE SocietyId = @SocietyId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = status;
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("AdminDAL.UpdateSocietyStatus failed.", ex);
                throw new AppException($"Failed to update society status to {status}.", ex);
            }
        }

        /// <summary>
        /// Returns university-wide statistics.
        /// </summary>
        public UniversityReportModel GetUniversityWideReport()
        {
            const string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Users WHERE Role = 'Student' AND IsActive = 1) AS TotalStudents,
                    (SELECT COUNT(*) FROM Societies WHERE Status = 'Active') AS TotalActiveSocieties,
                    (SELECT COUNT(*) FROM Events WHERE Status != 'Cancelled') AS TotalEvents;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new UniversityReportModel
                    {
                        TotalStudents = reader.GetInt32(0),
                        TotalActiveSocieties = reader.GetInt32(1),
                        TotalEvents = reader.GetInt32(2)
                    };
                }

                return new UniversityReportModel();
            }
            catch (Exception ex)
            {
                Logger.Error("AdminDAL.GetUniversityWideReport failed.", ex);
                throw new AppException("Failed to generate university-wide report.", ex);
            }
        }
    }
}
