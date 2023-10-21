using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Mail.NET.MailKit;

public class MailKitSmtpWriter : IMailWriter, IDisposable
{
    public SmtpClient Client { get; }
    public Auth Authentication { get; private set; }
    public string HostUrl { get; private set; }
    public ushort HostPort { get; private set; }
    public bool UsingSSL { get; private set; }

    public MailKitSmtpWriter(string url, ushort port, Auth auth, bool useSsl)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("HostUrl must not be null or empty", nameof(url));
        ArgumentNullException.ThrowIfNull(auth);

        Authentication = auth;
        HostUrl = url;
        HostPort = port;
        UsingSSL = useSsl;

        Client = new SmtpClient();
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

    public async ValueTask SendMessage(MailDraft draft, CancellationToken ct = default)
    {
        await EnsureAuth();

        var bb = new BodyBuilder();
        if (draft.BodyType is MailBodyType.Plain)
            bb.HtmlBody = draft.Body;
        else if (draft.BodyType is MailBodyType.Html)
            bb.TextBody = draft.Body;

        draft.Attachments?.ToMailAttachments(bb.Attachments);

        var msg = new MimeMessage(
            draft.From is not null ? new InternetAddress[] { InternetAddress.Parse(draft.From) } : Array.Empty<InternetAddress>(),
            MailKitExtensions.ToInternetAddressList(draft.Recipients),
            draft.Summary,
            bb.ToMessageBody()
        )
        {
            Importance = (MimeKit.MessageImportance)draft.Importance,
        };

        if (draft.CCs is not null)
            msg.Cc.FillWith(draft.CCs);

        if (draft.BCCs is not null)
            msg.Bcc.FillWith(draft.BCCs);

        if (draft.InReplyTo is ImapMessage imapmsg)
            msg.InReplyTo = imapmsg.Uid.ToString();
        else if (draft.InReplyTo is not null)
            throw new NotSupportedException("MailMessage objects that are not of type ImapMessage are not supported as Reply candidates");

        await Client.SendAsync(msg, ct);
    }

    public async ValueTask<bool> VerifyAddress(string address, CancellationToken ct = default)
    {
        await EnsureAuth();
        try
        {
            await Client.VerifyAsync(address, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Client.Dispose();
    }
}
