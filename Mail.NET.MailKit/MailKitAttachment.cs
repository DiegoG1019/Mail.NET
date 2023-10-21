using MimeKit;

namespace Mail.NET.MailKit;

public class MailKitAttachment : MailAttachment
{
    public MimeMessage Message { get; }
    public MimePart Part { get; }
    public IMimeContent Content { get; }
    public override Stream Data => Content.Stream;

    public MailKitAttachment(MimeMessage message, MimePart part)
        : base(string.IsNullOrWhiteSpace(part.FileName) ? $"unnamed-{Guid.NewGuid()}" : part.FileName, part.ContentType.ToString())
    {
        if (part.IsAttachment is false)
            throw new ArgumentException("part is not an attachment");
        Message = message;
        Part = part;
        Content = part.Content;
    }

    public MailKitAttachment(MimeMessage message, MimeEntity entity)
        : this(message, (MimePart)entity) { }
}