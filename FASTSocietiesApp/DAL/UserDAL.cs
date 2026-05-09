using System.Data;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;
using Microsoft.Data.SqlClient;

namespace FASTSocietiesApp.DAL
{
    /// <summary>
    /// Data Access Layer for the Users table. Performs only parameterized
    /// SQL operations. Wraps low-level errors in AppException and re-throws
    /// after logging. Never references Windows Forms types.
    /// </summary>
    public class UserDAL
    {
        /// <summary>
        /// Looks up an active user by email and password hash.
        /// Returns the matching UserModel, or null if no row matches.
        /// </summary>
        /// <param name="email">User's email address (case-insensitive on SQL Server default collation).</param>
        /// <param name="passwordHash">Pre-hashed password. The DAL never hashes; the BLL does.</param>
        public UserModel? GetUserByEmailAndPassword(string email, string passwordHash)
        {
            const string sql =
                "SELECT UserId, FullName, Email, PasswordHash, Role, IsActive, CreatedAt " +
                "FROM Users " +
                "WHERE Email = @Email AND PasswordHash = @PasswordHash AND IsActive = 1;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = passwordHash;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);

                if (!reader.Read())
                {
                    return null;
                }

                return MapReaderToUser(reader);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("UserDAL.GetUserByEmailAndPassword failed.", ex);
                throw new AppException("Failed to authenticate user against the database.", ex);
            }
        }

        /// <summary>
        /// Returns true if a user with the given email already exists.
        /// Used by registration to enforce the UNIQUE constraint client-side.
        /// </summary>
        public bool EmailExists(string email)
        {
            const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email;";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;

                connection.Open();
                object? result = command.ExecuteScalar();
                int count = result == null ? 0 : Convert.ToInt32(result);
                return count > 0;
            }
            catch (Exception ex)
            {
                Logger.Error("UserDAL.EmailExists failed.", ex);
                throw new AppException("Failed to verify email uniqueness.", ex);
            }
        }

        /// <summary>
        /// Inserts a new user and returns the generated UserId.
        /// PasswordHash must already be hashed by the BLL.
        /// </summary>
        public int InsertUser(UserModel user)
        {
            const string sql =
                "INSERT INTO Users (FullName, Email, PasswordHash, Role, IsActive) " +
                "VALUES (@FullName, @Email, @PasswordHash, @Role, @IsActive); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = user.FullName;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = user.Email;
                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = user.PasswordHash;
                command.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = user.Role;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = user.IsActive;

                connection.Open();
                object? result = command.ExecuteScalar();
                return result == null ? 0 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Logger.Error("UserDAL.InsertUser failed.", ex);
                throw new AppException("Failed to create the user record.", ex);
            }
        }

        /// <summary>
        /// Maps a SqlDataReader row positioned on a Users record to a UserModel.
        /// Centralised to avoid column-index mistakes across methods.
        /// </summary>
        private static UserModel MapReaderToUser(SqlDataReader reader)
        {
            return new UserModel
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Role = reader.GetString(reader.GetOrdinal("Role")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}
