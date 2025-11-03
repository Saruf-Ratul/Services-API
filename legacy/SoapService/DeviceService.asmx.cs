using Newtonsoft.Json;
using ResponseEntity;
using Services.Entity;
using Services.Models;
using Services.Processor;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using static Services.Processor.AppointmentProcessor;

namespace Services
{
    /// <summary>
    /// Summary description for DeviceService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DeviceService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void VerifyUser(string UserName, string Password, int AppType)
        {
            Response response = new Response();
            LoginProcessor loginProcessor = new LoginProcessor();
            response = loginProcessor.VerifyUser(new RequestEntity { UserName = UserName, Password = Password, AppType = AppType });

            //  return new JavaScriptSerializer().Serialize(response);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAppointmentList(string appointmentDate, string companyId, string userId, int appointmentTypeStatus)
        {

            var response = new List<Appointment>();
            AppointmentProcessor appointmentProcessorProcessor = new AppointmentProcessor();
            response = appointmentProcessorProcessor.GetAllAppointments(appointmentDate, companyId, userId, appointmentTypeStatus);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetCustomerList(string Date, string CompanyId)
        {
            //  string formattedDate = filterDate.ToString("yyyy/MM/dd");
            var response = new List<Customer>();
            CustomerProcessor customerProcessorProcessor = new CustomerProcessor();
            response = customerProcessorProcessor.GetAllCustomers(Date, CompanyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetInvoiceList(string Date, string CompanyId)
        {
            var response = new List<InvoiceDetails>();
            InvoiceProcessor invoiceProcessor = new InvoiceProcessor();
            response = invoiceProcessor.GetInvoiceDetailsList(Date, CompanyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddCustomer(CustomerDTO customer)
        {
            try
            {
                CustomerProcessor customerProcessor = new CustomerProcessor();
                string result = customerProcessor.AddCustomer(customer);

                var response = new StringResult
                {
                    Status = result.StartsWith("Error:") ? "error" : "success",
                    Response = result
                };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/json";
                HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
                HttpContext.Current.Response.Write(js.Serialize(response));
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();

                // Context.Response.Write(js.Serialize(response));
            }
            catch (Exception ex)
            {
                var response = new StringResult
                {
                    Status = "Error",
                    Response = ex.Message
                };
                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                Context.Response.Write(js.Serialize(response));

            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetStatusList(string companyID)
        {
            var response = new List<Status>();
            AppointmentStatusProcessor AppointmentStatusProcessor = new AppointmentStatusProcessor();
            response = AppointmentStatusProcessor.GetAllAppointmentStatusList(companyID);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTicketStatusList(string companyID)
        {
            var response = new List<TicketStatus>();
            TicketStatusProcessor TicketStatusProcessor = new TicketStatusProcessor();
            response = TicketStatusProcessor.GetAllTicketStatus(companyID);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTaxList(string companyID)
        {
            var response = new List<Tax>();

            InvoiceProcessor processor = new InvoiceProcessor();
            response = processor.GetAllTaxes(companyID);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateAppointment(AppointmentDTO appointment)
        {
            var response = "";
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.UpdateAppointment(appointment);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();


            // Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAllItemList(string companyId)
        {
            var response = new List<Items>();
            ItemProcessor processor = new ItemProcessor();
            response = processor.GetAllItems(companyId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void TEST(string invoice)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            //  response.Message = invoiceProccessor.CreateInvoice(invoice, ref Issuccess);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";



            // Context.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Write("{property: value}");
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateInvoice(InvoiceDTO invoice)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            string id = string.Empty;
            response.Message = invoiceProccessor.CreateInvoice(invoice,
                ref Issuccess,
                ref id);
            response.IsValid = Issuccess;
            response.Id = id;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            // Context.Response.Write(js.Serialize(response));
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void SendHtmlFormattedEmail2(string CompanyID, string CustomerID, string EmailType, string subject, string body,
        //       string recepientToEmail, string recepientCCEmail, string recepientBCCEmail, List<EmailContent> emailContents, string UserId)
        //{
        //    Response response = new Response();
        //    EmailProcessor invoiceProccessor = new EmailProcessor();
        //    bool Issuccess = false;
        //    response.Message = invoiceProccessor.SendHtmlFormattedEmail(CompanyID, CustomerID, EmailType, subject, body, recepientToEmail, recepientCCEmail, recepientBCCEmail, emailContents, UserId);
        //    response.IsValid = Issuccess;

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    Context.Response.Clear();
        //    Context.Response.ContentType = "application/json";
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.ContentType = "application/json";
        //    HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
        //    HttpContext.Current.Response.Write(js.Serialize(response));
        //    HttpContext.Current.Response.Flush();
        //    HttpContext.Current.Response.End();

        //}
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddPayment(PaymentDTO payment)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            response.Message = invoiceProccessor.Addpayment(payment, ref Issuccess);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            // Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void EditInvoice(InvoiceEditDTO invoice)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            response.Message = invoiceProccessor.EditInvoice(invoice, ref Issuccess);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            // Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAutoGeneratedInvoice(string companyId, bool IsInvoice)
        {
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();

            string invoiceNo = invoiceProccessor.AutoGeneratedInvoiceNo(companyId, IsInvoice);
            var response = new
            {
                InvoiceNo = invoiceNo
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetXPayLink(string RMCompanyID, string CustomerID, string InvoiceNo, string CustomerName, string email, string amount)
        {

            XPayLinkProcessor xPayLinkProcessor = new XPayLinkProcessor();
            string link = xPayLinkProcessor.GetCSPaymentLink(RMCompanyID, CustomerID, InvoiceNo, CustomerName, email, amount);
            var response = new
            {
                XPayLink = link
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendHtmlFormattedEmail(string companyID, string customerID, string emailType, string subject, string body, string recepientToEmail, string recepientCCEmail, string recepientBCCEmail, List<EmailContent> emailContents, string userId, string currentPdfType, string invoiceNo, bool isSendPaymentLink)
        {
            Response response = new Response();
            bool Issuccess = false;
            response.IsValid = Issuccess;
            EmailProcessor emailProcessor = new EmailProcessor();
            response.Message = emailProcessor.ProcessInvoiceForEmail(companyID, customerID, emailType, subject, body, recepientToEmail, recepientCCEmail, recepientBCCEmail, emailContents, userId, currentPdfType, invoiceNo, isSendPaymentLink);
            if (response.Message == "Sent")
            {
                response.IsValid = true;
            }
            response.CompanyID = companyID;
            response.Id = invoiceNo;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
      
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAutoFillValuesForEmail(string companyId, string type)
        {
            EmailProcessor emailProcessor = new EmailProcessor();
            var response = emailProcessor.GetAutoFillValuesForEmail(companyId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ConvertEstimateToInvoice(string invoiceId, string modifiedBy, string companyId)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            response.Message = invoiceProccessor.ConvertInvoice(invoiceId, modifiedBy,  companyId);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            // Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GenerateXPayLink(string companyId, string customerId, string invoiceId, string customerName, string email, string amount)
        {

            XPayLinkProcessor xPayLinkProcessor = new XPayLinkProcessor();
            string link = xPayLinkProcessor.GetXpayLink(companyId, customerId, invoiceId, customerName, email, amount);
            var response = new
            {
                XPayLink = link
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendCustomerSMS(string companyId, string customerId, string SMSBody, string mobile)
        {
            try
            {
                TwilioProcessor twilioProcessor = new TwilioProcessor();
                string result = twilioProcessor.SendCustomerAdHocSMS(companyId, customerId, SMSBody, mobile);

                var response = new StringResult
                {
                    Status = result.StartsWith("Error:") ? "error" : "success",
                    Response = result
                };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/json";
                HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
                HttpContext.Current.Response.Write(js.Serialize(response));
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();

                // Context.Response.Write(js.Serialize(response));
            }
            catch (Exception ex)
            {
                var response = new StringResult
                {
                    Status = "Error",
                    Response = ex.Message
                };
                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                Context.Response.Write(js.Serialize(response));

            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAllFormTemplates(string companyId)
        {
            var response = new List<FormTemplate>();
            FormProcessor formProcessor= new FormProcessor();
            response = formProcessor.GetAllFormTemplates(companyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveImageForCSL(CSLImageDTO requestPeram)
        {
            var response = "";
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.SaveImageForCSL(requestPeram);

      
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetImageForCSL(int appointmentId, int customerId, int cSLId, string companyId)
        {
            var response = new List<CSLImageDTO>();
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.GetCSLImages(appointmentId, customerId, cSLId, companyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveFormTemplate(FormTemplate requestPeram)
        {
            var response = "";
            FormProcessor formProcessor = new FormProcessor();
            response = formProcessor.SaveFormTemplate(requestPeram);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteFormTemplate(int id, string companyId)
        {
            var response = "";
            FormProcessor formProcessor = new FormProcessor();
            response = formProcessor.DeleteFormTemplate(id, companyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AssignForm(AppointmnetForm requestPeram)
        {
            var response = "";
            AppointmentProcessor  appointment = new AppointmentProcessor();
            response = appointment.AssignForm(requestPeram);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateFormTemplate(FormTemplate requestPeram)
        {
            var response = "";
            FormProcessor formProcessor = new FormProcessor();
            response = formProcessor.UpdateFormTemplate(requestPeram);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAllTags(string companyId)
        {

            var response = new List<CSLTag>();
            AppointmentProcessor appointmentProcessorProcessor = new AppointmentProcessor();
            response = appointmentProcessorProcessor.GetAllTags(companyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveTag(CSLTag tag)
        {
            var response = "";
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.SaveTag(tag);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateTag(CSLTag tag)
        {
            var response = "";
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.UpdateTag(tag);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTagById(int id, string companyId)
        {

            var response = new CSLTag();
            AppointmentProcessor appointmentProcessorProcessor = new AppointmentProcessor();
            response = appointmentProcessorProcessor.GetTagById(id, companyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteTag(int id, string companyId)
        {
            var response = "";
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.DeleteTag(id, companyId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAppointmentListWithForms(string appointmentDate, string companyId, string userId, int appointmentTypeStatus)
        {

            var response = new List<AppoinmentListWithForms>();
            AppointmentProcessor appointmentProcessorProcessor = new AppointmentProcessor();
            response = appointmentProcessorProcessor.GetAllAppointmentsWithForms(appointmentDate, companyId, userId, appointmentTypeStatus);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveNote(Note note)
        {
            var response = "";
            NoteProcessor noteProcessor = new NoteProcessor();
            response = noteProcessor.SaveNote(note);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateNote(Note note)
        {
            var response = "";
            NoteProcessor noteProcessor = new NoteProcessor();
            response = noteProcessor.UpdateNote(note);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAllNote (string companyId)
        {

            var response = new List<Note>();
            NoteProcessor noteProcessor = new NoteProcessor();
            response = noteProcessor.GetAllNotes(companyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteNote(int id, string companyId)
        {
            var response = "";
            NoteProcessor noteProcessor = new NoteProcessor();
            response = noteProcessor.DeleteNote(id, companyId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
    }
}

