using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AuslogicsTest.Helpers
{
    static class ArgumentLineDelimiter
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
        private static IEnumerable<string> CommandLineToArgsWithQuotes(string commandLine)
        {
            int argNum;
            var argv = CommandLineToArgvW(commandLine, out argNum);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var results = new string[argNum];
                for (var i = 0; i < results.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    results[i] = Marshal.PtrToStringUni(p);
                }

                return results;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        private static IEnumerable<string> CommandLineToArgsWithoutQuotes(string commandLine)
        {
            var regex = new Regex(@"(^([a-z]|[A-Z]):(?=\\(?![\0-\37<>:/\\|?*])|\/(?![\0-\37<>:/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:/\\|?*]+))((\\|\/)[^\0-\37<>:/\\|?*]+|(\\|\/)$)*()(?<=\.(exe))");
            var matches = regex.Matches(commandLine);
            if (matches.Count > 0)
            {
                var path = matches[0].Value;
                var args = commandLine.Replace(path, "");
                return new []{ path, args };
            }
            return null;
        }

        public static IEnumerable<string> ParsePathLine(string pathWithArguments)
        {
            if (pathWithArguments.Contains("\""))            
                return CommandLineToArgsWithQuotes(pathWithArguments);
            var lines = CommandLineToArgsWithoutQuotes(pathWithArguments);
            if (lines != null)
                return lines;
            //it's relative path
            return new[] { pathWithArguments };
        }
    }
}
