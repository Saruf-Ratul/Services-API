using Services.Entity;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class AppointmentProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<Appointment> GetAllAppointments(string appointmentDate, string companyId, string userId, int appointmentTypeStatus)
        {
            string appointmentFrom = "";
            if (appointmentTypeStatus == 1)
            {
                appointmentFrom = "CEC";
            }
            if (appointmentTypeStatus == 2)
            {
                appointmentFrom = "FSM";
            }
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"
            WITH ResourceCTE AS (
                SELECT id 
                FROM [msSchedulerV3].[dbo].tbl_Resources 
                WHERE UserID = @UserID
            )
            SELECT DISTINCT
                a.CompanyID, a.ApptID, a.AppoinmentUId, a.CustomerID, a.ServiceType, a.ResourceID, 
                a.TimeSlotId, a.ApptDateTime, a.StartDateTime, a.EndDateTime, a.TimeSlot, a.Note, 
                a.Status, a.TicketStatus, a.CreatedDateTime AS AppointmentCreatedDateTime, 
                a.MarkDownloaded, a.PromoCode, a.CreatedBy, a.UserID, 
                c.CreatedCompanyID, c.TagID, c.AMCustomerID, c.CustomerGuid, c.Title, c.Title2, 
                c.FirstName, c.FirstName2, c.LastName, c.LastName2, c.JobTitle, c.JobTitle2, 
                c.Address1, c.Address2, c.City, c.State, c.ZipCode, c.Phone, c.Mobile, c.Email, 
                c.Notes AS CustomerNotes, c.CreatedDateTime AS CustomerCreatedDateTime, 
                c.CallPopUploaded, c.CallPopAppId, c.IsPrimaryContact, c.BusinessID, 
                c.SyncToken, c.BusinessName, c.IsBusinessContact, c.CompanyName, c.CompanyName2,
                r.Name AS ResourceName, s.StatusName AS StatusName, st.ServiceName ,  s.StatusID , 
				st.ServiceTypeID , r.Id AS ResourceId,ts.StatusID as TicketStatusId, ts.StatusName as TicketStatusName
            FROM [msSchedulerV3].[dbo].tbl_Appointment AS a   
            INNER JOIN [msSchedulerV3].[dbo].tbl_Customer AS c 
                ON a.CustomerID = c.CustomerID AND a.CompanyID = c.CompanyID 
            INNER JOIN [msSchedulerV3].[dbo].tbl_Resources AS r 
                ON a.ResourceID = r.Id AND a.CompanyID = r.CompanyID  
            INNER JOIN [msSchedulerV3].[dbo].tbl_Status AS s 
                ON a.Status = s.StatusID AND a.CompanyID = s.CompanyID  
            INNER JOIN [msSchedulerV3].[dbo].tbl_ServiceType AS st 
                ON a.ServiceType = st.ServiceTypeID AND a.CompanyID = st.CompanyID
		LEFT JOIN [msSchedulerV3].[dbo].tbl_TicketStatus AS ts 
    ON a.TicketStatus = ts.StatusID and a.CompanyID = ts.CompanyID
            WHERE a.CompanyID = @CompanyID 
                AND a.Status != 'Deleted'  
                AND s.StatusName NOT IN ('Cancelled', 'Closed','Pending')  
                AND a.ResourceID IS NOT NULL 
                AND a.ResourceID IN (SELECT id FROM ResourceCTE)
                  AND CONVERT(VARCHAR, a.CreatedDateTime, 111) <= @AppointmentDate ";
                    if (!string.IsNullOrEmpty(appointmentFrom))
                    {
                        query += @" AND a.SchedulingCal = @AppointmentFrom ;";
                    }


                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@CompanyID", companyId);
                        command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
                        if (!string.IsNullOrEmpty(appointmentFrom))
                        {
                            command.Parameters.AddWithValue("@AppointmentFrom", appointmentFrom);
                        }
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                appointments.Add(new Appointment
                                {
                                    CompanyID = reader["CompanyID"].ToString(),
                                    ApptID = Convert.ToInt32(reader["ApptID"]),
                                    AppoinmentUId = reader["AppoinmentUId"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ServiceTypeId = reader["ServiceType"].ToString(),
                                    ResourceID = reader["ResourceID"] != DBNull.Value ? Convert.ToInt32(reader["ResourceID"]) : (int?)null,
                                    TimeSlotId = reader["TimeSlotId"] != DBNull.Value ? Convert.ToInt32(reader["TimeSlotId"]) : (int?)null,
                                    ApptDateTime = reader["ApptDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["ApptDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    StartDateTime = reader["StartDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["StartDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    EndDateTime = reader["EndDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["EndDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    TimeSlot = reader["TimeSlot"].ToString(),
                                    Note = reader["Note"].ToString(),
                                    StatusId = reader["Status"].ToString(),
                                    TicketStatusId = reader["TicketStatus"].ToString(),
                                    CreatedDateTime = reader["AppointmentCreatedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["AppointmentCreatedDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    MarkDownloaded = reader["MarkDownloaded"] != DBNull.Value && Convert.ToBoolean(reader["MarkDownloaded"]),
                                    PromoCode = reader["PromoCode"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    UserID = reader["UserID"].ToString(),

                                    Customer = new Customer
                                    {
                                        CompanyID = reader["CompanyID"].ToString(),
                                        CreatedCompanyID = reader["CreatedCompanyID"].ToString(),
                                        TagID = Convert.ToInt32(reader["TagID"]),
                                        CustomerID = reader["CustomerID"].ToString(),
                                        AMCustomerID = reader["AMCustomerID"].ToString(),
                                        CustomerGuid = reader["CustomerGuid"].ToString(),
                                        Title = reader["Title"].ToString(),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"]?.ToString(),
                                        JobTitle = reader["JobTitle"].ToString(),
                                        Address1 = reader["Address1"].ToString(),
                                        Address2 = reader["Address2"]?.ToString(),
                                        City = reader["City"]?.ToString(),
                                        State = reader["State"]?.ToString(),
                                        ZipCode = reader["ZipCode"]?.ToString(),
                                        Phone = reader["Phone"].ToString(),
                                        Mobile = reader["Mobile"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        Notes = reader["CustomerNotes"]?.ToString(),
                                        CreatedDateTime = reader["CustomerCreatedDateTime"]?.ToString(),
                                        CallPopUploaded = reader["CallPopUploaded"] != DBNull.Value && Convert.ToBoolean(reader["CallPopUploaded"]),
                                        BusinessName = reader["BusinessName"].ToString(),
                                        IsBusinessContact = reader["IsBusinessContact"] != DBNull.Value && Convert.ToBoolean(reader["IsBusinessContact"]),
                                        CompanyName = reader["CompanyName"].ToString(),
                                    },

                                    Resource = new Services.Entity.Resource
                                    {
                                        Id = Convert.ToInt32(reader["ResourceId"]),
                                        Name = reader["ResourceName"].ToString()
                                    },

                                    Status = new Status
                                    {
                                        StatusId = Convert.ToInt32(reader["StatusID"]),
                                        StatusName = reader["StatusName"].ToString()
                                    },
                                    TicketStatus = new TicketStatus
                                    {
                                        StatusId = reader["TicketStatusId"] != DBNull.Value ? Convert.ToInt32(reader["TicketStatusId"]) : 0,
                                        StatusName = reader["TicketStatusName"] != DBNull.Value ? reader["TicketStatusName"].ToString() : string.Empty,
                                        CompanyId = reader["CompanyID"].ToString()
                                    },
                                    ServiceType = new ServiceType
                                    {
                                        ServiceTypeID = Convert.ToInt32(reader["ServiceTypeID"]),
                                        ServiceName = reader["ServiceName"].ToString()
                                    },
                                    Invoices = GetInvoicesByAppointment(Convert.ToInt32(reader["ApptID"]), Convert.ToInt32(reader["CustomerID"]), reader["CompanyID"].ToString()),

                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching appointments: " + ex.Message);
            }
            return appointments;
        }
        public List<AppointmentInvoice> GetInvoicesByAppointment(int appointmentrId, int customerId, string companyId)
        {
            try
            {
                string sql = "";
                Database db = new Database();
                DataTable dt = new DataTable();
                List<AppointmentInvoice> invoices = new List<AppointmentInvoice>();
                sql = @"SELECT *,c.CustomerGuid,c.FirstName+ c.LastName as FullName,c.CustomerID,c.City
            FROM [msSchedulerV3].[dbo].tbl_invoice as i
			join [msSchedulerV3].[dbo].tbl_Customer as c on c.customerID = i.customerId and c.CompanyID =  @CompanyID
            WHERE AppointmentId != 0 AND AppointmentId = " + appointmentrId + " AND CompnyID = @CompanyID AND i.CustomerId = " + customerId + "";

                DataSet dataSet = db.Get_DataSet(sql, companyId);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    return invoices;
                }
                dt = dataSet.Tables[0];


                foreach (DataRow dr in dt.Rows)
                {

                    // FOR ITEMS
                    sql = @"select ItemId,ItemName,Description,Quantity,TotalPrice,uPrice,ServiceDate,ItemTyId,IsTaxable   from [msSchedulerV3].[dbo].tbl_InvoiceDetails where RefId = '" + dr["ID"].ToString() + "' and companyid =@CompanyID order by CAST(NULLIF(LineNum,'') AS INT) asc ;";

                    DataSet dataSet_Items = db.Get_DataSet(sql, companyId);


                    List<InvoiceItem> items = new List<InvoiceItem>();
                    DataTable dt_InvoiceItems = dataSet_Items.Tables[0];

                    foreach (DataRow dr_Items in dt_InvoiceItems.Rows)
                    {
                        InvoiceItem item = new InvoiceItem
                        {
                            ItemId = dr_Items["ItemId"].ToString(),
                            Name = dr_Items["ItemName"].ToString(),
                            Description = dr_Items["Description"].ToString(),
                            Quantity = dr_Items["Quantity"].ToString(),
                            UnitPrice = dr_Items["uPrice"].ToString(),
                            IsTaxable = dr_Items["IsTaxable"].ToString(),
                            ItemTyId = dr_Items["ItemTyId"].ToString(),

                            TotalPrice = dr_Items["TotalPrice"].ToString()

                        };
                        items.Add(item);
                    }



                    AppointmentInvoice invoice = new AppointmentInvoice
                    {

                        InvoiceID = dr["ID"].ToString(),
                        DepositAmount = dr["DepositAmount"] != DBNull.Value ? Convert.ToDecimal(dr["DepositAmount"]) : 0,

                        Number = dr["Number"] != DBNull.Value ? dr["Number"].ToString() : string.Empty,

                        InvoiceDate = dr["InvoiceDate"] != DBNull.Value
                            ? Convert.ToDateTime(dr["InvoiceDate"]).ToString("yyyy/MM/dd hh:mm tt")
                            : string.Empty,

                        Subtotal = dr["Subtotal"] != DBNull.Value ? Convert.ToDecimal(dr["Subtotal"]) : 0,
                        AmountCollect = dr["AmountCollect"] != DBNull.Value ? Convert.ToDecimal(dr["AmountCollect"]) : 0,
                        Discount = dr["Discount"] != DBNull.Value ? Convert.ToDecimal(dr["Discount"]) : 0,
                        Total = dr["Total"] != DBNull.Value ? Convert.ToDecimal(dr["Total"]) : 0,
                        Tax = dr["Tax"] != DBNull.Value ? Convert.ToDecimal(dr["Tax"]) : 0,

                        Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : string.Empty,
                        Type = dr["Type"] != DBNull.Value ? dr["Type"].ToString() : string.Empty,
                        Note = dr["Note"] != DBNull.Value ? dr["Note"].ToString() : string.Empty,
                        CustomerGuid = dr["CustomerGuid"].ToString(),
                        FullName = dr["FullName"].ToString(),
                        QBOCustomerId = "0",
                        CustomerId = dr["CustomerID"].ToString(),
                        QBOId = "0",
                        City = dr["City"].ToString(),
                        Due = "0",
                        IsConverted = Convert.ToBoolean(dr["isConverted"]),
                        ConvertedInvoiceID = "",
                        Surcharge = 0,
                        DiscountOption = dr["DiscountOption"] != DBNull.Value ? dr["DiscountOption"].ToString() : string.Empty,
                        TaxType = dr["TaxType"] != DBNull.Value ? dr["TaxType"].ToString() : string.Empty,
                        RequestedDepositAmount = dr["RequestedDepoAmt"] != DBNull.Value ? dr["RequestedDepoAmt"].ToString() : "0.00",
                        RequestedDepositPercentage = dr["ReqDepoPercent"] != DBNull.Value ? dr["ReqDepoPercent"].ToString() : "0.00",
                        RequestedAmountType = dr["RequestedAmtType"] != DBNull.Value ? Convert.ToInt32( dr["RequestedAmtType"].ToString()): 0,
                        items = items,
                        PaymentList = GetPaymentByInvoiceId(dr["ID"].ToString(), companyId)
                    };

                    invoices.Add(invoice);
                }

                return invoices;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching invoices", ex);
            }


        }
        public string UpdateAppointment(AppointmentDTO appointment)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"UPDATE [msSchedulerV3].[dbo].[tbl_Appointment]
                            SET [CompanyID] = @CompanyID,
                                [ApptID] = @ApptID,
                                [AppoinmentUId] = @AppoinmentUId,
                                [CustomerID] = @CustomerID,
                                [ServiceType] = @ServiceType,
                                [ResourceID] = @ResourceID,
                                [TimeSlotId] = @TimeSlotId,
                                [ApptDateTime] = @ApptDateTime,
                                [StartDateTime] = @StartDateTime,
                                [EndDateTime] = @EndDateTime,
                                [TimeSlot] = @TimeSlot,
                                [Note] = @Note,
                                [Status] = @Status,
                                [TicketStatus] = @TicketStatus,
                                [PromoCode] = @PromoCode,
                                [UserID] = @UserID
                            WHERE ApptID = @ApptID And CompanyID = @CompanyID And [CustomerID] = @CustomerID ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyID", appointment.CompanyID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ApptID", appointment.ApptID);
                        command.Parameters.AddWithValue("@AppoinmentUId", appointment.AppoinmentUId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CustomerID", appointment.CustomerID);
                        command.Parameters.AddWithValue("@ServiceType", appointment.ServiceTypeId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ResourceID", appointment.ResourceID);
                        command.Parameters.AddWithValue("@TimeSlotId", appointment.TimeSlotId.HasValue ? (object)appointment.TimeSlotId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@ApptDateTime", appointment.ApptDateTime);
                        command.Parameters.AddWithValue("@StartDateTime", appointment.StartDateTime);
                        command.Parameters.AddWithValue("@EndDateTime", appointment.EndDateTime);
                        command.Parameters.AddWithValue("@TimeSlot", appointment.TimeSlot ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Note", appointment.Note ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Status", appointment.StatusId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TicketStatus", appointment.TicketStatusId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PromoCode", appointment.PromoCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserID", appointment.UserID ?? (object)DBNull.Value);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result != 0)
                        {
                            response = "Appointment updated successfully.";
                        }
                        else
                        {
                            response = "Error updating appointment.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }
        public List<Payment> GetPaymentByInvoiceId(string invoiceId, string companyId)
        {
            var list = new List<Payment>();
            try
            {
                string sql = @"SELECT * 
                       FROM [msSchedulerV3].[dbo].[tbl_Payment] 
                       WHERE companyid = @CompanyID AND InvocieId = '" + invoiceId + "'";

                Database db = new Database();

                DataSet dataSet = db.Get_DataSet(sql, companyId);
                    if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                        return list;

                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    Payment payment = new Payment
                    {
                        Id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0,
                        CompanyId = dr["Companyid"]?.ToString(),
                        InvocieId = dr["InvocieId"]?.ToString(),
                        Amount = dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0,
                        CheckName = dr["CheckName"]?.ToString(),
                        CheckNumber = dr["CheckNumber"]?.ToString(),
                        Type = dr["Type"]?.ToString(),
                        IsDeposit = dr["IsDeposit"] != DBNull.Value && Convert.ToBoolean(dr["IsDeposit"]),
                        Source = dr["Source"]?.ToString(),
                        CreatedDate = dr["createdDate"] != DBNull.Value? Convert.ToDateTime(dr["createdDate"]).ToString("MM/dd/yyyy"): string.Empty,
                        QboId = dr["QboId"] != DBNull.Value ? Convert.ToInt32(dr["QboId"]) : (int?)null,
                        PaymentRefNum = dr["PaymentRefNum"]?.ToString(),
                        RMPaymentId = dr["RMPaymentId"] != DBNull.Value ? Convert.ToInt32(dr["RMPaymentId"]) : 0
                    };

                    list.Add(payment);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching payment records", ex);
            }
        }
        public string SaveImageForCSL(CSLImageDTO entity)
        {
            string response = "";
            try
            {
                if (entity.CustomerId == 0)
                    return "Please select a customer";

                if (entity.AppointmentId == 0)
                    return "Please select an appointment";

                if (string.IsNullOrEmpty(entity.CompanyId))
                    return "Invalid company.";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        
                    string parentQuery = @"
                    INSERT INTO [msSchedulerV3].[dbo].[tbl_AppointmentCSlImageMapping]
                    (AppointmentId, CustomerId, CSLId, CompanyId, TagName)
                    VALUES (@AppointmentId, @CustomerId, @CSLId, @CompanyId, @TagName);
                    SELECT SCOPE_IDENTITY();";

                        int parentId;
                        using (SqlCommand cmd = new SqlCommand(parentQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AppointmentId", entity.AppointmentId);
                            cmd.Parameters.AddWithValue("@CustomerId", entity.CustomerId);
                            cmd.Parameters.AddWithValue("@CSLId", (object)entity.CSLId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CompanyId", (object)entity.CompanyId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@TagName", (object)entity.TagName ?? DBNull.Value);

                            parentId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        if (entity.ImageList != null && entity.ImageList.Count > 0)
                        {
                            foreach (var img in entity.ImageList)
                            {
                                if (string.IsNullOrEmpty(img.ImageName) || string.IsNullOrEmpty(img.ImageBase64))
                                    continue;
                                DateTime dateTime = Convert.ToDateTime(img.CreatedAt);
                                string childQuery = @"
                            INSERT INTO [msSchedulerV3].[dbo].[tbl_AppointmentCSLImages]
                            (AppointmentCSLId, ImageName, ImageBase64, CompanyId,Description,CreatedAt)
                            VALUES (@AppointmentCSLId, @ImageName, @ImageBase64,@CompanyId,@Description,@CreatedAt);";

                                using (SqlCommand cmd = new SqlCommand(childQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@AppointmentCSLId", parentId);
                                    cmd.Parameters.AddWithValue("@ImageName", (object)img.ImageName ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@ImageBase64", (object)img.ImageBase64 ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Description", (object)img.Description ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@CreatedAt", (object)dateTime ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@CompanyId", (object)entity.CompanyId ?? DBNull.Value);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
                        response = $"Image saved successfully.";
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
        //public List<CSLImageDTO> GetCSLImages(int appointmentId, int customerId,int cSLId, string companyId)
        //{
        //    List<CSLImageDTO> result = new List<CSLImageDTO>();

        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        string query = @"
        //    SELECT m.Id AS ParentId, 
        //           m.AppointmentId, 
        //           m.CustomerId, 
        //           m.CSLId, 
        //           m.CompanyId, 
        //           m.TagName,
        //           i.Id AS ImageId,
        //           i.ImageName,
        //           i.ImageBase64
        //    FROM [msSchedulerV3].[dbo].tbl_AppointmentCSlImageMapping m
        //    LEFT JOIN [msSchedulerV3].[dbo].tbl_AppointmentCSLImages i 
        //        ON m.Id = i.AppointmentCSLId
        //        WHERE m.AppointmentId = @AppointmentId
        //      AND m.CustomerId = @CustomerId
        //      AND m.CompanyId = @CompanyId";
        //        if(cSLId !=  0)
        //        {
        //            query += "  And m.CSLId = @CSLId ;";
        //        }

        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
        //            cmd.Parameters.AddWithValue("@CustomerId", customerId);
        //            cmd.Parameters.AddWithValue("@CompanyId", companyId);
        //            if (cSLId != 0)
        //            {
                       
        //                cmd.Parameters.AddWithValue("@CSLId", cSLId);
        //            }
        //            conn.Open();
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                Dictionary<int, CSLImageDTO> map = new Dictionary<int, CSLImageDTO>();

        //                while (reader.Read())
        //                {
        //                    int parentId = Convert.ToInt32(reader["ParentId"]);

        //                    if (!map.ContainsKey(parentId))
        //                    {
        //                        map[parentId] = new CSLImageDTO
        //                        {
        //                            AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
        //                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
        //                            CSLId = reader["CSLId"] != DBNull.Value ? Convert.ToInt32(reader["CSLId"]) : 0,
        //                            CompanyId = reader["CompanyId"].ToString(),
        //                            TagName = reader["TagName"] != DBNull.Value ? reader["TagName"].ToString() : null,
        //                            ImageList = new List<Image>()
        //                        };
        //                    }

        //                    if (reader["ImageId"] != DBNull.Value)
        //                    {
        //                        map[parentId].ImageList.Add(new Image
        //                        {
        //                            ImageName = reader["ImageName"] != DBNull.Value ? reader["ImageName"].ToString() : null,
        //                            ImageBase64 = reader["ImageBase64"] != DBNull.Value ? reader["ImageBase64"].ToString() : null
        //                        });
        //                    }
        //                }

        //                result = map.Values.ToList();
        //            }
        //        }
        //    }

        //    return result;
        //}
        public List<CSLImageDTO> GetCSLImages(int appointmentId, int customerId, int cSLId, string companyId)
        {
            List<CSLImageDTO> result = new List<CSLImageDTO>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
            SELECT m.Id AS ParentId, 
                   m.AppointmentId, 
                   m.CustomerId, 
                   m.CSLId, 
                   m.CompanyId, 
                   m.TagName,
                   i.Id AS ImageId,
                   i.ImageName,
                   i.ImageBase64,
                   i.Description,
                   i.CreatedAt
            FROM [msSchedulerV3].[dbo].tbl_AppointmentCSlImageMapping m
            LEFT JOIN [msSchedulerV3].[dbo].tbl_AppointmentCSLImages i 
                ON m.Id = i.AppointmentCSLId
                WHERE m.AppointmentId = @AppointmentId
              AND m.CustomerId = @CustomerId
              AND m.CompanyId = @CompanyId";
                if (cSLId != 0)
                {
                    query += "  And m.CSLId = @CSLId ;";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    if (cSLId != 0)
                    {

                        cmd.Parameters.AddWithValue("@CSLId", cSLId);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, CSLImageDTO> map = new Dictionary<int, CSLImageDTO>();

                        while (reader.Read())
                        {
                            int parentId = Convert.ToInt32(reader["ParentId"]);

                            if (!map.ContainsKey(parentId))
                            {
                                map[parentId] = new CSLImageDTO
                                {
                                    AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                    CSLId = reader["CSLId"] != DBNull.Value ? Convert.ToInt32(reader["CSLId"]) : 0,
                                    CompanyId = reader["CompanyId"].ToString(),
                                    TagName = reader["TagName"] != DBNull.Value ? reader["TagName"].ToString() : null,
                                    ImageList = new List<Image>()
                                };
                            }

                            if (reader["ImageId"] != DBNull.Value)
                            {
                                map[parentId].ImageList.Add(new Image
                                {
                                    ImageName = reader["ImageName"] != DBNull.Value ? reader["ImageName"].ToString() : null,
                                    ImageBase64 = reader["ImageBase64"] != DBNull.Value ? reader["ImageBase64"].ToString() : null,
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? reader["CreatedAt"].ToString() : null
                                });
                            }
                        }

                        result = map.Values.ToList();
                    }
                }
            }

            return result;
        }
        public string AssignForm(AppointmnetForm entity)
        {
            string response = "";
            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Step 1: Delete existing FormInstances
                        string deleteQuery = @"
                    DELETE FROM [myServiceJobs].[dbo].[FormInstances] 
                    WHERE AppointmentId = @AppointmentId
                      AND CustomerId = @CustomerId
                      AND CompanyId = @CompanyId;";

                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AppointmentId", entity.AppointmentId);
                            cmd.Parameters.AddWithValue("@CustomerId", entity.CustomerId);
                            cmd.Parameters.AddWithValue("@CompanyId", entity.CompanyId);
                            cmd.ExecuteNonQuery();
                        }

                        // Step 2: Insert new FormInstances if FormIds exist
                        if (entity.FormIds != null && entity.FormIds.Count > 0)
                        {
                            foreach (var formId in entity.FormIds)
                            {
                                string insertQuery = @"
                            INSERT INTO [myServiceJobs].[dbo].[FormInstances]
                            (CompanyID, TemplateId, AppointmentId, CustomerId, Status, StartedDateTime)
                            VALUES (@CompanyID, @TemplateId, @AppointmentId, @CustomerId, 'Pending', GETDATE());";

                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@CompanyID", entity.CompanyId);
                                    cmd.Parameters.AddWithValue("@TemplateId", formId);
                                    cmd.Parameters.AddWithValue("@AppointmentId", entity.AppointmentId);
                                    cmd.Parameters.AddWithValue("@CustomerId", entity.CustomerId);
                                

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        response = "Forms assigned successfully.";
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
        public List<CSLTag> GetAllTags(string companyId)
        {
            var list = new List<CSLTag>();
            try
            {
                string sql = @"SELECT Id, Name, Description, CreatedAt,CompanyId
                           FROM [msSchedulerV3].[dbo].[Tbl_CSLTag] where companyId = '" + companyId + "'";

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CSLTag tag = new CSLTag
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                Name = reader["Name"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]).ToString("yyy/mm/dd") : DateTime.MinValue.ToString("yyy/mm/dd"),
                                CompanyId = reader["CompanyId"]?.ToString()
                            };
                            list.Add(tag);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching CSL tags", ex);
            }
            return list;
        }
        public CSLTag GetTagById(int id, string companyId)
        {
            CSLTag tag = null;
            try
            {
                string sql = @"SELECT Id, Name, Description, CreatedAt, CompanyId
                           FROM [msSchedulerV3].[dbo].[Tbl_CSLTag] 
                           WHERE Id = @Id and CompanyId = @companyId";

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@companyId", companyId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tag = new CSLTag
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]).ToString("yyy/mm/dd"),
                                CompanyId = reader["CompanyId"].ToString(),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching CSL tag by Id", ex);
            }
            return tag;
        }
        public string UpdateTag(CSLTag tag)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"UPDATE [msSchedulerV3].[dbo].[Tbl_CSLTag]
                                 SET [Name] = @Name,
                                     [Description] = @Description
                                 WHERE Id = @Id and CompanyId = @companyId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", tag.Name ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", tag.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Id", tag.Id);
                        command.Parameters.AddWithValue("@companyId", tag.CompanyId);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        response = result > 0 ? "Tag updated successfully." : "No tag found to update.";
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }
        public string DeleteTag(int id, string companyId)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"DELETE FROM [msSchedulerV3].[dbo].[Tbl_CSLTag] 
                                 WHERE Id = @Id and CompanyId = @companyId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@companyId", companyId);
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        response = result > 0 ? "Tag deleted successfully." : "No tag found to delete.";
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }
        public string SaveTag(CSLTag tag)
        {

            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"INSERT INTO [msSchedulerV3].[dbo].[Tbl_CSLTag]
                                ([Name], [Description], [CreatedAt], [CompanyId])
                             VALUES (@Name, @Description, @CreatedAt,@CompanyId)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", tag.Name ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", tag.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Parse(tag.CreatedAt));
                        command.Parameters.AddWithValue("@CompanyId", tag.CompanyId);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        response = result > 0 ? "Tag saved successfully." : "Error saving tag.";
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }
        public List<AppoinmentListWithForms> GetAllAppointmentsWithForms(string appointmentDate, string companyId, string userId, int appointmentTypeStatus)
        {
            string appointmentFrom = "";
            if (appointmentTypeStatus == 1)
            {
                appointmentFrom = "CEC";
            }
            if (appointmentTypeStatus == 2)
            {
                appointmentFrom = "FSM";
            }
            List<AppoinmentListWithForms> appointments = new List<AppoinmentListWithForms>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"
            WITH ResourceCTE AS (
                SELECT id 
                FROM [msSchedulerV3].[dbo].tbl_Resources 
                WHERE UserID = @UserID
            )
            SELECT DISTINCT
                a.CompanyID, a.ApptID, a.AppoinmentUId, a.CustomerID, a.ServiceType, a.ResourceID, 
                a.TimeSlotId, a.ApptDateTime, a.StartDateTime, a.EndDateTime, a.TimeSlot, a.Note, 
                a.Status, a.TicketStatus, a.CreatedDateTime AS AppointmentCreatedDateTime, 
                a.MarkDownloaded, a.PromoCode, a.CreatedBy, a.UserID, 
                c.CreatedCompanyID, c.TagID, c.AMCustomerID, c.CustomerGuid, c.Title, c.Title2, 
                c.FirstName, c.FirstName2, c.LastName, c.LastName2, c.JobTitle, c.JobTitle2, 
                c.Address1, c.Address2, c.City, c.State, c.ZipCode, c.Phone, c.Mobile, c.Email, 
                c.Notes AS CustomerNotes, c.CreatedDateTime AS CustomerCreatedDateTime, 
                c.CallPopUploaded, c.CallPopAppId, c.IsPrimaryContact, c.BusinessID, 
                c.SyncToken, c.BusinessName, c.IsBusinessContact, c.CompanyName, c.CompanyName2,
                r.Name AS ResourceName, s.StatusName AS StatusName, st.ServiceName ,  s.StatusID , 
				st.ServiceTypeID , r.Id AS ResourceId,ts.StatusID as TicketStatusId, ts.StatusName as TicketStatusName
            FROM [msSchedulerV3].[dbo].tbl_Appointment AS a   
            INNER JOIN [msSchedulerV3].[dbo].tbl_Customer AS c 
                ON a.CustomerID = c.CustomerID AND a.CompanyID = c.CompanyID 
            INNER JOIN [msSchedulerV3].[dbo].tbl_Resources AS r 
                ON a.ResourceID = r.Id AND a.CompanyID = r.CompanyID  
            INNER JOIN [msSchedulerV3].[dbo].tbl_Status AS s 
                ON a.Status = s.StatusID AND a.CompanyID = s.CompanyID  
            INNER JOIN [msSchedulerV3].[dbo].tbl_ServiceType AS st 
                ON a.ServiceType = st.ServiceTypeID AND a.CompanyID = st.CompanyID
		LEFT JOIN [msSchedulerV3].[dbo].tbl_TicketStatus AS ts 
    ON a.TicketStatus = ts.StatusID and a.CompanyID = ts.CompanyID
            WHERE a.CompanyID = @CompanyID 
                AND a.Status != 'Deleted'  
                AND s.StatusName NOT IN ('Cancelled', 'Closed','Pending')  
                AND a.ResourceID IS NOT NULL 
                AND a.ResourceID IN (SELECT id FROM ResourceCTE)
                AND CONVERT(VARCHAR, a.CreatedDateTime, 111) <= @AppointmentDate ";
                    if (!string.IsNullOrEmpty(appointmentFrom))
                    {
                        query += @" AND a.SchedulingCal = @AppointmentFrom ;";
                    }


                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@CompanyID", companyId);
                        command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
                        if (!string.IsNullOrEmpty(appointmentFrom))
                        {
                            command.Parameters.AddWithValue("@AppointmentFrom", appointmentFrom);
                        }
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                appointments.Add(new AppoinmentListWithForms
                                {
                                    CompanyID = reader["CompanyID"].ToString(),
                                    ApptID = Convert.ToInt32(reader["ApptID"]),
                                    AppoinmentUId = reader["AppoinmentUId"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ServiceTypeId = reader["ServiceType"].ToString(),
                                    ResourceID = reader["ResourceID"] != DBNull.Value ? Convert.ToInt32(reader["ResourceID"]) : (int?)null,
                                    TimeSlotId = reader["TimeSlotId"] != DBNull.Value ? Convert.ToInt32(reader["TimeSlotId"]) : (int?)null,
                                    ApptDateTime = reader["ApptDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["ApptDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    StartDateTime = reader["StartDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["StartDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    EndDateTime = reader["EndDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["EndDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    TimeSlot = reader["TimeSlot"].ToString(),
                                    Note = reader["Note"].ToString(),
                                    StatusId = reader["Status"].ToString(),
                                    TicketStatusId = reader["TicketStatus"].ToString(),
                                    CreatedDateTime = reader["AppointmentCreatedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["AppointmentCreatedDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    MarkDownloaded = reader["MarkDownloaded"] != DBNull.Value && Convert.ToBoolean(reader["MarkDownloaded"]),
                                    PromoCode = reader["PromoCode"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    UserID = reader["UserID"].ToString(),

                                    Customer = new Customer
                                    {
                                        CompanyID = reader["CompanyID"].ToString(),
                                        CreatedCompanyID = reader["CreatedCompanyID"].ToString(),
                                        TagID = Convert.ToInt32(reader["TagID"]),
                                        CustomerID = reader["CustomerID"].ToString(),
                                        AMCustomerID = reader["AMCustomerID"].ToString(),
                                        CustomerGuid = reader["CustomerGuid"].ToString(),
                                        Title = reader["Title"].ToString(),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"]?.ToString(),
                                        JobTitle = reader["JobTitle"].ToString(),
                                        Address1 = reader["Address1"].ToString(),
                                        Address2 = reader["Address2"]?.ToString(),
                                        City = reader["City"]?.ToString(),
                                        State = reader["State"]?.ToString(),
                                        ZipCode = reader["ZipCode"]?.ToString(),
                                        Phone = reader["Phone"].ToString(),
                                        Mobile = reader["Mobile"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        Notes = reader["CustomerNotes"]?.ToString(),
                                        CreatedDateTime = reader["CustomerCreatedDateTime"]?.ToString(),
                                        CallPopUploaded = reader["CallPopUploaded"] != DBNull.Value && Convert.ToBoolean(reader["CallPopUploaded"]),
                                        BusinessName = reader["BusinessName"].ToString(),
                                        IsBusinessContact = reader["IsBusinessContact"] != DBNull.Value && Convert.ToBoolean(reader["IsBusinessContact"]),
                                        CompanyName = reader["CompanyName"].ToString(),
                                    },

                                    Resource = new Services.Entity.Resource
                                    {
                                        Id = Convert.ToInt32(reader["ResourceId"]),
                                        Name = reader["ResourceName"].ToString()
                                    },

                                    Status = new Status
                                    {
                                        StatusId = Convert.ToInt32(reader["StatusID"]),
                                        StatusName = reader["StatusName"].ToString()
                                    },
                                    TicketStatus = new TicketStatus
                                    {
                                        StatusId = reader["TicketStatusId"] != DBNull.Value ? Convert.ToInt32(reader["TicketStatusId"]) : 0,
                                        StatusName = reader["TicketStatusName"] != DBNull.Value ? reader["TicketStatusName"].ToString() : string.Empty,
                                        CompanyId = reader["CompanyID"].ToString()
                                    },
                                    ServiceType = new ServiceType
                                    {
                                        ServiceTypeID = Convert.ToInt32(reader["ServiceTypeID"]),
                                        ServiceName = reader["ServiceName"].ToString()
                                    },
                                    Invoices = GetInvoicesByAppointment(Convert.ToInt32(reader["ApptID"]), Convert.ToInt32(reader["CustomerID"]), reader["CompanyID"].ToString()),
                                    FormIds = GetAssignedForms(Convert.ToInt32(reader["ApptID"]), Convert.ToInt32(reader["CustomerID"]), reader["CompanyID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching appointments: " + ex.Message);
            }
            return appointments;
        }
        public List<int> GetAssignedForms(int appointmentId, int customerId, string companyId)
        {
            string sql = "";
            Database db = new Database(connStr);
            DataTable dt = new DataTable();
            List<int> forms = new List<int>();

            sql = @"SELECT TemplateId
            FROM [myServiceJobs].[dbo].[FormInstances] 
            WHERE AppointmentId = " + appointmentId + @" 
              AND CustomerId = " + customerId + @" 
              AND CompanyId = @CompanyID;";

            DataSet dataSet = db.Get_DataSet(sql, companyId);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return forms;
            }

            dt = dataSet.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                forms.Add(Convert.ToInt32(dr["TemplateId"]));
            }

            return forms;
        }
        public class AppointmentInvoice
        {
            public string InvoiceID { get; set; }
            public string CustomerGuid { get; set; }
            public string FullName { get; set; }
            public string QBOCustomerId { get; set; }
            public string CustomerId { get; set; }
            public decimal DepositAmount { get; set; }
            public string City { get; set; }
            public string QBOId { get; set; }

            public string Number { get; set; }
            public string InvoiceDate { get; set; }
            public decimal Subtotal { get; set; }
            public decimal AmountCollect { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
            public decimal Tax { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
            public string Note { get; set; }
            public string Due { get; set; }
            public bool IsConverted { get; set; }
            public string ConvertedInvoiceID { get; set; }
            public decimal Surcharge { get; set; }
            public string DiscountOption { get; set; }
            public string TaxType { get; set; }
            public string RequestedDepositAmount { get; set; }
            public string RequestedDepositPercentage { get; set; }
            public int RequestedAmountType { get; set; }
            public List<InvoiceItem> items { get; set; }
            public List<Payment> PaymentList { get; set; }

        }
        public class Payment
        {
            public int Id { get; set; }
            public string CompanyId { get; set; }
            public string InvocieId { get; set; }
            public decimal Amount { get; set; }
            public string CheckName { get; set; }
            public string CheckNumber { get; set; }
            public string Type { get; set; }
            public bool IsDeposit { get; set; }
            public string Source { get; set; }
            public string CreatedDate { get; set; }
            public int? QboId { get; set; }
            public string PaymentRefNum { get; set; }
            public int RMPaymentId { get; set; }
        }
        public class AppointmnetForm
        {
            public int AppointmentId { get; set; }
            public int CustomerId { get; set; }
            public string CompanyId { get; set; }
            public List<int> FormIds { get; set; }
            public string UserId { get; set; }
        }
        public class CSLTag
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string CompanyId { get; set; }
            public string CreatedAt { get; set; }
        }
        public class AppoinmentListWithForms : Appointment
        {
            public List<int> FormIds { get; set; }
        }
    }
}