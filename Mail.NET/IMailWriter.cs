namespace Mail.NET;

public interface IMailWriter
{
    /// <summary>
    /// Asynchronously sends <paramref name="draft"/>
    /// </summary>
    public ValueTask SendMessage(MailDraft draft, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously verifies that <paramref name="address"/> exists
    /// </summary>
    /// <param name="address">The address to verify</param>
    public ValueTask<bool> VerifyAddress(string address, CancellationToken ct = default);
}