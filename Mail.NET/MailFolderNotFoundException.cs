namespace Mail.NET;

[Serializable]
public class MailFolderNotFoundException : Exception
{
    public MailFolderNotFoundException(string foldername) : base($"Could not find any folder under the name of \"{foldername}\"") { }
    protected MailFolderNotFoundException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
