using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Services.Processor
{
    public class TwilioProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public string SendSMS(string recipientNumber, string message, string Companyid)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            string accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            string authToken = ConfigurationManager.AppSettings["TwilioAccountAuthToken"];
            string twilioPhoneNumber = ConfigurationManager.AppSettings["TwilioPhoneNumber"];

            TwilioSetting twilioSetting = GettwilioSetting(Companyid);


            TwilioClient.Init(twilioSetting.TwilioAccountSid, twilioSetting.TwilioAccountAuthToken);

            var to = new PhoneNumber(recipientNumber);
            var msg = MessageResource.Create(
                to,
                from: new PhoneNumber(twilioSetting.TwilioPhoneNumber),
                body: message);
            return msg.Sid;
        }
        public TwilioSetting GettwilioSetting(string Companyid)
        {
            TwilioSetting twilioSetting = new TwilioSetting();

            try
            {
                Database db = new Database();
                DataTable dt = new DataTable();
                string sql_Qry = @"SELECT  
                          ,[CompanyID]
                          ,[TwilioAccountSid]
                          ,[TwilioAccountAuthToken]
                          ,[TwilioPhoneNumber]
                      FROM [msSchedulerV3].[dbo].[tbl_TwilioSetting] Where CompanyID=@CompanyID;";
                DataSet dataSet = db.Get_DataSet(sql_Qry, Companyid.Trim());
                dt = dataSet.Tables[0];
                string startTimeString = string.Empty, endTimeString = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    twilioSetting.CompanyID = dr["CompanyID"].ToString().Trim();
                    twilioSetting.TwilioAccountAuthToken = dr["TwilioAccountAuthToken"].ToString().Trim();
                    twilioSetting.TwilioAccountSid = dr["TwilioAccountSid"].ToString().Trim();
                    twilioSetting.TwilioPhoneNumber = dr["TwilioPhoneNumber"].ToString().Trim();

                }
            }
            catch { }



            return twilioSetting;
        }
        bool LogSMS(string companyId, string customerId, string resourceId, string apptId, string mobile, string SMSType, string SMSBody, string smsSid)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnString"].ToString()))

            {
                connection.Open();
                string sqlQuery = string.Format("INSERT INTO [msSchedulerV3].[dbo].[tbl_TwilioSMSLog]  ([CompanyId] ,[CustomerId] ,[ResourceId] ,[AppointmentId],[ToNumber] ,[SMSType] ,[SMSBody],[SMSSid] ,[SendDateTime]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',GETDATE())", companyId, customerId, resourceId, apptId, mobile, SMSType, SMSBody, smsSid);
                SqlCommand command = new SqlCommand();
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;
                command.Connection = connection;

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public string SendCustomerAdHocSMS(string companyId, string customerId, string SMSBody, string mobile)
        {
            bool result = false;
            string response = "";
            try
            {
                string smsSid = SendSMS(mobile, SMSBody, companyId);
                result = LogSMS(companyId, customerId, "", "", mobile, "CustomerAdHoc", SMSBody, smsSid);
                if (result == true)
                {
                    response = "SMS Sent Successfully.";
                }
                else
                {
                    response = "Something went wrong, Please try again";
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }

            return response;
        }
    }
}