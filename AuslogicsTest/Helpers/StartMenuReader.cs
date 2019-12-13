using AuslogicsTest.ViewModels;
using Microsoft.Win32;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace AuslogicsTest.Helpers
{
    static class StartMenuReader
    {
        public static IEnumerable<FileViewModel> ReadStartMenu(bool isLocalMachine)
        {
            DirectoryInfo globalDir = null;
            var files = new List<FileViewModel>();
            try
            {
                if (isLocalMachine)
                    using (var globalReg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", false))
                    {
                        globalDir = new DirectoryInfo(globalReg.GetValue("Common Startup", "C:\\").ToString());
                    }
                else
                    globalDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Startup));

                if (globalDir.Exists)
                {
                    foreach (var shortcut in globalDir.GetFiles("*.lnk"))
                    {
                        var file = GetShortcutTargetFile(shortcut.FullName);
                        if (file != null)
                            files.Add(file);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show($"Invalid operation: {e.Message} - {e.Source}" );
            }
            catch (System.Security.SecurityException e)
            {
                MessageBox.Show($"An error returned while trying to access \n Error is {e.Message} \n" +
                    "This app requires an administrartor privileges to run properly", "Access is denied");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Unknown exeption's type: {e.Message} \n  {e.Source}");
            }
            return files;
        }

        private static FileViewModel GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);
            var shell = new Shell();
            var folder = shell.NameSpace(pathOnly);
            var folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                var link = (ShellLinkObject)folderItem.GetLink;
                return new FileViewModel(link.Path, link.Arguments, false);
            }
            return null;
        }
    }
}
