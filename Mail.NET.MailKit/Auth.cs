using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using MailKit.Security;

namespace Mail.NET.MailKit;

public readonly struct Auth : IEquatable<Auth>
{
    public string? Username { get; }
    public string? Password { get; }
    public SaslMechanism? Mechanism { get; }
    private readonly bool notDefault;

    public bool GetMechanism([NotNullWhen(true)] out SaslMechanism? mechanism)
    {
        if (notDefault is false) throw new InvalidOperationException("Cannot query a default-initialized Auth object");
        if (Mechanism is not null)
        {
            mechanism = Mechanism;
            return true;
        }

        mechanism = null;
        return false;
    }

    public bool GetCredentials([NotNullWhen(true)] out ICredentials? credentials)
    {
        if (notDefault is false) throw new InvalidOperationException("Cannot query a default-initialized Auth object");
        if (Username is not null)
        {
            Debug.Assert(Password is not null);
            credentials = new NetworkCredential(Username, Password);
            return true;
        }

        credentials = null;
        return false;
    }

    public Auth(string username, string password) : this()
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(password);
        Username = username;
        Password = password;
        notDefault = true;
    }

    public Auth(SaslMechanism auth) : this()
    {
        ArgumentNullException.ThrowIfNull(auth);
        Mechanism = auth;
        notDefault = true;
    }

    public bool Equals(Auth other)
    {
        if (other.notDefault != notDefault)
            return false; // They're already not equal
        if (other.notDefault is false)
            return true; // Both's notDefault status is the same, but then if one is default, then both are, and thus no more checks are necessary

        if (Username is not null)
        {
            if (other.Username is null) return false;
            Debug.Assert(Password is not null);
            Debug.Assert(other.Password is not null);
            return Username == other.Username && Password == other.Password;
        }

        return ReferenceEquals(Mechanism, other.Mechanism);
    }
}
