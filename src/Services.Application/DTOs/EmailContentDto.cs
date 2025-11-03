namespace Services.Application.DTOs;

public class EmailContentDto
{
    public byte[]? FileContent { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}

public class SendEmailDto
{
    public string CompanyID { get; set; } = string.Empty;
    public string CustomerID { get; set; } = string.Empty;
    public string EmailType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string RecipientToEmail { get; set; } = string.Empty;
    public string RecipientCCEmail { get; set; } = string.Empty;
    public string RecipientBCCEmail { get; set; } = string.Empty;
    public List<EmailContentDto> EmailContents { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
    public string CurrentPdfType { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public bool IsSendPaymentLink { get; set; }
}

public class AutoFillEmailDto
{
    public string EmailTo { get; set; } = string.Empty;
    public string StandardMailSubject { get; set; } = string.Empty;
    public string StandardMailBody { get; set; } = string.Empty;
    public string EmailBCC { get; set; } = string.Empty;
    public string EmailCC { get; set; } = string.Empty;
    public string ProposalMailSubject { get; set; } = string.Empty;
    public string ProposalMailBody { get; set; } = string.Empty;
    public string EmailConfirmText { get; set; } = string.Empty;
    public string SMSConfirmText { get; set; } = string.Empty;
    public string EmailAckText { get; set; } = string.Empty;
    public string SMSAckText { get; set; } = string.Empty;
    public string InvoiceMailSubject { get; set; } = string.Empty;
    public string InvoiceMailBody { get; set; } = string.Empty;
    public string AttachmentsName { get; set; } = string.Empty;
    public string EmailType { get; set; } = string.Empty;
    public List<EmailContentDto> EmailContents { get; set; } = new();
}

public class StringResponseDto
{
    public string Status { get; set; } = "success";
    public string Response { get; set; } = string.Empty;
}

public class InvoiceNoResponseDto
{
    public string InvoiceNo { get; set; } = string.Empty;
}

public class PaymentLinkResponseDto
{
    public string XPayLink { get; set; } = string.Empty;
}

