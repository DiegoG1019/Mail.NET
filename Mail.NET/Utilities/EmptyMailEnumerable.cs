using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail.NET.Utilities;

public sealed class EmptyMailEnumerable : IAsyncEnumerable<MailMessage>, IAsyncEnumerator<MailMessage>, IEnumerable<MailMessage>, IEnumerator<MailMessage>
{
    public IAsyncEnumerator<MailMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => this;

    public ValueTask<bool> MoveNextAsync()
        => ValueTask.FromResult(false);

    public MailMessage Current => null!;

    object IEnumerator.Current => null!;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private EmptyMailEnumerable() { }

    public static EmptyMailEnumerable Instance { get; } = new();

    public IEnumerator<MailMessage> GetEnumerator()
        => ((IEnumerable<MailMessage>)Array.Empty<MailMessage>()).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Array.Empty<MailMessage>().GetEnumerator();

    public bool MoveNext()
        => false;

    public void Reset() { }

    public void Dispose() { }
}