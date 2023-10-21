using MailKit;
using MimeKit;

namespace Mail.NET.MailKit;

public sealed class ImapMessage : MailMessage
{
    public IMailFolder Folder { get; }

    public UniqueId Uid { get; }

    public ImapMessage(IMailFolder folder, UniqueId msg, IEnumerable<string> senders, string summary, string body, MailBodyType bodyType, bool isRead, DateTimeOffset sentDate, IEnumerable<string>? recipients = null, IEnumerable<string>? ccs = null, IEnumerable<MailAttachment>? attachments = null) : base(senders, summary, body, bodyType, isRead, sentDate, recipients, ccs, attachments)
    {
        Folder = folder;
        Uid = msg;
    }
    
    public static MailMessage FromMimeMessage(IMailFolder folder, MimeMessage message, IMessageSummary summary)
    {
        return new ImapMessage(
                folder,
                summary.UniqueId,
                message.From.Mailboxes.Select(x => x.Address).Concat(message.ResentFrom.Mailboxes.Select(x => x.Address)),
                message.Subject,
                message.HtmlBody ?? message.TextBody ?? "",
                message.HtmlBody is not null ? MailBodyType.Html : message.TextBody is not null ? MailBodyType.Plain : MailBodyType.None,
                summary.Flags.HasValue && summary.Flags.Value.HasFlag(MessageFlags.Seen),
                message.Date,
                message.To.Mailboxes.Select(x => x.Address),
                message.Cc.Mailboxes.Select(x => x.Address),
                message.Attachments.Select(x => new MailKitAttachment(message, x))
            );
    }

    public override async ValueTask<bool> MarkAsRead(CancellationToken ct)
    {
        try
        {
            await Folder.AddFlagsAsync(Uid, MessageFlags.Seen, true, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}