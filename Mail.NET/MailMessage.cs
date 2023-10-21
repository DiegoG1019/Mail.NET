using System.Collections.Immutable;

namespace Mail.NET;

public abstract class MailMessage
{
    public string? OriginalSender => Senders[0];
    public string? LastSender => Senders[^1];

    /// <summary>
    /// The list of senders for this message, ordered first-to-last, starting with the original sender and ending with the last resender
    /// </summary>
    public ImmutableArray<string> Senders { get; }
    public ImmutableArray<string> Recipients { get; }
    public ImmutableArray<string> CCs { get; }
    public string Summary { get; }
    public string Body { get; }
    public MailBodyType BodyType { get; }
    public ImmutableArray<MailAttachment> Attachments { get; }
    public bool IsRead { get; }
    public DateTimeOffset SentDate { get; }

    public MailMessage(
        IEnumerable<string> senders,
        string summary,
        string body,
        MailBodyType bodyType,
        bool isRead,
        DateTimeOffset sentDate,
        IEnumerable<string>? recipients = null,
        IEnumerable<string>? ccs = null,
        IEnumerable<MailAttachment>? attachments = null
    )
    {
        IsRead = isRead;
        SentDate = sentDate;
        Senders = senders?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(senders));
        Recipients = GetArray(recipients);
        CCs = GetArray(ccs);
        Attachments = GetArray(attachments);
        Summary = summary ?? throw new ArgumentNullException(nameof(summary));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        BodyType = bodyType;
    }

    private static ImmutableArray<T> GetArray<T>(IEnumerable<T>? values)
        => values is not null && values.Any() ? values.ToImmutableArray() : ImmutableArray<T>.Empty;

    public abstract ValueTask<bool> MarkAsRead(CancellationToken ct = default);
}
