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
    public class FormProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        Database db = new Database();

        public List<FormTemplate> GetAllFormTemplates(string companyId)
        {
            var response = new List<FormTemplate>();
            try
            {
                string sql = @"
                Select * from myServiceJobs.dbo.FormTemplates where companyid= @CompanyID
                ";
                DataSet dataSet = db.Get_DataSet(sql, companyId);
               
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                return response;
                DataTable dt = dataSet.Tables[0];
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    FormTemplate template = new FormTemplate
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        CompanyID = dr["CompanyID"].ToString(),
                        TemplateName = dr["TemplateName"].ToString(),
                        Description = dr["Description"] == DBNull.Value ? null : dr["Description"].ToString(),
                        Category = dr["Category"] == DBNull.Value ? null : dr["Category"].ToString(),

                        IsAutoAssignEnabled = dr["IsAutoAssignEnabled"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsAutoAssignEnabled"]),
                        AutoAssignServiceTypes = dr["AutoAssignServiceTypes"] == DBNull.Value ? null : dr["AutoAssignServiceTypes"].ToString(),

                        FormStructure = dr["FormStructure"] == DBNull.Value ? null : dr["FormStructure"].ToString(),
                        RequireSignature = dr["RequireSignature"] == DBNull.Value ? false : Convert.ToBoolean(dr["RequireSignature"]),
                        RequireTip = dr["RequireTip"] == DBNull.Value ? false : Convert.ToBoolean(dr["RequireTip"]),
                        IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"]),

                        CreatedBy = dr["CreatedBy"] == DBNull.Value ? null : dr["CreatedBy"].ToString(),
                        CreatedDateTime = dr["CreatedDateTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["CreatedDateTime"]),
                        UpdatedBy = dr["UpdatedBy"] == DBNull.Value ? null : dr["UpdatedBy"].ToString(),
                        UpdatedDateTime = dr["UpdatedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["UpdatedDateTime"])
                    };

                    response.Add(template);
                }   
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public string SaveFormTemplate(FormTemplate template)
        {
            string response = "";
            try
            {
                if (string.IsNullOrEmpty(template.CompanyID))
                    return "Invalid company.";

                if (string.IsNullOrEmpty(template.TemplateName))
                    return "Template Name is required.";
                if (string.IsNullOrEmpty(template.FormStructure))
                    return "From data is required.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string insertQuery = @"
                INSERT INTO [myServiceJobs].[dbo].[FormTemplates]
                (
                    CompanyID,
                    TemplateName,
                    Description,
                    Category,
                    IsAutoAssignEnabled,
                    AutoAssignServiceTypes,
                    FormStructure,
                    RequireSignature,
                    RequireTip,
                    IsActive,
                    CreatedBy,
                    CreatedDateTime,
                    UpdatedBy,
                    UpdatedDateTime
                )
                VALUES
                (
                    @CompanyID,
                    @TemplateName,
                    @Description,
                    @Category,
                    @IsAutoAssignEnabled,
                    @AutoAssignServiceTypes,
                    @FormStructure,
                    @RequireSignature,
                    @RequireTip,
                    @IsActive,
                    @CreatedBy,
                    @CreatedDateTime,
                    @UpdatedBy,
                    @UpdatedDateTime
                );
                SELECT SCOPE_IDENTITY();";

                        int newId;
                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CompanyID", (object)template.CompanyID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@TemplateName", (object)template.TemplateName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Description", (object)template.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Category", (object)template.Category ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@IsAutoAssignEnabled", template.IsAutoAssignEnabled);
                            cmd.Parameters.AddWithValue("@AutoAssignServiceTypes", (object)template.AutoAssignServiceTypes ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@FormStructure", (object)template.FormStructure ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@RequireSignature", template.RequireSignature);
                            cmd.Parameters.AddWithValue("@RequireTip", template.RequireTip);
                            cmd.Parameters.AddWithValue("@IsActive", template.IsActive);
                            cmd.Parameters.AddWithValue("@CreatedBy", (object)template.CreatedBy ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedDateTime", template.CreatedDateTime == DateTime.MinValue ? DateTime.Now : template.CreatedDateTime);
                            cmd.Parameters.AddWithValue("@UpdatedBy", (object)template.UpdatedBy ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@UpdatedDateTime", (object)template.UpdatedDateTime ?? DBNull.Value);

                            newId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        transaction.Commit();
                        response = $"FormTemplate saved successfully with Id {newId}.";
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
        public string UpdateFormTemplate(FormTemplate template)
        {
            string response = "";
            try
            {
                if (template.Id == 0)
                    return "Select a template.";
               
                if (string.IsNullOrEmpty(template.CompanyID))
                    return "Invalid company.";

                if (template.Id <= 0)
                    return "Invalid template Id.";

                if (string.IsNullOrEmpty(template.TemplateName))
                    return "Template Name is required.";

                if (string.IsNullOrEmpty(template.FormStructure))
                    return "Form data is required.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                                                                        
                        string checkQuery = @"SELECT COUNT(1) 
                                      FROM [myServiceJobs].[dbo].[FormTemplates] 
                                      WHERE Id = @Id AND CompanyID = @CompanyID";

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", template.Id);
                            checkCmd.Parameters.AddWithValue("@CompanyID", (object)template.CompanyID ?? DBNull.Value);

                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (exists == 0)
                            {
                                transaction.Rollback();
                                return "FormTemplate not found.";
                            }
                        }

                       
                        string updateQuery = @"
                UPDATE [myServiceJobs].[dbo].[FormTemplates]
                SET
                    TemplateName = @TemplateName,
                    Description = @Description,
                    Category = @Category,
                    IsAutoAssignEnabled = @IsAutoAssignEnabled,
                    AutoAssignServiceTypes = @AutoAssignServiceTypes,
                    FormStructure = @FormStructure,
                    RequireSignature = @RequireSignature,
                    RequireTip = @RequireTip,
                    IsActive = @IsActive,
                    UpdatedBy = @UpdatedBy,
                    UpdatedDateTime = @UpdatedDateTime
                WHERE Id = @Id AND CompanyID = @CompanyID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", template.Id);
                            cmd.Parameters.AddWithValue("@CompanyID", (object)template.CompanyID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@TemplateName", (object)template.TemplateName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Description", (object)template.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Category", (object)template.Category ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@IsAutoAssignEnabled", template.IsAutoAssignEnabled);
                            cmd.Parameters.AddWithValue("@AutoAssignServiceTypes", (object)template.AutoAssignServiceTypes ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@FormStructure", (object)template.FormStructure ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@RequireSignature", template.RequireSignature);
                            cmd.Parameters.AddWithValue("@RequireTip", template.RequireTip);
                            cmd.Parameters.AddWithValue("@IsActive", template.IsActive);
                            cmd.Parameters.AddWithValue("@UpdatedBy", (object)template.UpdatedBy ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@UpdatedDateTime", template.UpdatedDateTime == DateTime.MinValue ? DateTime.Now : template.UpdatedDateTime);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        response = $"FormTemplate {template.Id} updated successfully.";
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
        public string DeleteFormTemplate(int id, string companyId)
        {
            string response = "";
            try
            {
                if (id <= 0)
                    return "Invalid Id.";

                if (string.IsNullOrEmpty(companyId))
                    return "Invalid company.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        
                        string checkQuery = @"
                    SELECT COUNT(1) 
                    FROM [myServiceJobs].[dbo].[FormTemplates]
                    WHERE Id = @Id AND CompanyID = @CompanyID AND IsActive = 1";

                        int exists = 0;
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Id", id);
                            checkCmd.Parameters.AddWithValue("@CompanyID", companyId);
                            exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        }

                        if (exists == 0)
                        {
                            transaction.Rollback();
                            return "FormTemplate not found.";
                        }

                        
                        string deleteQuery = @"
                    UPDATE [myServiceJobs].[dbo].[FormTemplates]
                    SET 
                        IsActive = 0,
                        UpdatedDateTime = GETDATE()
                    WHERE Id = @Id AND CompanyID = @CompanyID";

                        int rowsAffected;
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.Parameters.AddWithValue("@CompanyID", companyId);
                            rowsAffected = cmd.ExecuteNonQuery();
                        }

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            response = "FormTemplate  deleted successfully.";
                        }
                        else
                        {
                            transaction.Rollback();
                            response = "FormTemplate not found.";
                        }
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