using MailKit.Search;
using MimeKit;

namespace Mail.NET.MailKit;
public static class MailKitExtensions
{
    public static IEnumerable<InternetAddress> ToInternetAddressList(IEnumerable<string> addresses)
    {
        foreach (var address in addresses)
            yield return InternetAddress.Parse(address);
    }

    public static void FillWith(this InternetAddressList ial, IEnumerable<string> addresses)
    {
        foreach (var address in addresses)
            ial.Add(InternetAddress.Parse(address));
    }

    public static AttachmentCollection ToMailAttachments(this IEnumerable<MailAttachment> attachments, AttachmentCollection? collection = null)
    {
        collection ??= new();
        foreach (var atch in attachments)
            collection.Add(atch.Name, atch.Data, ContentType.Parse(atch.MimeType));

        return collection;
    }

    public static SearchQuery ToMailKitSearchQuery(this MailQuery query)
    {
        var q = new SearchQuery();

        if (query.Start is DateTimeOffset st)
            q = q.And(SearchQuery.SentSince(st.UtcDateTime));

        if (query.End is DateTimeOffset nd)
            q = q.And(SearchQuery.SentBefore(nd.UtcDateTime));

        foreach (var bq in query.BodyContains)
            q = q.And(SearchQuery.BodyContains(bq));

        foreach (var sq in query.SubjectContains)
            q = q.And(SearchQuery.SubjectContains(sq));

        if (query.HasSender.Count > 0)
        {
            var qq = new SearchQuery();
            foreach (var fc in query.HasSender)
                qq = qq.Or(SearchQuery.FromContains(fc));
            q = q.And(qq);
        }

        if (query.Read is bool r)
            q = q.And(r ? SearchQuery.Seen : SearchQuery.NotSeen);

        return q;
    }
}
