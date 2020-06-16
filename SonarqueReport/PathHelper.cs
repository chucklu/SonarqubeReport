using System;

namespace SonarqueReport
{
    class PathHelper
    {
        public static string GetDownloadFolderPath()
        {
            return System.Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
                    , "{374DE290-123F-4565-9164-39C4925E467B}"
                    , String.Empty
                )
            );
        }
    }
}
