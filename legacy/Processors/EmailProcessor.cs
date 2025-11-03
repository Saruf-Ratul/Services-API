using CECPro.Wisetack;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace Services.Processor
{
    public class EmailProcessor
    {

        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        DataSet dataSet = null;
        public string SendHtmlFormattedEmail(string CompanyID, string CustomerID, string EmailType, string subject, string body,
                string recepientToEmail, string recepientCCEmail, string recepientBCCEmail, List<EmailContent> emailContents, string UserId)
        {

            string SendBy = UserId;
            string CompanyAddress = "";
            string CompanyCity = "";
            string CompanyState = "";
            string CompanyZipCode = "";
            string CompanyPhone = "";
            string CompanyEmail = "";
            string CompanyWebsite = "";
            string CompanyFacebook = "";
            string CompanyTwitter = "";
            string CompanyLogoFile = "";
            string CompanyFullName = "";
            string CompanyGUID = "";
            try
            {
                string wisetackFooter = "";

                Database db = new Database(connStr);

                string historyid = string.Empty;
                string EmailFrom = string.Empty;
                bool isSendPrequal = false;

                string prequalLink = string.Empty;


                string sql = "select * from msSchedulerV3.dbo.tbl_Company where CompanyID=@CompanyID;";

                sql += "Select [WisetackFooterMsg] from msSchedulerV3.dbo.tbl_CustCommunication where  CompanyID=@CompanyID;";

                sql += "Select ISNULL(Max(id), 0)+1 as newid from msSchedulerV3.dbo.tbl_EmailHistory;";

                sql += "Select EmailFrom from msSchedulerV3.dbo.tbl_CustCommunication where CompanyID = @CompanyID;";

                sql += "Select ISNULL(IsSendPrequal, 0) AS IsSendPrequal from msSchedulerV3.dbo.tbl_CustCommunication where  CompanyID=@CompanyID;";


                sql += "select ISNULL(PrequalLink, '') AS PrequalLink  From Wiseteck.dbo.tbl_MerchantSettings Where CompanyID =@CompanyID;";




                DataTable dt_Company = new DataTable();


                if (dataSet == null)
                {
                    dataSet = db.Get_DataSet(sql, CompanyID);
                }

                dt_Company = dataSet.Tables[0];

                wisetackFooter = dataSet.Tables[1].Rows.Count > 0 ? dataSet.Tables[1].Rows[0]["WisetackFooterMsg"].ToString() : "";
                historyid = dataSet.Tables[2].Rows.Count > 0 ? dataSet.Tables[2].Rows[0]["newid"].ToString() : "";
                EmailFrom = dataSet.Tables[3].Rows.Count > 0 ? dataSet.Tables[3].Rows[0]["EmailFrom"].ToString() : "";
                isSendPrequal = dataSet.Tables[4].Rows.Count > 0 ? Convert.ToBoolean(dataSet.Tables[4].Rows[0]["IsSendPrequal"].ToString()) : false;

                prequalLink = dataSet.Tables[5].Rows.Count > 0 ? dataSet.Tables[5].Rows[0]["PrequalLink"].ToString() : "";

                //db.Execute(sql, out dt_Company);

                if (dt_Company.Rows.Count > 0)
                {
                    DataRow Rs = dt_Company.Rows[0];
                    CompanyAddress = Rs["Address"].ToString();
                    CompanyCity = Rs["City"].ToString();
                    CompanyState = Rs["State"].ToString();
                    CompanyZipCode = Rs["ZipCode"].ToString();
                    CompanyPhone = Rs["Phone"].ToString();
                    CompanyEmail = Rs["Email"].ToString();
                    CompanyWebsite = Rs["Website"].ToString();
                    CompanyFacebook = Rs["Facebook"].ToString();
                    CompanyTwitter = Rs["Twitter"].ToString();
                    CompanyLogoFile = Rs["LogoFile"].ToString();
                    CompanyFullName = Rs["CompanyName"].ToString();
                    CompanyGUID = Rs["CompanyGUID"].ToString();

                }
                string Emailbody = body;

                if (!string.IsNullOrEmpty(body))
                {

                    StringBuilder builder = new StringBuilder(Emailbody);
                    builder.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
                    Emailbody = builder.ToString();

                }

                string LogoPath = HttpContext.Current.Server.MapPath("~/CompanyLogo/" + CompanyLogoFile);
                string header = "";

                string BodyText = "";
                BodyText = "<body><table style='max-width:800px; font-family:Arial;font-size:13px;'>" +
                 "<tr><td align='left'><table width='100%'><tr><td>" +
                 "<img src=\"cid:photo\"  id='img' alt='' style='max-height:60px;' height='60' /></td>" +
                 "<td align='center' style='color:#2980b9; vertical-align:bottom;'>" +
                 "</td></tr></table></td></tr>" +
                 "<tr><td align='left' style='padding-left:5px;'>" +
                 Emailbody +
                        "</td></tr>";


                BodyText = BodyText + "</table></td></tr></table></body>";
                string attachedfileNames = "";
                using (MailMessage mailMessage = new MailMessage())
                {


                    if (!string.IsNullOrEmpty(recepientToEmail))
                    {
                        string[] recepientToList = recepientToEmail.Split(',');
                        foreach (string multiple_email in recepientToList)
                        {
                            mailMessage.To.Add(new MailAddress(multiple_email));

                        }
                    }
                    if (!string.IsNullOrEmpty(recepientCCEmail))
                    {
                        string[] recepientCCList = recepientCCEmail.Split(',');
                        foreach (string multiple_email in recepientCCList)
                        {
                            mailMessage.CC.Add(new MailAddress(multiple_email));

                        }
                    }

                    if (!string.IsNullOrEmpty(recepientBCCEmail))
                    {
                        string[] recepientBCCList = recepientBCCEmail.Split(',');
                        foreach (string multiple_email in recepientBCCList)
                        {
                            mailMessage.Bcc.Add(multiple_email);

                        }
                    }

                    if (string.IsNullOrEmpty(EmailFrom) || EmailFrom == "0")
                    {
                        EmailFrom = "noreply@" + CompanyID + ".com";

                    }



                    if (!string.IsNullOrEmpty(prequalLink))
                    {
                        BodyText = BodyText.Replace("[Prequal]", prequalLink);
                    }
                    else
                    {
                        BodyText = BodyText.Replace("[Prequal]", "");
                    }
                    if (!string.IsNullOrEmpty(prequalLink))
                    {
                        BodyText = BodyText.Replace("[Prequal]", prequalLink);
                    }
                    if (isSendPrequal)
                    {
                        BodyText += "<br/><br/>Prequal Link: " + prequalLink;
                    }


                    mailMessage.From = new MailAddress(EmailFrom);
                    mailMessage.Subject = subject;
                    mailMessage.Body = BodyText;

                    if (!string.IsNullOrEmpty(CompanyLogoFile))
                    {

                        if (File.Exists(LogoPath))
                        {
                            AlternateView htmlview = default(AlternateView);
                            htmlview = AlternateView.CreateAlternateViewFromString(BodyText, null, "text/html");
                            LinkedResource imageResourceEs = new LinkedResource(LogoPath, MediaTypeNames.Image.Jpeg)
                            {
                                ContentId = "photo"
                            };
                
                            htmlview.LinkedResources.Add(imageResourceEs);
                            mailMessage.AlternateViews.Add(htmlview);
                        }
                        else
                        {
                            mailMessage.Body = BodyText;
                        }

                    }
                    else
                    {
                        mailMessage.Body = BodyText;
                    }
                    if (emailContents != null)
                    {
                        if (emailContents.Count > 0)
                        {
                            foreach (EmailContent f in emailContents)
                            {
                                Attachment attachment = new Attachment(HttpContext.Current.Server.MapPath(f.FileUrl)); //create the attachment
                                mailMessage.Attachments.Add(attachment); //add the attachment

                            }

                        }

                    }





          

                    mailMessage.IsBodyHtml = true;
                    string emailSentError = "";

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["SMTP"];
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = ConfigurationManager.AppSettings["SmtpUser"];
                    NetworkCred.Password = ConfigurationManager.AppSettings["SmtpPassword"];
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                    smtp.Send(mailMessage);

                    string emailhistory = "INSERT INTO msSchedulerV3.dbo.tbl_EmailHistory(id,CompanyID, CustomerID, Subject, EmailBody, EmailTo, EmailCC, EmailBCC, EmailType, SendBy)VALUES" +
                         "(" +
                         "'" + historyid + "'," +
                         "'" + CompanyID + "'," +
                         "'" + CustomerID + "'," +
                         "'" + subject + "'," +
                         "'" + body.Replace("'", string.Empty) + "'," +
                         "'" + recepientToEmail + "'," +
                         "'" + recepientCCEmail + "'," +
                         "'" + recepientBCCEmail + "'," +
                         "'" + EmailType + "'," +
                         "'" + SendBy + "'" +

                         ")";

                    db.Execute(emailhistory);
                    if (emailContents != null)
                    {
                        if (emailContents.Count > 0)
                        {
                            foreach (EmailContent e in emailContents)
                            {
                                SaveFileContent(historyid, CompanyID, e.FileName, e.FileUrl);
                            }
                        }
                    }

                }

                return "Sent";



            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public void SaveFileContent(string historyid, string CompanyID, string filename, string FileUrl)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "insert into msSchedulerV3.dbo.tbl_EmailHistoryContent(HistoryID,CompanyID,FileName,FileUrl) values (@HistoryID,@CompanyID,@FileName,  @FileUrl)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@HistoryID", historyid);
                    cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                    cmd.Parameters.AddWithValue("@FileName", filename);
                    cmd.Parameters.AddWithValue("@FileUrl", FileUrl);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        public byte[] EstimatePdfLHG(string companyID, string CustomerID, string InvoiceNo, string doctype, string CSPaymentLink, bool IsInvoiceWithoutDiscount, decimal depoReqAmt = 0)
        {
            CustomerID = Common.CleanInput(CustomerID);
            string body = string.Empty;
            string LogoPath = "";
            string SignaturePath = "";
            string paylink = "";

            string connStr = ConfigurationManager.AppSettings["ConnStrSch"].ToString();
            Database db = new Database(connStr);

            string sSQL = "";

            sSQL += "SELECT CONCAT(FirstName, ' ', LastName) AS FullName, " +
                    "CASE WHEN BusinessID = 0 THEN 1 ELSE 2 END AS ctype, " +
                    "ISNULL(QboId, N'0') AS qboCustID, * " +
                    "FROM msSchedulerV3.dbo.tbl_Customer WHERE CustomerId='" + CustomerID + "' AND CompanyID = @CompanyID;";

            sSQL += "SELECT CONVERT(VARCHAR(10), InvoiceDate, 101) AS IssueDate, " +
                    "CONVERT(VARCHAR, ExpirationDate, 107) AS ExpDate, " +
                    "ISNULL(QboId, N'0') AS qboInvID, ISNULL(QboEstimateId, 0) AS qboEstID, " +
                    "ISNULL(AmountCollect, 0.00) AS DepositAmount, Discount, Tax, " +
                    "ISNULL(RequestedDepoAmt, 0) AS ReqDepo, " +
                    "(Total - ISNULL(AmountCollect, 0.00)) AS Due, * " +
                    "FROM msSchedulerV3.dbo.tbl_Invoice WHERE CustomerId='" + CustomerID + "' AND id='" + InvoiceNo + "' AND CompnyID = @CompanyID;";

            sSQL += "SELECT inv.TotalPrice, inv.Description, inv.Quantity, inv.uPrice, inv.ItemId AS ID, " +
                    "i.Name AS ItemName FROM msSchedulerV3.dbo.Items i " +
                    "INNER JOIN msSchedulerV3.dbo.tbl_InvoiceDetails inv ON i.Id = inv.ItemId AND i.CompanyID = inv.CompanyID " +
                    "WHERE inv.RefId='" + InvoiceNo + "' AND inv.CompanyID = @CompanyID " +
                    "ORDER BY CAST(NULLIF(inv.LineNum, '') AS INT) ASC;";

            sSQL += "SELECT * FROM msSchedulerV3.dbo.tbl_Company WHERE CompanyID = @CompanyID;";
            sSQL += "SELECT * FROM msSchedulerV3.dbo.tbl_DisclaimerSettings WHERE CompanyID = @CompanyID;";

            System.Data.DataSet ds = db.Get_DataSet(sSQL, companyID);


            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/LHG_Estimate.html")))
            {
                body = reader.ReadToEnd();
            }


            System.Data.DataRow company = ds.Tables[3].Rows[0];
            string CompanyName = company["CompanyName"].ToString();
            string CompanyAddress = company["Address"].ToString();
            string CompanyPhone = company["Phone"].ToString();
            string CompanyEmail = company["Email"].ToString();
            string companyLogoFile = company["LogoFile"].ToString();

            LogoPath = HttpContext.Current.Server.MapPath("~/CompanyLogo/" + companyLogoFile);
            if (!File.Exists(LogoPath)) LogoPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/central.png");

            SignaturePath = HttpContext.Current.Server.MapPath("~/images/Signature.PNG");
            if (!File.Exists(SignaturePath)) SignaturePath = HttpContext.Current.Server.MapPath("~/images/Signature.PNG");

            //string base64Logo = Convert.ToBase64String(File.ReadAllBytes(LogoPath));
            body = body.Replace("{image}", LogoPath);
            string base64Signature = Convert.ToBase64String(File.ReadAllBytes(SignaturePath));

            System.Data.DataRow cust = ds.Tables[0].Rows[0];
            string phoneNumber = cust["Phone"].ToString();
            int digitCount = phoneNumber.Count(char.IsDigit);
            if (digitCount > 9)
            {
                phoneNumber = String.Format("({0}) {1}-{2}",
                  cust["Phone"].ToString().Substring(0, 3),
                  cust["Phone"].ToString().Substring(3, 3),
                  cust["Phone"].ToString().Substring(6, 4)
                 );
            }

            body = body.Replace("${customerName}", cust["FullName"].ToString());
            body = body.Replace("${address}", cust["Address1"].ToString());
            body = body.Replace("${phone}", phoneNumber);
            body = body.Replace("${email}", cust["Email"].ToString());

            //body = body.Replace("${image}", $"data:image/png;base64,{base64Logo}");
            body = body.Replace("${Signatureimage}", $"data:image/png;base64,{base64Signature}");

            string phoneNumberCompany = CompanyPhone;
            int digitCountcom = phoneNumberCompany.Count(char.IsDigit);

            if (digitCountcom > 9)
            {
                phoneNumberCompany = String.Format("({0}) {1}-{2}",
                CompanyPhone.Substring(0, 3),
                CompanyPhone.Substring(3, 3),
                CompanyPhone.Substring(6, 4)
            );
            }

            body = body.Replace("${CompanyName}", CompanyName);
            body = body.Replace("${companyAddress}", CompanyAddress);
            body = body.Replace("${companyPhone}", phoneNumberCompany);
            body = body.Replace("${companyEmail}", CompanyEmail);
            // No icon file loading needed; SVGs are embedded in the HTML template
            //body = body.Replace("${locationIcon}", "");
            //body = body.Replace("${phoneIcon}", "");
            //body = body.Replace("${emailIcon}", "");
            string locationIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/locationicon.png");
            if (!File.Exists(locationIconPath)) locationIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");
            string phoneIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/phoneicon.png");
            if (!File.Exists(phoneIconPath)) phoneIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");
            string emailIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/emailicon.png");
            if (!File.Exists(emailIconPath)) emailIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");

            // Placeholders with icon file paths
            body = body.Replace("${locationIcon}", locationIconPath);
            body = body.Replace("${phoneIcon}", phoneIconPath);
            body = body.Replace("${emailIcon}", emailIconPath);


            System.Data.DataRow inv = ds.Tables[1].Rows[0];
            body = body.Replace("${IssueDate}", inv["IssueDate"].ToString());
            body = body.Replace("${InvoiceNo}", inv["Number"].ToString());
            body = body.Replace("${InvoiceTotal}", Convert.ToDecimal(inv["Total"]).ToString("N2"));
            body = body.Replace("${SubTotal}", Convert.ToDecimal(inv["Subtotal"]).ToString("N2"));
            body = body.Replace("${Discount}", Convert.ToDecimal(inv["Discount"]).ToString("N2"));
            body = body.Replace("${Total}", Convert.ToDecimal(inv["Total"]).ToString("N2"));
            body = body.Replace("${Tax}", Convert.ToDecimal(inv["Tax"]).ToString("N2"));
            body = body.Replace("${Deposit}", Convert.ToDecimal(inv["DepositAmount"]).ToString("N2"));
            body = body.Replace("${BalanceDue}", Convert.ToDecimal(inv["Due"]).ToString("N2"));
            body = body.Replace("${ReqDepo}", depoReqAmt.ToString("N2"));

            string itemDetails = "";
            foreach (System.Data.DataRow row in ds.Tables[2].Rows)
            {
                itemDetails += "<tr>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>" + row["ItemName"] + "</td>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>" + row["Description"] + "</td>" +
                  "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>" + row["Quantity"] + "</td>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>$ " + row["uPrice"] + "</td>" +

                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>$ " + row["TotalPrice"] + "</td>" +
                 "</tr>";
            }
            body = body.Replace("${itemRow}", itemDetails);

            //if (HttpContext.Current.Session["LoanInfo"] != null)
            //{
            //    LoanInfo ln = (LoanInfo)HttpContext.Current.Session["LoanInfo"];
            //    paylink = ln.PaymentLink;
            //    body = body.Replace("${AsLowAsAmount}", ln.AsLowAsAmount ?? "");
            //    body = body.Replace("${wisetackclicktext}", "Click this link to see your financing options");
            //    body = body.Replace("${PaymentLink}", ln.PaymentLink);
            //}
            //else
            //{
            //    body = body.Replace("${AsLowAsAmount}", "");
            //    body = body.Replace("${wisetackclicktext}", "");
            //    body = body.Replace("${PaymentLink}", "");
            //}

            body = body.Replace("${CSclicktext}", string.IsNullOrEmpty(CSPaymentLink) ? "" : "Click this link to pay now");
            body = body.Replace("{paymentLink}", CSPaymentLink ?? "");

            if (string.IsNullOrEmpty(paylink) && ds.Tables[4].Rows.Count > 0)
            {
                string disclaimer = (doctype == "Invoice") ? ds.Tables[4].Rows[0]["InvoiceDisclaimer"].ToString()
                                                           : ds.Tables[4].Rows[0]["QuoteDisclaimer"].ToString();
                body = body.Replace("[Disclimer]", disclaimer);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 40f, 40f, 40f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                // Add company logo (existing code)
                //if (File.Exists(LogoPath))
                //{
                //    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(LogoPath);
                //    logo.ScaleToFit(50f, 50f);
                //    logo.Alignment = Element.ALIGN_RIGHT;
                //    pdfDoc.Add(logo);
                //}

                // Parse the HTML content
                using (StringReader sr = new StringReader(body))
                {
                    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                }

                // Add icons directly to the PDF at specific positions
                PdfContentByte cb = writer.DirectContent;
                //string locationIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/locationicon.png");
                //string phoneIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/phoneicon.png");
                //string emailIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/emailicon.png");
                //string defaultIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");

                // Log paths for debugging
                System.Diagnostics.Debug.WriteLine("Location Icon Path: " + locationIconPath);
                System.Diagnostics.Debug.WriteLine("Phone Icon Path: " + phoneIconPath);
                System.Diagnostics.Debug.WriteLine("Email Icon Path: " + emailIconPath);
                //System.Diagnostics.Debug.WriteLine("Default Icon Path: " + defaultIconPath);

                // Define positions (adjust these based on your PDF layout)
                //float tableYPosition = 709f; for dev
                //float tableYPosition = 680f; // Vertical position (adjust up/down, e.g., 710f or 730f)  for live
                //float iconSize = 14f; // Icon height/width
                //float addressX = 91f; // Left column (adjust left/right, e.g., 40f or 60f)
                //float phoneX = 235f; // Middle column (adjust left/right, e.g., 200f or 240f)
                //float emailX = 380f; // Right column (adjust left/right, e.g., 360f or 400f)

                //// Add location icon
                //if (File.Exists(locationIconPath))
                //{
                //    iTextSharp.text.Image locationIcon = iTextSharp.text.Image.GetInstance(locationIconPath);
                //    locationIcon.ScaleToFit(iconSize, iconSize);
                //    locationIcon.SetAbsolutePosition(addressX, tableYPosition);
                //    cb.AddImage(locationIcon);
                //}
                //else if (File.Exists(defaultIconPath))
                //{
                //    iTextSharp.text.Image defaultIcon = iTextSharp.text.Image.GetInstance(defaultIconPath);
                //    defaultIcon.ScaleToFit(iconSize, iconSize);
                //    defaultIcon.SetAbsolutePosition(addressX, tableYPosition);
                //    cb.AddImage(defaultIcon);
                //}

                //// Add phone icon
                //if (File.Exists(phoneIconPath))
                //{
                //    iTextSharp.text.Image phoneIcon = iTextSharp.text.Image.GetInstance(phoneIconPath);
                //    phoneIcon.ScaleToFit(iconSize, iconSize);
                //    phoneIcon.SetAbsolutePosition(phoneX, tableYPosition);
                //    cb.AddImage(phoneIcon);
                //}
                //else if (File.Exists(defaultIconPath))
                //{
                //    iTextSharp.text.Image defaultIcon = iTextSharp.text.Image.GetInstance(defaultIconPath);
                //    defaultIcon.ScaleToFit(iconSize, iconSize);
                //    defaultIcon.SetAbsolutePosition(phoneX, tableYPosition);
                //    cb.AddImage(defaultIcon);
                //}

                //// Add email icon
                //if (File.Exists(emailIconPath))
                //{
                //    iTextSharp.text.Image emailIcon = iTextSharp.text.Image.GetInstance(emailIconPath);
                //    emailIcon.ScaleToFit(iconSize, iconSize);
                //    emailIcon.SetAbsolutePosition(emailX, tableYPosition);
                //    cb.AddImage(emailIcon);
                //}
                //else if (File.Exists(defaultIconPath))
                //{
                //    iTextSharp.text.Image defaultIcon = iTextSharp.text.Image.GetInstance(defaultIconPath);
                //    defaultIcon.ScaleToFit(iconSize, iconSize);
                //    defaultIcon.SetAbsolutePosition(emailX, tableYPosition);
                //    cb.AddImage(defaultIcon);
                //}


                // Add payment link (existing code)
                if (!string.IsNullOrEmpty(paylink))
                {
                    PdfContentByte cbLink = writer.DirectContent;
                    BaseFont font = BaseFont.CreateFont();
                    iTextSharp.text.Rectangle linkArea = new iTextSharp.text.Rectangle(pdfDoc.PageSize.Width - 200, 720, pdfDoc.PageSize.Width - 40, 750);
                    cbLink.SetColorFill(new BaseColor(7, 192, 202));
                    cbLink.Rectangle(linkArea.Left, linkArea.Bottom, linkArea.Width, linkArea.Height);
                    cbLink.Fill();
                    cbLink.SetColorFill(BaseColor.WHITE);
                    cbLink.BeginText();
                    cbLink.SetFontAndSize(font, 12);
                    cbLink.ShowTextAligned(Element.ALIGN_CENTER, "See Financing Options", linkArea.Left + linkArea.Width / 2, linkArea.Bottom + 8, 0);
                    cbLink.EndText();
                    PdfAnnotation annotation = PdfAnnotation.CreateLink(writer, linkArea, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(paylink));
                    writer.AddAnnotation(annotation);
                }

                // Add "Pay Now" button (existing code)
                if (!string.IsNullOrEmpty(CSPaymentLink))
                {
                    Font instructionFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(138, 138, 138));
                    Paragraph instruction = new Paragraph("To approve this invoice and pay the amount due, click the button below.", instructionFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 5f,
                        SpacingBefore = 15f
                    };
                    instruction.SetLeading(1.2f, 1.2f);
                    pdfDoc.Add(instruction);

                    PdfPTable table = new PdfPTable(1);
                    table.TotalWidth = 120f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SpacingBefore = 5f;
                    table.SpacingAfter = 10f;

                    Font linkFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                    Chunk linkChunk = new Chunk("Pay Now", linkFont);
                    linkChunk.SetAnchor(CSPaymentLink);

                    PdfPCell cell = new PdfPCell(new Phrase(linkChunk))
                    {
                        BackgroundColor = new BaseColor(255, 69, 0),
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 8f,
                        FixedHeight = 30f
                    };

                    cell.CellEvent = new RoundedCellEvent(10f);
                    table.AddCell(cell);
                    pdfDoc.Add(table);
                }

                pdfDoc.Close();
                return ms.ToArray();
            }
        }

        public byte[] InvoicePdfForLHGNew(string companyID,string CustomerID, string InvoiceNo, string doctype, string CSPaymentLink, decimal depoReqAmt = 0)
        {
            CustomerID = Common.CleanInput(CustomerID);
            string body = string.Empty;
            string LogoPath = "";
            string SignaturePath = "";
            string paylink = "";

            string connStr = ConfigurationManager.AppSettings["ConnStrSch"].ToString();
            Database db = new Database(connStr);

            string sSQL = "";

            sSQL += "SELECT CONCAT(FirstName, ' ', LastName) AS FullName, " +
                    "CASE WHEN BusinessID = 0 THEN 1 ELSE 2 END AS ctype, " +
                    "ISNULL(QboId, N'0') AS qboCustID, * " +
                    "FROM msSchedulerV3.dbo.tbl_Customer WHERE CustomerId='" + CustomerID + "' AND CompanyID = @CompanyID;";

            sSQL += "SELECT CONVERT(VARCHAR(10), InvoiceDate, 101) AS IssueDate, " +
                    "CONVERT(VARCHAR, ExpirationDate, 107) AS ExpDate, " +
                    "ISNULL(QboId, N'0') AS qboInvID, ISNULL(QboEstimateId, 0) AS qboEstID, " +
                    "ISNULL(AmountCollect, 0.00) AS DepositAmount, Discount, Tax, " +
                    "ISNULL(RequestedDepoAmt, 0) AS ReqDepo, " +
                    "(Total - ISNULL(AmountCollect, 0.00)) AS Due, * " +
                    "FROM msSchedulerV3.dbo.tbl_Invoice WHERE CustomerId='" + CustomerID + "' AND id='" + InvoiceNo + "' AND CompnyID = @CompanyID;";

            sSQL += "SELECT inv.TotalPrice, inv.Description, inv.Quantity, inv.uPrice, inv.ItemId AS ID, " +
                    "i.Name AS ItemName FROM msSchedulerV3.dbo.Items i " +
                    "INNER JOIN msSchedulerV3.dbo.tbl_InvoiceDetails inv ON i.Id = inv.ItemId AND i.CompanyID = inv.CompanyID " +
                    "WHERE inv.RefId='" + InvoiceNo + "' AND inv.CompanyID = @CompanyID " +
                    "ORDER BY CAST(NULLIF(inv.LineNum, '') AS INT) ASC;";

            sSQL += "SELECT * FROM msSchedulerV3.dbo.tbl_Company WHERE CompanyID = @CompanyID;";
            sSQL += "SELECT * FROM msSchedulerV3.dbo.tbl_DisclaimerSettings WHERE CompanyID = @CompanyID;";

            System.Data.DataSet ds = db.Get_DataSet(sSQL, companyID);


            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/LHG_Invoice.html")))
            {
                body = reader.ReadToEnd();
            }

            System.Data.DataRow company = ds.Tables[3].Rows[0];
            string CompanyName = company["CompanyName"].ToString();
            string CompanyAddress = company["Address"].ToString();
            string CompanyPhone = company["Phone"].ToString();
            string CompanyEmail = company["Email"].ToString();
            string companyLogoFile = company["LogoFile"].ToString();

            LogoPath = HttpContext.Current.Server.MapPath("~/CompanyLogo/" + companyLogoFile);
            if (!File.Exists(LogoPath)) LogoPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/central.png");

            SignaturePath = HttpContext.Current.Server.MapPath("~/images/Signature.PNG");
            if (!File.Exists(SignaturePath)) SignaturePath = HttpContext.Current.Server.MapPath("~/images/Signature.PNG");

            //string base64Logo = Convert.ToBase64String(File.ReadAllBytes(LogoPath));
            string base64Signature = Convert.ToBase64String(File.ReadAllBytes(SignaturePath));


            System.Data.DataRow cust = ds.Tables[0].Rows[0];

            string phoneNumber = cust["Phone"].ToString();
            int digitCount = phoneNumber.Count(char.IsDigit);
            if (digitCount > 9)
            {
                phoneNumber = String.Format("({0}) {1}-{2}",
                  cust["Phone"].ToString().Substring(0, 3),
                  cust["Phone"].ToString().Substring(3, 3),
                  cust["Phone"].ToString().Substring(6, 4)
                 );
            }


            body = body.Replace("${customerName}", cust["FullName"].ToString());
            body = body.Replace("${address}", cust["Address1"].ToString());
            body = body.Replace("${phone}", phoneNumber);
            body = body.Replace("${email}", cust["Email"].ToString());

            body = body.Replace("{image}", LogoPath);
            body = body.Replace("${Signatureimage}", $"data:image/png;base64,{base64Signature}");

            string phoneNumberCompany = CompanyPhone;
            int digitCountcom = phoneNumberCompany.Count(char.IsDigit);

            if (digitCountcom > 9)
            {
                phoneNumberCompany = String.Format("({0}) {1}-{2}",
                CompanyPhone.Substring(0, 3),
                CompanyPhone.Substring(3, 3),
                CompanyPhone.Substring(6, 4)
            );
            }

            body = body.Replace("${CompanyName}", CompanyName);
            body = body.Replace("${companyAddress}", CompanyAddress);
            body = body.Replace("${companyPhone}", phoneNumberCompany);
            body = body.Replace("${companyEmail}", CompanyEmail);
            //// No icon file loading needed; SVGs are embedded in the HTML template
            //body = body.Replace("${locationIcon}", "");
            //body = body.Replace("${phoneIcon}", "");
            //body = body.Replace("${emailIcon}", "");
            // Defining icon paths
            string locationIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/locationicon.png");
            if (!File.Exists(locationIconPath)) locationIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");
            string phoneIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/phoneicon.png");
            if (!File.Exists(phoneIconPath)) phoneIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");
            string emailIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/emailicon.png");
            if (!File.Exists(emailIconPath)) emailIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png");

            // Placeholders with icon file paths
            body = body.Replace("${locationIcon}", locationIconPath);
            body = body.Replace("${phoneIcon}", phoneIconPath);
            body = body.Replace("${emailIcon}", emailIconPath);


            System.Data.DataRow inv = ds.Tables[1].Rows[0];
            body = body.Replace("${IssueDate}", inv["IssueDate"].ToString());
            body = body.Replace("${InvoiceNo}", inv["Number"].ToString());
            body = body.Replace("${InvoiceTotal}", Convert.ToDecimal(inv["Total"]).ToString("N2"));
            body = body.Replace("${SubTotal}", Convert.ToDecimal(inv["Subtotal"]).ToString("N2"));
            body = body.Replace("${Discount}", Convert.ToDecimal(inv["Discount"]).ToString("N2"));
            body = body.Replace("${Total}", Convert.ToDecimal(inv["Total"]).ToString("N2"));
            body = body.Replace("${Tax}", Convert.ToDecimal(inv["Tax"]).ToString("N2"));
            body = body.Replace("${Deposit}", Convert.ToDecimal(inv["DepositAmount"]).ToString("N2"));
            body = body.Replace("${BalanceDue}", Convert.ToDecimal(inv["Due"]).ToString("N2"));
            body = body.Replace("${ReqDepo}", depoReqAmt.ToString("N2"));

            string itemDetails = "";
            foreach (System.Data.DataRow row in ds.Tables[2].Rows)
            {
                itemDetails += "<tr>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>" + row["ItemName"] + "</td>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>" + row["Description"] + "</td>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>" + row["Quantity"] + "</td>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>$ " + row["uPrice"] + "</td>" +
                 "<td style='text-align: left; border-bottom: 1px solid #8a8a8a;'>$ " + row["TotalPrice"] + "</td>" +
                 "</tr>";
            }
            body = body.Replace("${itemRow}", itemDetails);

            //if (HttpContext.Current.Session["LoanInfo"] != null)
            //{
            //    LoanInfo ln = (LoanInfo)HttpContext.Current.Session["LoanInfo"];
            //    paylink = ln.PaymentLink;
            //    body = body.Replace("${AsLowAsAmount}", ln.AsLowAsAmount ?? "");
            //    body = body.Replace("${wisetackclicktext}", "Click this link to see your financing options");
            //    body = body.Replace("${PaymentLink}", ln.PaymentLink);
            //}
            //else
            //{
            //    body = body.Replace("${AsLowAsAmount}", "");
            //    body = body.Replace("${wisetackclicktext}", "");
            //    body = body.Replace("${PaymentLink}", "");
            //}

            body = body.Replace("${CSclicktext}", string.IsNullOrEmpty(CSPaymentLink) ? "" : "Click this link to pay now");
            body = body.Replace("{paymentLink}", CSPaymentLink ?? "");

            if (string.IsNullOrEmpty(paylink) && ds.Tables[4].Rows.Count > 0)
            {
                string disclaimer = (doctype == "Invoice") ? ds.Tables[4].Rows[0]["InvoiceDisclaimer"].ToString()
                                                           : ds.Tables[4].Rows[0]["QuoteDisclaimer"].ToString();
                body = body.Replace("[Disclimer]", disclaimer);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 40f, 40f, 40f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                //// Add company logo (existing code)
                //if (File.Exists(LogoPath))
                //{
                //    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(LogoPath);
                //    logo.ScaleToFit(50f, 50f);
                //    logo.Alignment = Element.ALIGN_RIGHT;
                //    pdfDoc.Add(logo);
                //}

                // Parse the HTML content
                using (StringReader sr = new StringReader(body))
                {
                    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                }

                // Add icons directly to the PDF at specific positions
                PdfContentByte cb = writer.DirectContent;
                //string locationIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/locationicon.png");
                //string phoneIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/phoneicon.png");
                //string emailIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/emailicon.png");
                /*  string defaultIconPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/default_icon.png")*/
                ;

                // Log paths for debugging
                System.Diagnostics.Debug.WriteLine("Location Icon Path: " + locationIconPath);
                System.Diagnostics.Debug.WriteLine("Phone Icon Path: " + phoneIconPath);
                System.Diagnostics.Debug.WriteLine("Email Icon Path: " + emailIconPath);
                //System.Diagnostics.Debug.WriteLine("Default Icon Path: " + defaultIconPath);

                //// Define positions (adjust these based on your PDF layout)

                ////float tableYPosition = 709f;
                //float tableYPosition = 680f; // Vertical position (adjust up/down, e.g., 710f or 730f)
                //float iconSize = 14f; // Icon height/width
                //float addressX = 91f; // Left column (adjust left/right, e.g., 40f or 60f)
                //float phoneX = 235f; // Middle column (adjust left/right, e.g., 200f or 240f)
                //float emailX = 380f; // Right column (adjust left/right, e.g., 360f or 400f)

                //// Add location icon
                //if (File.Exists(locationIconPath))
                //{
                //    iTextSharp.text.Image locationIcon = iTextSharp.text.Image.GetInstance(locationIconPath);
                //    locationIcon.ScaleToFit(iconSize, iconSize);
                //    locationIcon.SetAbsolutePosition(addressX, tableYPosition);
                //    cb.AddImage(locationIcon);
                //}
                //else if (File.Exists(defaultIconPath))
                //{
                //    iTextSharp.text.Image defaultIcon = iTextSharp.text.Image.GetInstance(defaultIconPath);
                //    defaultIcon.ScaleToFit(iconSize, iconSize);
                //    defaultIcon.SetAbsolutePosition(addressX, tableYPosition);
                //    cb.AddImage(defaultIcon);
                //}

                //// Add phone icon
                //if (File.Exists(phoneIconPath))
                //{
                //    iTextSharp.text.Image phoneIcon = iTextSharp.text.Image.GetInstance(phoneIconPath);
                //    phoneIcon.ScaleToFit(iconSize, iconSize);
                //    phoneIcon.SetAbsolutePosition(phoneX, tableYPosition);
                //    cb.AddImage(phoneIcon);
                //}
                //else if (File.Exists(defaultIconPath))
                //{
                //    iTextSharp.text.Image defaultIcon = iTextSharp.text.Image.GetInstance(defaultIconPath);
                //    defaultIcon.ScaleToFit(iconSize, iconSize);
                //    defaultIcon.SetAbsolutePosition(phoneX, tableYPosition);
                //    cb.AddImage(defaultIcon);
                //}

                //// Add email icon
                //if (File.Exists(emailIconPath))
                //{
                //    iTextSharp.text.Image emailIcon = iTextSharp.text.Image.GetInstance(emailIconPath);
                //    emailIcon.ScaleToFit(iconSize, iconSize);
                //    emailIcon.SetAbsolutePosition(emailX, tableYPosition);
                //    cb.AddImage(emailIcon);
                //}
                //else if (File.Exists(defaultIconPath))
                //{
                //    iTextSharp.text.Image defaultIcon = iTextSharp.text.Image.GetInstance(defaultIconPath);
                //    defaultIcon.ScaleToFit(iconSize, iconSize);
                //    defaultIcon.SetAbsolutePosition(emailX, tableYPosition);
                //    cb.AddImage(defaultIcon);
                //}


                // Add payment link (existing code)
                if (!string.IsNullOrEmpty(paylink))
                {
                    PdfContentByte cbLink = writer.DirectContent;
                    BaseFont font = BaseFont.CreateFont();
                    iTextSharp.text.Rectangle linkArea = new iTextSharp.text.Rectangle(pdfDoc.PageSize.Width - 200, 720, pdfDoc.PageSize.Width - 40, 750);
                    cbLink.SetColorFill(new BaseColor(7, 192, 202));
                    cbLink.Rectangle(linkArea.Left, linkArea.Bottom, linkArea.Width, linkArea.Height);
                    cbLink.Fill();
                    cbLink.SetColorFill(BaseColor.WHITE);
                    cbLink.BeginText();
                    cbLink.SetFontAndSize(font, 12);
                    cbLink.ShowTextAligned(Element.ALIGN_CENTER, "See Financing Options", linkArea.Left + linkArea.Width / 2, linkArea.Bottom + 8, 0);
                    cbLink.EndText();
                    PdfAnnotation annotation = PdfAnnotation.CreateLink(writer, linkArea, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(paylink));
                    writer.AddAnnotation(annotation);
                }

                // Add "Pay Now" button (existing code)
                if (!string.IsNullOrEmpty(CSPaymentLink))
                {
                    Font instructionFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(138, 138, 138));
                    Paragraph instruction = new Paragraph("To approve this invoice and pay the amount due, click the button below.", instructionFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 5f,
                        SpacingBefore = 15f
                    };
                    instruction.SetLeading(1.2f, 1.2f);
                    pdfDoc.Add(instruction);

                    PdfPTable table = new PdfPTable(1);
                    table.TotalWidth = 120f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SpacingBefore = 5f;
                    table.SpacingAfter = 10f;

                    Font linkFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                    Chunk linkChunk = new Chunk("Pay Now", linkFont);
                    linkChunk.SetAnchor(CSPaymentLink);

                    PdfPCell cell = new PdfPCell(new Phrase(linkChunk))
                    {
                        BackgroundColor = new BaseColor(255, 69, 0),
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 8f,
                        FixedHeight = 30f
                    };

                    cell.CellEvent = new RoundedCellEvent(10f);
                    table.AddCell(cell);
                    pdfDoc.Add(table);
                }

                pdfDoc.Close();
                return ms.ToArray();
            }
        }
        public class RoundedCellEvent : IPdfPCellEvent
        {
            private float radius;

            public RoundedCellEvent(float radius)
            {
                this.radius = radius;
            }

            public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
            {
                PdfContentByte canvas = canvases[PdfPTable.BACKGROUNDCANVAS];
                canvas.SaveState();
                canvas.SetColorFill(cell.BackgroundColor);
                canvas.RoundRectangle(
                    position.GetLeft(0) + 2, // Adjust for padding
                    position.GetBottom(0) + 2,
                    position.Width - 4,
                    position.Height - 4,
                    radius
                );
                canvas.Fill();
                canvas.RestoreState();
            }
        }
        public byte[] InvoicePdf(string CustomerID, string InvoiceNo, string doctype, string CSPaymentLink,string companyID, decimal depoReqAmt = 0)
        {
            try
            {

                CustomerID = Common.CleanInput(CustomerID);
                string Host = "";
                string LogoPath = "";
                string body = string.Empty;
                string MailTo = "";
                string ProposalMailSubject = "";
                string ProposalMailBody = "";
                string CompanyLogoFile = "";

                string CompanyAddress = "";
                string CompanyCity = "";
                string CompanyState = "";
                string CompanyZipCode = "";
                string CompanyPhone = "";
                string CompanyEmail = "";
                string CompanyWebsite = "";
                string CompanyFacebook = "";
                string CompanyTwitter = "";
                string companyFax = "";
                string CompanyFullName = "";

                string connStr = ConfigurationManager.AppSettings["ConnStrSch"].ToString();
                Database db = new Database(connStr);

                DataTable dtCompany = new DataTable();
                string sql = @"SELECT [CompanyType] FROM [XinatorCentral].dbo.tbl_Company where  CompanyID='" + companyID + "'";
                DataSet ds = db.Get_DataSet(sql, companyID);
                dtCompany = ds.Tables[0];

                string companyType = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        companyType = ds.Tables[0].Rows[0]["CompanyType"].ToString();
                    }
                }

                if (companyType == "LHG")
                {
                    if (doctype == "Invoice")
                    {
                        var invLHG = InvoicePdfForLHGNew(companyID,CustomerID, InvoiceNo, doctype, CSPaymentLink, depoReqAmt);
                        return invLHG;
                    }
                    else
                    {
                        var lhgEstimate = EstimatePdfLHG(companyID,CustomerID, InvoiceNo, doctype, CSPaymentLink, false, depoReqAmt);
                        return lhgEstimate;
                    }
                }

                string sSQL = @"SELECT concat (FirstName,' ',LastName)as FullName,CASE WHEN dbo.tbl_Customer.BusinessID = 0 THEN 1 ELSE 2 END as ctype,ISNULL(dbo.tbl_Customer.QboId, N'0') as qboCustID,* FROM[msSchedulerV3].[dbo].[tbl_Customer] Where CustomerId='" + CustomerID + "' and  CompanyID =@CompanyID  ;";

                sSQL += @"SELECT CONVERT(VARCHAR(10), InvoiceDate, 101)as IssueDate,convert(varchar, ExpirationDate, 107) as ExpDate,ISNULL(dbo.tbl_Invoice.QboId, N'0') as qboInvID,ISNULL(dbo.tbl_Invoice.QboEstimateId, 0) as qboEstID,isnull([AmountCollect],0.00) as DepositAmount,Discount,Tax,(Total- (isnull(AmountCollect,0.00))) as Due,* FROM[msSchedulerV3].[dbo].[tbl_Invoice] Where  CustomerId='" + CustomerID + "' and id='" + InvoiceNo + "' and CompnyID =@CompanyID;";

                sSQL += "SELECT inv.Description,i.Location,inv.Quantity,inv.uPrice, inv.TotalPrice,inv.ItemId AS ID, i.Name AS ItemName FROM msSchedulerV3.dbo.Items as i INNER JOIN " +
                    "             msSchedulerV3.dbo.tbl_InvoiceDetails as inv ON i.Id = inv.ItemId and i.CompanyID = inv.CompanyID Where inv.RefId='" + InvoiceNo + "' and  inv.CompanyID =@CompanyID order by CAST(NULLIF(inv.LineNum,'') AS INT) asc ;";

                sSQL += @"select * from [msSchedulerV3].[dbo].[tbl_Company] Where CompanyID =@CompanyID;";

                sSQL += @"SELECT [CompanyID]
                            ,[QuoteDisclaimer]
                          ,[InvoiceDisclaimer]
                          ,[CreatedDateTime]
                      FROM [msSchedulerV3].[dbo].[tbl_DisclaimerSettings] where CompanyID =@CompanyID";

                DataSet dataSet = db.Get_DataSet(sSQL, companyID);

                string Disclaimer = "";
                DataTable dt_Disclaimer = dataSet.Tables[4];
                if (dt_Disclaimer.Rows.Count > 0)
                {
                    DataRow Rs = dt_Disclaimer.Rows[0];
                    if (doctype == "Invoice")
                    {
                        Disclaimer = Rs["InvoiceDisclaimer"].ToString();
                    }
                    else
                    {
                        Disclaimer = Rs["QuoteDisclaimer"].ToString();
                    }
                }

                DataTable dt_Company = new DataTable();
                dt_Company = dataSet.Tables[3];
                string companyLogo = "";
                if (dt_Company.Rows.Count > 0)
                {
                    DataRow Rs = dt_Company.Rows[0];
                    CompanyAddress = Rs["Address"].ToString();
                    CompanyCity = Rs["City"].ToString();
                    CompanyState = Rs["State"].ToString();
                    CompanyZipCode = Rs["ZipCode"].ToString();
                    CompanyPhone = Rs["Phone"].ToString();
                    CompanyEmail = Rs["Email"].ToString();
                    CompanyWebsite = Rs["Website"].ToString();
                    CompanyFacebook = Rs["Facebook"].ToString();
                    CompanyTwitter = Rs["Twitter"].ToString();
                    CompanyLogoFile = Rs["LogoFile"].ToString();
                    CompanyFullName = Rs["CompanyName"].ToString();
                    companyLogo = Rs["LogoFile"].ToString();
                    companyFax = Rs["Fax"].ToString();
                }

                LogoPath = HttpContext.Current.Server.MapPath("~/CompanyLogo/" + companyLogo);
                if (!File.Exists(LogoPath))
                {
                    LogoPath = "crv_sched/img/logo/central.png";
                    LogoPath = HttpContext.Current.Server.MapPath("~/crv_sched/img/logo/central.png");
                }
                string SignaturePath = HttpContext.Current.Server.MapPath("~/images/Signature.PNG");
                if (!File.Exists(SignaturePath))
                {
                    SignaturePath = HttpContext.Current.Server.MapPath("~/images/Signature.PNG");
                }

                if (doctype == "Invoice")
                {
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/AsaInvocie.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                else
                {
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/AsaQuote.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                }

                body = body.Replace("{Disclaimer}", Disclaimer);
                // For Company
                body = body.Replace("{CompanyName}", CompanyFullName);
                body = body.Replace("{companyAddress}", CompanyAddress);
                body = body.Replace("{companyPhone}", CompanyPhone);
                body = body.Replace("{companyFax}", companyFax);
                body = body.Replace("{companyEmail}", CompanyEmail);
                body = body.Replace("{companyEmail}", CompanyEmail);
                body = body.Replace("{Signatureimage}", SignaturePath);

                DataTable customer = dataSet.Tables[0];
                int count = 0;
                if (customer.Rows.Count > 0)
                {
                    DataRow dr = customer.Rows[0];

                    body = body.Replace("{customerName}", dr["FullName"].ToString());
                    body = body.Replace("{address}", dr["Address1"].ToString());
                    body = body.Replace("{phone}", dr["Phone"].ToString());
                    body = body.Replace("{email}", dr["Email"].ToString());

                    body = body.Replace("{Fax}", "");

                    body = body.Replace("{image}", LogoPath);

                    DataTable inv = dataSet.Tables[1];

                    if (inv.Rows.Count > 0)
                    {
                        DataRow dr2 = inv.Rows[0];

                        body = body.Replace("{IssueDate}", dr2["IssueDate"].ToString());
                        body = body.Replace("{InvoiceNo}", dr2["Number"].ToString());
                        body = body.Replace("{InvoiceTotal}", dr2["Total"].ToString());
                        body = body.Replace("{SubTotal}", dr2["Subtotal"].ToString());
                        body = body.Replace("{Discount}", dr2["Discount"].ToString());
                        body = body.Replace("{Total}", dr2["Total"].ToString());
                        body = body.Replace("{Tax}", dr2["Tax"].ToString());
                        body = body.Replace("{Deposit}", dr2["DepositAmount"].ToString());
                        body = body.Replace("{BalanceDue}", dr2["Due"].ToString());
                        body = body.Replace("{DocType}", doctype);
                        body = body.Replace("{offerisvalid}", dr2["ExpDate"].ToString());
                        body = body.Replace("{ReqDepo}", depoReqAmt.ToString());
                    }

                    MailTo = dr["Email"].ToString();
                    string itemDetails = "";
                    Int32 rowcount = 0;
                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        count = dataSet.Tables[2].Rows.Count;

                        DataTable dt = dataSet.Tables[2];
                        foreach (DataRow d in dt.Rows)
                        {
                            rowcount += 1;
                            itemDetails += "<tr>" +
                                    "<td width='5%' style='border:none;min-width:5%;max-width:5%;text-align:center;'>" + rowcount.ToString() + "</td>" +
                                    "<td width='10%' style='border:none;min-width:10%;max-width:10%;'>" + d["Location"] + "</td>" +
                                    "<td width='50%' style='border:none;min-width:50%;max-width:50%;text-align:left;padding-left: 4px;'>" + d["Description"] + "</td>" +
                                    "<td width='10%' style='border:none;min-width:10%;max-width:10%;text-align:center;'>" + d["Quantity"] + "</td>" +
                                     "<td width='10%' style='border:none;min-width:10%;max-width:10%;text-align:center;'>$" + d["uPrice"] + "</td>" +
                                    "<td width='10%' style='border:none;min-width:10%;max-width:10%;text-align:center;'>$" + d["TotalPrice"] + "</td>" +
                                    "</tr>";
                        }
                    }

                    body = body.Replace("{itemRow}", itemDetails);
                }
                //add wisetack Loan Info  into pdf start
                string paylink = "";

                if (!string.IsNullOrEmpty(paylink))
                {
                    body = body.Replace("[WisetackDisclimer]", "	*All financing is subject to credit approval. Your terms may vary. Payment options through Wisetack are provided by our lending partners. For example, a $1,200 purchase could cost $104.89 a month for 12 months, based on an 8.9% APR, or $400 a month for 3 months, based on a 0% APR. Offers range from 0-35.9% APR based on creditworthiness. No other financing charges or participation fees. See additional terms at http://wisetack.com/faqs.");
                }
                else
                {
                    body = body.Replace("[WisetackDisclimer]", "");
                }

                StringReader sr = new StringReader(body);

                Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 10f);

                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                MemoryStream memoryStream = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                pdfDoc.Open();

                htmlparser.Parse(sr);
                PdfContentByte cb = writer.DirectContent;
                BaseFont font = BaseFont.CreateFont();

                float buttonWidth = 150; // Width of the button
                float buttonHeight = 30; // Height of the button
                float verticalSpacing = 5; // Vertical spacing between buttons

                float initialYPosition = pdfDoc.PageSize.Height * 0.12f; // Initial position at 65% from the top
                float totalHeight = count * (buttonHeight + verticalSpacing); // Total height needed for all buttons
                int c = 0;
                if (!string.IsNullOrEmpty(paylink))
                {
                    c += 1;
                    Rectangle linkArea2 = new Rectangle(pdfDoc.PageSize.Width - buttonWidth - 30, initialYPosition - buttonHeight, pdfDoc.PageSize.Width - 30, initialYPosition);

                    PdfAction action2 = new PdfAction(paylink);

                    cb.SetColorFill(new BaseColor(7, 192, 202)); // Button color: #07c0ca
                    cb.Rectangle(linkArea2.Left, linkArea2.Bottom, linkArea2.Width, linkArea2.Height);
                    cb.Fill();
                    cb.SetColorFill(BaseColor.WHITE); // Text color: white
                    cb.BeginText();
                    cb.SetFontAndSize(font, 12);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "See Financing Options", linkArea2.Left + linkArea2.Width / 2, linkArea2.Bottom + linkArea2.Height / 2, 0);
                    cb.EndText();
                    PdfAnnotation linkAnnotation1 = PdfAnnotation.CreateLink(writer, linkArea2, PdfAnnotation.HIGHLIGHT_INVERT, action2);
                    writer.AddAnnotation(linkAnnotation1);
                }

                if (!string.IsNullOrEmpty(CSPaymentLink))
                {
                    c += 1;
                    //Rectangle linkArea = new Rectangle(pdfDoc.PageSize.Width - buttonWidth - 30, initialYPosition - totalHeight, pdfDoc.PageSize.Width - 30, initialYPosition - totalHeight + buttonHeight);

                    //PdfAction action = new PdfAction(CSPaymentLink);

                    //cb.SetColorFill(new BaseColor(0x50, 0x78, 0xB0));
                    //cb.Rectangle(linkArea.Left, linkArea.Bottom, linkArea.Width, linkArea.Height);
                    //cb.Fill();
                    //cb.SetColorFill(BaseColor.BLACK);
                    //cb.BeginText();
                    //cb.SetFontAndSize(font, 12);
                    //cb.ShowTextAligned(Element.ALIGN_CENTER, "Pay Now", linkArea.Left + linkArea.Width / 2, linkArea.Bottom + linkArea.Height / 2, 0);
                    //cb.EndText();

                    //PdfAnnotation linkAnnotation2 = PdfAnnotation.CreateLink(writer, linkArea, PdfAnnotation.HIGHLIGHT_INVERT, action);
                    //writer.AddAnnotation(linkAnnotation2);

                    // [1] create a Chunk with font and colors you want
                    var anchor = new Chunk("Click this link to pay now")
                    {
                        Font = new Font(
                            Font.FontFamily.HELVETICA, 15,
                            Font.NORMAL,
                            BaseColor.BLACK
                        )
                    };

                    // [2] set the anchor URL
                    anchor.SetAnchor(CSPaymentLink);

                    // [3] create a Paragraph with alignment, indentation, etc
                    Paragraph p = new Paragraph()
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        IndentationLeft = 5
                    };
                    p.SetLeading(1.1f, 1.1f);

                    // [4] add chunk to Paragraph
                    p.Add(anchor);

                    // [5] add Paragraph to Document
                    pdfDoc.Add(p);
                }

                if (c == 1)
                {
                    totalHeight -= buttonHeight;
                    initialYPosition = pdfDoc.PageSize.Height * 0.18f - totalHeight;
                }
                else
                {
                    totalHeight -= (count - 2) * (buttonHeight + verticalSpacing);
                    initialYPosition = pdfDoc.PageSize.Height * 0.18f - totalHeight;
                }

                pdfDoc.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                return bytes;
            }
            catch (Exception ex )
            {
                throw ex;
            }
        }      
       
        public string ProcessInvoiceForEmail(
        string CompanyID, string CustomerID, string EmailType, string subject, string body, string recepientToEmail, string recepientCCEmail, string recepientBCCEmail, List<EmailContent> emailContents, string UserId, string currentPdfType, string invoiceNo, bool isSendPaymentLink)
        {
            string returnMessage = "Failed";

            try
            {
                string CSPaymentLink = "";
                byte[] invoicepdf = null;
                string pdf_FileName = "_Proposal.pdf";

                EmailProcessor emailProcessor = new EmailProcessor();
                XPayLinkProcessor xPayLinkProcessor = new XPayLinkProcessor();

                //if (!isSendWisetackPaymentLink)
                //{
                //    emailProcessor.GetWiseTackPaymentLink(invoiceNo, customerID);
                //}
                //else
                //{
                //    HttpContext.Current.Session["LoanInfo"] = null;
                //}

                // code by Mohsin
                decimal reqDAmt = 0;
                if (isSendPaymentLink)
                {
                    Database db = new Database();
                    string amount = db.ExecuteScalar("select (isNULL(Total,0)-isNULL(AmountCollect,0))as Amount from msSchedulerV3.dbo.tbl_Invoice where CustomerId='" + CustomerID + "' and CompnyID='" + CompanyID + "' and ID='" + invoiceNo + "'");

                    
                    string reqDepoAmount = db.ExecuteScalar("select ((isNULL(Total,0)-isNULL(AmountCollect,0)) * isNULL(ReqDepoPercent,0))/100 from msSchedulerV3.dbo.tbl_Invoice where CustomerId='" + CustomerID + "' and CompnyID='" + CompanyID + "' and ID='" + invoiceNo + "'");

                    string reqDepoAmtDollarAmt = db.ExecuteScalar("select isNULL(RequestedDepoAmt,0)reqDep from  msSchedulerV3.dbo.tbl_invoice where id='" + invoiceNo + "' and CompnyID='" + CompanyID + "'");

                    if (reqDepoAmount == "0" || reqDepoAmount == "0.000000") reqDepoAmount = reqDepoAmtDollarAmt;

                    reqDAmt = Convert.ToDecimal(reqDepoAmount);

                    if (reqDAmt > 0) amount = reqDepoAmount;
                   // string amount = db.ExecuteScalar($"SELECT (Total - AmountCollect) AS Amount FROM msSchedulerV3.dbo.tbl_Invoice WHERE CustomerId = {CustomerID} AND CompnyID = '{CompanyID}' AND ID = '{invoiceNo}'");
                    string customerName = db.ExecuteScalar($"SELECT CONCAT(FirstName, ' ', LastName) AS CustomerName FROM msSchedulerV3.dbo.tbl_Customer WHERE CustomerId = {CustomerID} AND CompanyId = '{CompanyID}'");

                    CSPaymentLink = xPayLinkProcessor.GetXpayLink(CompanyID, CustomerID, invoiceNo, customerName, recepientToEmail, amount);
                }
                //end code Mohsin

                // Select PDF Generation Method
                switch (currentPdfType)
                {

                    case "Invoice":
                        pdf_FileName = "_Invoice.pdf";
                        invoicepdf = emailProcessor.InvoicePdf(CustomerID, invoiceNo, "Invoice", CSPaymentLink, CompanyID, reqDAmt);
                        break;
                    case "Estimate":
                        invoicepdf = emailProcessor.InvoicePdf(CustomerID, invoiceNo, "Estimate", CSPaymentLink, CompanyID, reqDAmt);
                        pdf_FileName = "_Estimate.pdf";
                        break;

                }
                // Save PDF
                string invoiceCreated = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff");
                string relativePath = $"~/EmailHistoryContent/{CustomerID}/";
                string fullFolderPath = HttpContext.Current.Server.MapPath(relativePath);

                if (!Directory.Exists(fullFolderPath))
                {
                    Directory.CreateDirectory(fullFolderPath);
                }

                string fullPdfPath = Path.Combine(fullFolderPath, invoiceCreated + pdf_FileName);
                File.WriteAllBytes(fullPdfPath, invoicepdf);
                EmailContent emailContent = new EmailContent
                {
                    FileName = "pdf_FileName",
                    FileType = "",
                    FileContent = new byte[0],
                    FileUrl = relativePath + invoiceCreated + pdf_FileName,
                };
                emailContents.Add(emailContent);


                // Send email
                returnMessage = emailProcessor.SendHtmlFormattedEmail(
                    CompanyID, CustomerID, "Invoice Email",
                    subject, body,
                    recepientToEmail, recepientCCEmail, recepientBCCEmail,
                    emailContents, UserId
                );
            }
            catch (Exception ex)
            {
                returnMessage = "Error: " + ex.Message;
            }

            return returnMessage;
        }
       
        public CustCommunication GetAutoFillValuesForEmail(string companyId)
        {
            try
            {
                string sql = @"
            SELECT 
                CompanyID,WisetackFooterMsg, ReviewPostUrl, AppointmentRescheduleSubject, AppointmentRescheduleBody,
                StandardMailSubject, StandardMailBody, ProposalMailSubject, ProposalMailBody,
                InvoiceMailSubject, InvoiceMailBody, SMSAck, SMSAckText, SMSConfirm, SMSConfirmText,
                EmaiAck, EmailAckText, EmailConfirm, EmailConfirmText, EmailFrom, EmailCC, EmailBCC, IsSendPrequal
            FROM [msSchedulerV3].[dbo].[tbl_CustCommunication]
            WHERE CompanyID = @CompanyID";

                Database db = new Database();
                DataSet ds = db.Get_DataSet(sql, companyId);
                DataTable dt = ds.Tables[0];

                CustCommunication comm = new CustCommunication();

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    comm.CompanyID = dr["CompanyID"].ToString();
                    comm.SMSAck = dr["SMSAck"] != DBNull.Value && Convert.ToBoolean(dr["SMSAck"]);
                    comm.SMSAckText = dr["SMSAckText"].ToString();
                    comm.SMSConfirm = dr["SMSConfirm"] != DBNull.Value && Convert.ToBoolean(dr["SMSConfirm"]);
                    comm.SMSConfirmText = dr["SMSConfirmText"].ToString();
                    comm.EmaiAck = dr["EmaiAck"] != DBNull.Value && Convert.ToBoolean(dr["EmaiAck"]);
                    comm.EmailAckText = dr["EmailAckText"].ToString();
                    comm.EmailConfirm = dr["EmailConfirm"] != DBNull.Value && Convert.ToBoolean(dr["EmailConfirm"]);
                    comm.EmailConfirmText = dr["EmailConfirmText"].ToString();
                    comm.EmailFrom = dr["EmailFrom"].ToString();
                    comm.EmailCC = dr["EmailCC"].ToString();
                    comm.EmailBCC = dr["EmailBCC"].ToString();
                    comm.ProposalMailSubject = dr["ProposalMailSubject"].ToString();
                    comm.ProposalMailBody = dr["ProposalMailBody"].ToString();
                    comm.StandardMailSubject = dr["StandardMailSubject"].ToString();
                    comm.StandardMailBody = dr["StandardMailBody"].ToString();
                    comm.InvoiceMailSubject = dr["InvoiceMailSubject"].ToString();
                    comm.InvoiceMailBody = dr["InvoiceMailBody"].ToString();
                    comm.ReviewPostUrl = dr["ReviewPostUrl"].ToString();
                    comm.WisetackFooterMsg = dr["WisetackFooterMsg"].ToString();
                    comm.IsSendPrequal = dr["IsSendPrequal"] != DBNull.Value && Convert.ToBoolean(dr["IsSendPrequal"]);
                    comm.AppointmentRescheduleBody = dr["AppointmentRescheduleBody"].ToString();
                    comm.AppointmentRescheduleSubject = dr["AppointmentRescheduleSubject"].ToString();
                }


                return comm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class EmailCommunication
    {
        public string EmailTo { get; set; }
        public string StandardMailSubject { get; set; }
        public string StandardMailBody { get; set; }
        public string EmailBCC { get; set; }
        public string EmailCC { get; set; }
        public string ProposalMailSubject { get; set; }
        public string ProposalMailBody { get; set; }
        public string EmailConfirmText { get; set; }
        public string SMSConfirmText { get; set; }
        public string EmailAckText { get; set; }
        public string SMSAckText { get; set; }

        public string InvoiceMailSubject { get; set; }
        public string InvoiceMailBody { get; set; }
        public string AttachmentsName { get; set; }
        public string EmailType { get; set; }

        public List<EmailContent> EmailContents { get; set; }

    }
    public class EmailContent
    {
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; }
    }
    public class CustCommunication
    {
        public string CompanyID { get; set; } = string.Empty;
        public bool SMSAck { get; set; } = false;
        public string SMSAckText { get; set; } = string.Empty;
        public bool SMSConfirm { get; set; } = false;
        public string SMSConfirmText { get; set; } = string.Empty;
        public bool EmaiAck { get; set; } = false;
        public string EmailAckText { get; set; } = string.Empty;
        public bool EmailConfirm { get; set; } = false;
        public string EmailConfirmText { get; set; } = string.Empty;
        public string EmailFrom { get; set; } = string.Empty;
        public string EmailCC { get; set; } = string.Empty;
        public string EmailBCC { get; set; } = string.Empty;
        public string ProposalMailSubject { get; set; }
        public string ProposalMailBody { get; set; }
        public string StandardMailSubject { get; set; }
        public string StandardMailBody { get; set; }
        public string InvoiceMailSubject { get; set; }
        public string InvoiceMailBody { get; set; }
        public string AppointmentRescheduleSubject { get; set; } 
        public string AppointmentRescheduleBody { get; set; } 
        public string ReviewPostUrl { get; set; }
        public string WisetackFooterMsg { get; set; }
        public bool IsSendPrequal { get; set; } = false;
    }
}