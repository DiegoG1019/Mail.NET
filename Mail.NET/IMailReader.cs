namespace Mail.NET;

public interface IMailReader
{
    public readonly record struct GetFolderResults(int? Count, IAsyncEnumerable<MailMessage> Messages);

    /// <summary>
    /// Asynchronously enumerates the requested folder. If null, defaults to Inbox.
    /// </summary>
    /// <param name="folder">The name of the folder</param>
    /// <returns>An AsyncEnumerable that goes through the messages found</returns>
    public Task<GetFolderResults> GetMessages(string? folder = null, MailQuery? query = null, CancellationToken ct = default);
}
