using System;

using SVNWrapper;

namespace SVNTester
{
  public class Program
  {
    public static void Main(string[] args)
    {
      if (args.Length < 2)
      {
        Console.WriteLine("Parameters required");
        Console.ReadLine();
        return;
      }

      IVersionControlSystem system = ClientBuilder.GetClient(ClientBuilder.ClientTypes.TortoiseSVN);

      switch (args[1])
      {
        case "status":
          Console.WriteLine(args[0]);
          Console.WriteLine("Modified locally: " + system.IsModified(args[0]));
          break;
        case "update":
          Console.WriteLine("Updating:");
          Console.WriteLine(args[0]);
          Console.WriteLine("Synchronous operation is complete. Result: " + system.UpdateFile(args[0], true));
          break;
        case "revert":
          Console.WriteLine("Reverting:");
          Console.WriteLine(args[0]);
          Console.WriteLine("Synchronous operation is complete. Result: " + system.RevertFile(args[0], true));
          break;
        case "commit":
          Console.WriteLine("Enter a message:");
          string message = Console.ReadLine();
          Console.WriteLine("Commiting:");
          Console.WriteLine(args[0]);
          Console.WriteLine("Synchronous operation is complete. Result: " + system.CommitFile(args[0], message, true));
          break;
        case "logs":
          Console.WriteLine("Showing logs for:");
          Console.WriteLine(args[0]);
          Console.WriteLine("Synchronous operation is complete. Result: " + system.ShowLogs(args[0], true));
          break;
      }

      Console.WriteLine("Press <Enter> to exit.");
      Console.ReadLine();
    }
  }
}
