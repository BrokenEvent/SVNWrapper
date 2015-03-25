namespace SVNWrapper
{
  public interface IVersionControlSystem
  {
    /// <summary>
    /// Checks if SVN client operational
    /// </summary>
    /// <returns>True if client found, false otherwise</returns>
    bool IsEnabled();

    /// <summary>
    /// Check file for local modifications. Call is always synchronous
    /// </summary>
    /// <param name="filename">File to check</param>
    /// <returns>True if file is modified locally. False if not or if client is not operational</returns>
    bool IsModified(string filename);

    /// <summary>
    /// Update file from repository
    /// </summary>
    /// <param name="filename">File to update</param>
    /// <param name="sync">True to call synchronous and wait for execution</param>
    /// <returns>True if update procedure commence. False if client is not operational</returns>
    bool UpdateFile(string filename, bool sync);

    /// <summary>
    /// Revert file to base version
    /// </summary>
    /// <param name="filename">File to revert</param>
    /// <param name="sync">True to call Synchronous and wait for execution</param>
    /// <returns>True if succeed, false if user has canceled the operation or client is not operational</returns>
    bool RevertFile(string filename, bool sync);

    /// <summary>
    /// Commit file to repository
    /// </summary>
    /// <param name="filename">File to commit</param>
    /// <param name="logMessage">Optional predefined log message. May be empty or null if not used</param>
    /// <param name="sync">True to call synchronous and wait for execution</param>
    /// <returns>True if succeed, false if user has canceled the operation or client is not operational</returns>
    bool CommitFile(string filename, string logMessage, bool sync);

    /// <summary>
    /// Show logs interface for file
    /// </summary>
    /// <param name="filename">File to show logs</param>
    /// <param name="sync">True to call synchronous and wait until user closed dialog</param>
    /// <returns>False if client is not operational</returns>
    bool ShowLogs(string filename, bool sync);
  }
}
