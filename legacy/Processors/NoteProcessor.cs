using Services.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class NoteProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<Note> GetAllNotes(string companyId)
        {
            List<Note> notes = new List<Note>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"
                SELECT 
                    n.Id,
                    n.Description,
                    n.CreatedAt,
                    n.CSLId,
                    n.CustomerId,
                    n.AppointmentId,
                    n.CompanyId,
                    n.UserId,
                    u.FirstName,
                    u.LastName,
                    n.TagId
                FROM [msSchedulerV3].[dbo].[tbl_Note] n
                LEFT JOIN [XinatorCentral].[dbo].[tbl_User] u ON n.UserId = u.UserID and n.companyId = u.companyId
                WHERE n.CompanyId = @CompanyId
                ORDER BY n.CreatedAt DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                        return notes;

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Note note = new Note
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Description = dr["Description"].ToString(),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"]).ToString("yyyy-MM-dd HH:mm:ss"),
                            CSLId = dr["CSLId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["CSLId"]),
                            CustomerId = dr["CustomerId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["CustomerId"]),
                            AppointmentId = dr["AppointmentId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["AppointmentId"]),
                            CompanyId = dr["CompanyId"].ToString(),
                            UserId = dr["UserId"].ToString(),
                            TagId = dr["TagId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TagId"]),
                            UserName = (dr["FirstName"].ToString()) + " "+
                                        (dr["LastName"].ToString())
                        };
                        notes.Add(note);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching notes: " + ex.Message);
            }

            return notes;
        }

        public string SaveNote(Note note)
        {
            string response = "";
            try
            {
                if (string.IsNullOrEmpty(note.Description))
                    return "Description is required.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string insertQuery = @"
                        INSERT INTO [msSchedulerV3].[dbo].[tbl_Note]
                        (Description, CreatedAt, CSLId, CustomerId, AppointmentId,CompanyId,UserId,TagId)
                        VALUES (@Description, @CreatedAt, @CSLId, @CustomerId, @AppointmentId,@CompanyId,@UserId,@TagId);
                        SELECT SCOPE_IDENTITY();";

                        int newId;
                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Description", note.Description);
                            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Parse(note.CreatedAt));
                            cmd.Parameters.AddWithValue("@CSLId", note.CSLId );
                            cmd.Parameters.AddWithValue("@CustomerId", note.CustomerId );
                            cmd.Parameters.AddWithValue("@AppointmentId", note.AppointmentId);
                            cmd.Parameters.AddWithValue("@CompanyId", note.CompanyId);
                            cmd.Parameters.AddWithValue("@UserId", note.UserId);
                            cmd.Parameters.AddWithValue("@TagId", note.TagId);

                            newId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        transaction.Commit();
                        response = $"Note saved successfully with Id {newId}.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        response = "Error: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }
        public string UpdateNote(Note note)
        {
            string response = "";
            try
            {
                if (note.Id <= 0)
                    return "Invalid Note Id.";

                if (string.IsNullOrEmpty(note.Description))
                    return "Description is required.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string checkQuery = "SELECT COUNT(1) FROM [msSchedulerV3].[dbo].[tbl_Note] WHERE Id = @Id and companyId = @companyId";

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", note.Id);
                            checkCmd.Parameters.AddWithValue("@companyId", note.CompanyId);
                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (exists == 0)
                            {
                                transaction.Rollback();
                                return "Note not found.";
                            }
                        }

                        string updateQuery = @"
                        UPDATE [msSchedulerV3].[dbo].[tbl_Note]
                        SET Description = @Description,
                            CreatedAt = @CreatedAt,
                            CSLId = @CSLId,
                            CustomerId = @CustomerId,
                            AppointmentId = @AppointmentId,
                            TagId = @TagId
                        WHERE Id = @Id and CompanyId = @companyId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", note.Id);
                            cmd.Parameters.AddWithValue("@Description", note.Description );
                            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Parse(note.CreatedAt));
                            cmd.Parameters.AddWithValue("@CSLId", note.CSLId );
                            cmd.Parameters.AddWithValue("@CustomerId", note.CustomerId );
                            cmd.Parameters.AddWithValue("@AppointmentId", note.AppointmentId );
                            cmd.Parameters.AddWithValue("@companyId", note.AppointmentId );
                            cmd.Parameters.AddWithValue("@TagId", note.TagId);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        response = $"Note {note.Id} updated successfully.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        response = "Error: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string DeleteNote(int id, string companyId)
        {
            string response = "";
            try
            {
                if (id <= 0)
                    return "Invalid Id.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string checkQuery = "SELECT COUNT(1) FROM [msSchedulerV3].[dbo].[tbl_Note] WHERE Id = @Id and companyId = @companyId";
                        int exists;
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", id);
                            checkCmd.Parameters.AddWithValue("@companyId", companyId);
                            exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        }

                        if (exists == 0)
                        {
                            transaction.Rollback();
                            return "Note not found.";
                        }

                        string deleteQuery = "DELETE FROM [msSchedulerV3].[dbo].[tbl_Note] WHERE Id = @Id and companyId= @companyId";

                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.Parameters.AddWithValue("@companyId", companyId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        response = "Note deleted successfully.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        response = "Error: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }
    }
}
