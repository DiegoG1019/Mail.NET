namespace Mail.NET;

public abstract class MailAttachment
{
    public string Name { get; protected init; }
    public string MimeType { get; protected init; }
    public abstract Stream Data { get; }

    protected MailAttachment(string name, string mimeType)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("name must not be null or empty", nameof(name));
        Name = name;

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException("mimeType must not be null or empty", nameof(name));
        MimeType = mimeType;
    }
}
