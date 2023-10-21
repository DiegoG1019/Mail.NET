using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Mail.NET.Utilities;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace Mail.NET.MailKit;

public class MailKitImapReader : IMailReader, IDisposable
{
    public ImapClient Client { get; }
    public Auth Authentication { get; private set; }
    public string HostUrl { get; private set; }
    public ushort HostPort { get; private set; }
    public bool UsingSSL { get; private set; }

    public MailKitImapReader(string url, ushort port, Auth auth, bool useSsl)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("HostUrl must not be null or empty", nameof(url));
        ArgumentNullException.ThrowIfNull(auth);

        Authentication = auth;
        HostUrl = url;
        HostPort = port;
        UsingSSL = useSsl;

        Client = new ImapClient();
    }

    protected virtual async ValueTask EnsureAuth()
    {
        if (Client.IsConnected is false)
            await Client.ConnectAsync(HostUrl, HostPort, UsingSSL);
        if (Client.IsAuthenticated is false)
        {
            if (Authentication.GetCredentials(out var creds))
            {
                await Client.AuthenticateAsync(creds);
                return;
            }
            else if (Authentication.GetMechanism(out var mechanism))
            {
                await Client.AuthenticateAsync(mechanism);
                return;
            }
            throw new InvalidOperationException("Invalid Auth object");
        }
    }

    public virtual async ValueTask SetAccessData(string? url = null, ushort? port = null, Auth? auth = null, bool useSsl = true)
    {
        bool changed;
        changed = url is not null && url != HostUrl
            || port is ushort hport && hport != HostPort
            || auth is Auth a && a.Equals(Authentication) is false
            || useSsl != UsingSSL;

        if (changed is false) return;

        Authentication = auth ?? Authentication;
        HostUrl = url ?? HostUrl;
        HostPort = port ?? HostPort;
        UsingSSL = useSsl;

        if (Client.IsConnected)
            await Client.DisconnectAsync(true);
    }

    private static readonly IFetchRequest SummaryFetchRequest = new FetchRequest(MessageSummaryItems.Flags);
    public async Task<IMailReader.GetFolderResults> GetMessages(string? folder = null, MailQuery? query = null, CancellationToken ct = default)
    {
        await EnsureAuth();
        var f = folder is not null ? await Client.GetFolderAsync(folder, ct) : Client.Inbox;
        await f.OpenAsync(FolderAccess.ReadWrite, ct);
        var qq = query is null ? SearchQuery.All : query.ToMailKitSearchQuery();

        var search = await f.SearchAsync(SearchOptions.All, qq, ct);
        int count = search.Count;

        var summaries = await f.FetchAsync(search.UniqueIds, SummaryFetchRequest, ct);

        return count > 0 ? new(count, EnumerateMessages(f, summaries, ct)) : new(0, EmptyMailEnumerable.Instance);
    }

    private static async IAsyncEnumerable<MailMessage> EnumerateMessages(IMailFolder folder, IEnumerable<IMessageSummary> results, [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var summ in results)
            yield return ImapMessage.FromMimeMessage(folder, await folder.GetMessageAsync(summ.UniqueId, ct), summ);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Client.Dispose();
    }
}
