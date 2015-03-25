# SVNWrapper
C# wrapper for Tortoise SVN client UI. Used to access Tortoise SVN UI from your .NET code.

#### Also short demo project is included.
Usage:

    SVNTester.exe %path% %command%

Command list:

* status - prints status (is file/folder modified locally) for given %path%
* update - shows SVN Update UI for given %path%
* revert - shows SVN Revert UI for given %path%
* commit - requests commit message and shows SVN Commit UI for given %path% with given message
* logs - shows SVN Log UI for given %path%

Copyright (c) 2015, Broken Event

[http://brokenevent.com](http://brokenevent.com)