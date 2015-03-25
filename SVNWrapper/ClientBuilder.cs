namespace SVNWrapper
{
  public static class ClientBuilder
  {
    public enum ClientTypes
    {
      TortoiseSVN,
    }

    public static IVersionControlSystem GetClient(ClientTypes type)
    {
      switch (type)
      {
        case ClientTypes.TortoiseSVN:
          return TortoiseSVNWrapper.Instance;
      }

      return null;
    }
  }
}
