using System.Data;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;
using Microsoft.Data.SqlClient;

namespace FASTSocietiesApp.DAL
{
    /// <summary>
    /// Data Access Layer for the Societies and Memberships tables. All SQL is
    /// parameterized; no SqlConnection escapes a using() block; raw exceptions
    /// are logged and re-thrown as AppException.
    /// </summary>
    public class SocietyDAL
    {
        /// <summary>
        /// Returns all societies whose Status = 'Active', joined with the
        /// head user's full name. Sorted alphabetically by society name.
        /// </summary>
        public List<SocietyModel> GetAllActiveSocieties()
        {
            const string sql =
                "SELECT s.SocietyId, s.Name, s.Description, s.HeadUserId, " +
                "       u.FullName AS HeadFullName, s.Status, s.CreatedAt " +
                "FROM Societies s " +
                "INNER JOIN Users u ON u.UserId = s.HeadUserId " +
                "WHERE s.Status = 'Active' " +
                "ORDER BY s.Name;";

            List<SocietyModel> societies = new List<SocietyModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    societies.Add(MapReaderToSociety(reader));
                }
                return societies;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.GetAllActiveSocieties failed.", ex);
                throw new AppException("Failed to load active societies.", ex);
            }
        }

        /// <summary>
        /// Returns true if the user already has a membership row for the
        /// given society, regardless of its status. Used by the BLL to
        /// short-circuit duplicate applications.
        /// </summary>
        public bool MembershipExists(int userId, int societyId)
        {
            const string sql =
                "SELECT COUNT(1) FROM Memberships " +
                "WHERE UserId = @UserId AND SocietyId = @SocietyId;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                object? result = command.ExecuteScalar();
                int count = result == null ? 0 : Convert.ToInt32(result);
                return count > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.MembershipExists failed.", ex);
                throw new AppException("Failed to verify existing membership.", ex);
            }
        }

        /// <summary>
        /// Returns true if the society exists and is in 'Active' status.
        /// Used as a guard before inserting an application.
        /// </summary>
        public bool IsSocietyActive(int societyId)
        {
            const string sql =
                "SELECT COUNT(1) FROM Societies " +
                "WHERE SocietyId = @SocietyId AND Status = 'Active';";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                object? result = command.ExecuteScalar();
                int count = result == null ? 0 : Convert.ToInt32(result);
                return count > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.IsSocietyActive failed.", ex);
                throw new AppException("Failed to verify society status.", ex);
            }
        }

        /// <summary>
        /// Inserts a new pending membership for (userId, societyId).
        /// Returns true on a single-row insert. The caller (BLL) is
        /// responsible for ensuring no duplicate exists.
        /// </summary>
        public bool ApplyForMembership(int userId, int societyId)
        {
            const string sql =
                "INSERT INTO Memberships (UserId, SocietyId, Status) " +
                "VALUES (@UserId, @SocietyId, 'Pending');";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.ApplyForMembership failed.", ex);
                throw new AppException("Failed to submit membership application.", ex);
            }
        }

        /// <summary>
        /// Returns every membership row belonging to the given student,
        /// joined with the society name for display. Ordered newest first.
        /// </summary>
        public List<MembershipModel> GetMembershipsByUser(int userId)
        {
            const string sql =
                "SELECT m.MembershipId, m.UserId, m.SocietyId, m.Status, " +
                "       m.AppliedAt, m.UpdatedAt, s.Name AS SocietyName " +
                "FROM Memberships m " +
                "INNER JOIN Societies s ON s.SocietyId = m.SocietyId " +
                "WHERE m.UserId = @UserId " +
                "ORDER BY m.AppliedAt DESC;";

            List<MembershipModel> memberships = new List<MembershipModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    memberships.Add(MapReaderToMembership(reader));
                }
                return memberships;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.GetMembershipsByUser failed.", ex);
                throw new AppException("Failed to load your memberships.", ex);
            }
        }

        /// <summary>
        /// Retrieves pending membership applications for a specific society.
        /// Includes the applicant's full name.
        /// </summary>
        public List<MembershipModel> GetPendingRequests(int societyId)
        {
            const string sql =
                "SELECT m.MembershipId, m.UserId, m.SocietyId, m.Status, " +
                "       m.AppliedAt, m.UpdatedAt, u.FullName AS ApplicantFullName, " +
                "       s.Name AS SocietyName " +
                "FROM Memberships m " +
                "INNER JOIN Users u ON u.UserId = m.UserId " +
                "INNER JOIN Societies s ON s.SocietyId = m.SocietyId " +
                "WHERE m.SocietyId = @SocietyId AND m.Status = 'Pending' " +
                "ORDER BY m.AppliedAt ASC;";

            List<MembershipModel> requests = new List<MembershipModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    requests.Add(MapReaderToMembershipRequest(reader));
                }
                return requests;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.GetPendingRequests failed.", ex);
                throw new AppException("Failed to load pending membership requests.", ex);
            }
        }

        /// <summary>
        /// Updates a membership application status ('Approved' or 'Rejected').
        /// Also updates the UpdatedAt timestamp.
        /// </summary>
        public bool UpdateMembershipStatus(int membershipId, string status)
        {
            const string sql =
                "UPDATE Memberships " +
                "SET Status = @Status, UpdatedAt = GETDATE() " +
                "WHERE MembershipId = @MembershipId AND Status = 'Pending';";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = status;
                command.Parameters.Add("@MembershipId", SqlDbType.Int).Value = membershipId;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.UpdateMembershipStatus failed.", ex);
                throw new AppException($"Failed to mark application as {status}.", ex);
            }
        }

        /// <summary>
        /// Retrieves approved members of a specific society.
        /// </summary>
        public List<MembershipModel> GetApprovedMembers(int societyId)
        {
            const string sql =
                "SELECT m.MembershipId, m.UserId, m.SocietyId, m.Status, " +
                "       m.AppliedAt, m.UpdatedAt, u.FullName AS ApplicantFullName, " +
                "       s.Name AS SocietyName " +
                "FROM Memberships m " +
                "INNER JOIN Users u ON u.UserId = m.UserId " +
                "INNER JOIN Societies s ON s.SocietyId = m.SocietyId " +
                "WHERE m.SocietyId = @SocietyId AND m.Status = 'Approved' " +
                "ORDER BY u.FullName ASC;";

            List<MembershipModel> members = new List<MembershipModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    members.Add(MapReaderToMembershipRequest(reader));
                }
                return members;
            }
            catch (Exception ex)
            {
                Logger.Error("SocietyDAL.GetApprovedMembers failed.", ex);
                throw new AppException("Failed to load society members.", ex);
            }
        }

        /// <summary>
        /// Maps a SqlDataReader row from the Societies-with-head query.
        /// </summary>
        private static SocietyModel MapReaderToSociety(SqlDataReader reader)
        {
            int descOrdinal = reader.GetOrdinal("Description");
            return new SocietyModel
            {
                SocietyId = reader.GetInt32(reader.GetOrdinal("SocietyId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal),
                HeadUserId = reader.GetInt32(reader.GetOrdinal("HeadUserId")),
                HeadFullName = reader.GetString(reader.GetOrdinal("HeadFullName")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        /// <summary>
        /// Maps a SqlDataReader row from the Memberships-with-society query.
        /// </summary>
        private static MembershipModel MapReaderToMembership(SqlDataReader reader)
        {
            int updatedOrdinal = reader.GetOrdinal("UpdatedAt");
            return new MembershipModel
            {
                MembershipId = reader.GetInt32(reader.GetOrdinal("MembershipId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                SocietyId = reader.GetInt32(reader.GetOrdinal("SocietyId")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AppliedAt = reader.GetDateTime(reader.GetOrdinal("AppliedAt")),
                UpdatedAt = reader.IsDBNull(updatedOrdinal)
                    ? null
                    : reader.GetDateTime(updatedOrdinal),
                SocietyName = reader.GetString(reader.GetOrdinal("SocietyName"))
            };
        }

        /// <summary>
        /// Maps a SqlDataReader row from the Memberships-with-user query.
        /// </summary>
        private static MembershipModel MapReaderToMembershipRequest(SqlDataReader reader)
        {
            int updatedOrdinal = reader.GetOrdinal("UpdatedAt");
            int applicantOrdinal = reader.GetOrdinal("ApplicantFullName");
            int societyNameOrdinal = reader.GetOrdinal("SocietyName");

            return new MembershipModel
            {
                MembershipId = reader.GetInt32(reader.GetOrdinal("MembershipId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                SocietyId = reader.GetInt32(reader.GetOrdinal("SocietyId")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AppliedAt = reader.GetDateTime(reader.GetOrdinal("AppliedAt")),
                UpdatedAt = reader.IsDBNull(updatedOrdinal) ? null : reader.GetDateTime(updatedOrdinal),
                ApplicantFullName = reader.GetString(applicantOrdinal),
                SocietyName = reader.GetString(societyNameOrdinal)
            };
        }
    }
}
