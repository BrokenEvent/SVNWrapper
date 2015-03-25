using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32;

namespace SVNWrapper
{
  public class TortoiseSVNWrapper : IVersionControlSystem
  {
    [DllImport("Advapi32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern int RegOpenKeyEx(uint hKey, string lpSubKey, uint olOptions, uint samDesired, ref uint phkResult);

    [DllImport("Advapi32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern int RegCloseKey(uint hKey);

    [DllImport("Advapi32.dll", CallingConvention = CallingConvention.Winapi)]
    private static extern int RegQueryValueEx(uint hKey, string lpValueName, IntPtr lpReserved, IntPtr lpType, IntPtr lpData, ref uint lpcbData);

    private const uint HKEY_LOCAL_MACHINE = 0x80000002;
    private const uint KEY_WOW64_64KEY = 0x0100;
    private const uint KEY_READ = 131097;

    private static bool ReadWinAPIValues(ref string procPath, ref string directory)
    {
      uint key = 0;
      if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, @"SOFTWARE\TortoiseSVN", 0, KEY_WOW64_64KEY | KEY_READ, ref key) != 0)
        return false;

      uint size = 512;

      IntPtr pdata = Marshal.AllocHGlobal((int) size);
      if (RegQueryValueEx(key, "ProcPath", IntPtr.Zero, IntPtr.Zero, pdata, ref size) != 0)
      {
        RegCloseKey(key);
        return false;
      }

      byte[] data = new byte[(int)size - 1];
      Marshal.Copy(pdata, data, 0, (int)size - 1);
      procPath = Encoding.Default.GetString(data);

      if (RegQueryValueEx(key, "Directory", IntPtr.Zero, IntPtr.Zero, pdata, ref size) != 0)
      {
        RegCloseKey(key);
        return false;
      }

      data = new byte[(int)size - 1];
      Marshal.Copy(pdata, data, 0, (int)size - 1);
      directory = Encoding.Default.GetString(data);

      Marshal.FreeHGlobal(pdata);

      RegCloseKey(key);
      return true;
    }

    private TortoiseSVNWrapper()
    {
      using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\TortoiseSVN"))
        if (key == null)
        {
          if (!ReadWinAPIValues(ref procPath, ref checkerPath))
            return;
        }
        else
        {
          procPath = (string)key.GetValue("ProcPath");
          checkerPath = (string)key.GetValue("Directory");        
        }

      if (!string.IsNullOrEmpty(checkerPath))
        checkerPath = Path.Combine(checkerPath, "bin\\SubWCRev.exe");
    }

    #region Singletone

    private static TortoiseSVNWrapper instance;

    public static TortoiseSVNWrapper Instance
    {
      get
      {
        if (instance == null)
          instance = new TortoiseSVNWrapper();
        return instance;
      }
    }

    #endregion

    private string procPath;
    private string checkerPath;

    #region IVersionControlSystem implementation

    public bool IsEnabled()
    {
      return !string.IsNullOrEmpty(checkerPath) && !string.IsNullOrEmpty(procPath);
    }

    public bool IsModified(string filename)
    {
      if (!IsEnabled())
        return false;

      Process process = new Process();
      process.StartInfo.FileName = checkerPath;
      process.StartInfo.Arguments = string.Format("{0} -n", filename);
      process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      process.Start();
      process.WaitForExit();

      return process.ExitCode == 7;
    }

    public bool UpdateFile(string filename, bool sync)
    {
      if (!IsEnabled())
        return false;

      return ExecuteTortoiseProc(string.Format("/command:update /path:\"{0}\" /closeonend:3", filename), sync);
    }

    public bool RevertFile(string filename, bool sync)
    {
      if (!IsEnabled())
        return false;

      return ExecuteTortoiseProc(string.Format("/command:revert /path:\"{0}\" /closeonend:3", filename), sync);
    }

    public bool CommitFile(string filename, string logMessage, bool sync)
    {
      if (!IsEnabled())
        return false;

      if (!string.IsNullOrEmpty(logMessage))
        logMessage = string.Format(" /logmsg:\"{0}\"", logMessage);
      if (logMessage == null)
        logMessage = string.Empty;
      return ExecuteTortoiseProc(string.Format("/command:commit /path:\"{0}\" /closeonend:3{1}", filename, logMessage), sync);
    }

    public bool ShowLogs(string filename, bool sync)
    {
      if (!IsEnabled())
        return false;

      return ExecuteTortoiseProc(string.Format("/command:log /path:\"{0}\"", filename), sync);
    }

    #endregion

    #region Internal

    private bool ExecuteTortoiseProc(string arguments, bool sync)
    {
      Process process = new Process();
      process.StartInfo.FileName = procPath;
      process.StartInfo.Arguments = arguments;
      process.Start();
      if (sync)
        process.WaitForExit();
      else
        return true;

      return process.ExitCode == 0;
    }

    #endregion
  }
}
