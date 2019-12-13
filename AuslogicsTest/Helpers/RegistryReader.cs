using AuslogicsTest.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows;

namespace AuslogicsTest.Helpers
{
    static class RegistryReader
    {
        public static IEnumerable<FileViewModel> ReadRegistry(string hkey, string Suffix)
        {
            var key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\";
            RegistryKey globalReg = null;
            try
            {
                if(string.Compare(hkey, "HKEY_LOCAL_MACHINE", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    using (globalReg = Registry.LocalMachine.OpenSubKey(key + Suffix, false))
                    {
                        return GetRegValue(globalReg);
                    }
                }
                //HKEY_CURRENT_USER
                using (globalReg = Registry.CurrentUser.OpenSubKey(key + Suffix, false))
                {
                    return GetRegValue(globalReg);
                }
            }
            catch(InvalidOperationException e)
            {
                MessageBox.Show($"An invalid operation occurred while reading {hkey}\\{key}\\{Suffix}. Error is {e.Message}");
            }
            catch (SecurityException e)
            {
                MessageBox.Show($"An error returned while trying to access {hkey}\\{key}\\{Suffix}. Error is {e.Message}" +
                    "This app requires an administrartor privileges to run properly", "Access is denied");                
            }
            catch (ArgumentException e)
            {
                MessageBox.Show($"{hkey}\\{key}\\{Suffix}. Error is {e.Message.Trim()}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"General exception occurred while reading {hkey}\\{key}\\{Suffix}. Error is {e.Message.Trim()}");
            }
            return null;
        }

        private static IEnumerable<FileViewModel> GetRegValue(RegistryKey globalReg)
        {
            var files = new List<FileViewModel>();
            if (globalReg?.ValueCount > 0)
            {
                foreach (var str in globalReg.GetValueNames())
                {
                    var value = globalReg.GetValue(str, "Error").ToString();
                    files.Add(new FileViewModel(value, true));
                }
            }
            return files;
        }
    }
}
