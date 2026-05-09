using System.Data;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;
using Microsoft.Data.SqlClient;

namespace FASTSocietiesApp.DAL
{
    /// <summary>
    /// Data Access Layer for Tasks.
    /// Handles parameterized queries for creating and retrieving society tasks.
    /// </summary>
    public class TaskDAL
    {
        /// <summary>
        /// Inserts a new task assigned to a society member.
        /// </summary>
        public bool AssignTask(TaskModel t)
        {
            const string sql =
                "INSERT INTO Tasks (SocietyId, AssignedToUserId, AssignedByUserId, Title, Description, DueDate, Status) " +
                "VALUES (@SocietyId, @AssignedToUserId, @AssignedByUserId, @Title, @Description, @DueDate, 'Pending');";

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = t.SocietyId;
                command.Parameters.Add("@AssignedToUserId", SqlDbType.Int).Value = t.AssignedToUserId;
                command.Parameters.Add("@AssignedByUserId", SqlDbType.Int).Value = t.AssignedByUserId;
                command.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = t.Title;
                command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = (object?)t.Description ?? DBNull.Value;
                command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = (object?)t.DueDate ?? DBNull.Value;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("TaskDAL.AssignTask failed.", ex);
                throw new AppException("Failed to assign the task.", ex);
            }
        }

        /// <summary>
        /// Retrieves all tasks for a specific society, joining user names.
        /// </summary>
        public List<TaskModel> GetTasksBySociety(int societyId)
        {
            const string sql =
                "SELECT t.TaskId, t.SocietyId, t.AssignedToUserId, t.AssignedByUserId, " +
                "       t.Title, t.Description, t.DueDate, t.Status, t.CreatedAt, " +
                "       uTo.FullName AS AssignedToFullName, " +
                "       uBy.FullName AS AssignedByFullName " +
                "FROM Tasks t " +
                "INNER JOIN Users uTo ON uTo.UserId = t.AssignedToUserId " +
                "INNER JOIN Users uBy ON uBy.UserId = t.AssignedByUserId " +
                "WHERE t.SocietyId = @SocietyId " +
                "ORDER BY t.CreatedAt DESC;";

            List<TaskModel> tasks = new List<TaskModel>();

            try
            {
                using SqlConnection connection = new SqlConnection(DbHelper.GetConnectionString());
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@SocietyId", SqlDbType.Int).Value = societyId;

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(MapReaderToTask(reader));
                }
                return tasks;
            }
            catch (Exception ex)
            {
                Logger.Error("TaskDAL.GetTasksBySociety failed.", ex);
                throw new AppException("Failed to load tasks for this society.", ex);
            }
        }

        private static TaskModel MapReaderToTask(SqlDataReader reader)
        {
            int descOrdinal = reader.GetOrdinal("Description");
            int dueDateOrdinal = reader.GetOrdinal("DueDate");

            return new TaskModel
            {
                TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
                SocietyId = reader.GetInt32(reader.GetOrdinal("SocietyId")),
                AssignedToUserId = reader.GetInt32(reader.GetOrdinal("AssignedToUserId")),
                AssignedByUserId = reader.GetInt32(reader.GetOrdinal("AssignedByUserId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal),
                DueDate = reader.IsDBNull(dueDateOrdinal) ? null : reader.GetDateTime(dueDateOrdinal),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                AssignedToFullName = reader.GetString(reader.GetOrdinal("AssignedToFullName")),
                AssignedByFullName = reader.GetString(reader.GetOrdinal("AssignedByFullName"))
            };
        }
    }
}
