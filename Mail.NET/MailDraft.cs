namespace Mail.NET;

public class MailDraft
{
    public MailMessage? InReplyTo { get; set; }
    public List<string> Recipients { get; } = new();
    public List<string>? CCs { get; set; }
    public List<string>? BCCs { get; set; }
    public string? Summary { get; set; }
    public string? Body { get; set; }
    public string? From { get; set; }
    public MailBodyType BodyType { get; set; }
    public MessageImportance Importance { get; set; }
    public List<MailAttachment>? Attachments { get; set; }
}
