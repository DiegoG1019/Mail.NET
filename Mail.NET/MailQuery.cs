namespace Mail.NET;

public sealed class MailQuery
{
    private readonly HashSet<string> bodyc = new();
    private readonly HashSet<string> hass = new();
    private readonly HashSet<string> summc = new();

    public MailBodyType? BodyType { get; private set; }
    public DateTimeOffset? Start { get; private set; }
    public DateTimeOffset? End { get; private set; }
    public IReadOnlyCollection<string> BodyContains => bodyc;
    public IReadOnlyCollection<string> HasSender => hass;
    public IReadOnlyCollection<string> SubjectContains => summc;
    public bool? Read { get; private set; }

    public MailQuery WithReadMailsOnly()
    {
        Read = true;
        return this;
    }

    public MailQuery WithNotReadMailsOnly()
    {
        Read = false;
        return this;
    }

    /// <summary>
    /// Specifies that the query should not care whether the mail has been read or not
    /// </summary>
    /// <returns></returns>
    public MailQuery WithReadMailIndifference()
    {
        Read = null;
        return this;
    }

    public MailQuery WithStartDate(DateTimeOffset start)
    {
        Start = start;
        return this;
    }

    public MailQuery WithEndDate(DateTimeOffset end)
    {
        End = end;
        return this;
    }

    public MailQuery SetBodyTypeQuery(MailBodyType bodyType)
    {
        BodyType = bodyType;
        return this;
    }

    public MailQuery RemoveSubjectContainsQueries(params string[] queries)
    {
        foreach (var query in queries)
            summc.Remove(query);
        return this;
    }

    public MailQuery AddSubjectContainsQueries(params string[] queries)
    {
        foreach (var query in queries)
            summc.Add(query);
        return this;
    }

    public MailQuery RemoveSubjectContainsQueries(IEnumerable<string> queries)
    {
        foreach (var query in queries)
            summc.Remove(query);
        return this;
    }

    public MailQuery AddSubjectContainsQueries(IEnumerable<string> queries)
    {
        foreach (var query in queries)
            summc.Add(query);
        return this;
    }

    public MailQuery ClearSubjectContainsQueries()
    {
        summc.Clear();
        return this;
    }

    public MailQuery RemoveHasSenderQueries(params string[] queries)
    {
        foreach (var query in queries)
            hass.Remove(query);
        return this;
    }

    public MailQuery AddHasSenderQueries(params string[] queries)
    {
        foreach (var query in queries)
            hass.Add(query);
        return this;
    }

    public MailQuery RemoveHasSenderQueries(IEnumerable<string> queries)
    {
        foreach (var query in queries)
            hass.Remove(query);
        return this;
    }

    public MailQuery AddHasSenderQueries(IEnumerable<string> queries)
    {
        foreach (var query in queries)
            hass.Add(query);
        return this;
    }

    public MailQuery ClearHasSenderQueries()
    {
        hass.Clear();
        return this;
    }

    public MailQuery RemoveBodyContainsQueries(params string[] queries)
    {
        foreach (var query in queries)
            bodyc.Remove(query);
        return this;
    }

    public MailQuery AddBodyContainsQueries(params string[] queries)
    {
        foreach (var query in queries)
            bodyc.Add(query);
        return this;
    }

    public MailQuery RemoveBodyContainsQueries(IEnumerable<string> queries)
    {
        foreach (var query in queries)
            bodyc.Remove(query);
        return this;
    }

    public MailQuery AddBodyContainsQueries(IEnumerable<string> queries)
    {
        foreach (var query in queries)
            bodyc.Add(query);
        return this;
    }

    public MailQuery ClearBodyContainsQueries()
    {
        bodyc.Clear();
        return this;
    }

    public MailQuery() { }
}
