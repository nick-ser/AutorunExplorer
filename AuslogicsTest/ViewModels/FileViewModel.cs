using AuslogicsTest.Helpers;
using System;
using System.Linq;
using System.Text;

namespace AuslogicsTest.ViewModels
{
    class FileViewModel : BaseViewModel
    {
        #region fields
        private string _name;
        private string _commandArguments;
        private string _path;
        private bool _isRegistryType;
        #endregion

        #region properties
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string CommandArguments
        {
            get { return _commandArguments; }
            set
            {
                _commandArguments = value;
                NotifyPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsRegistryType
        {
            get { return _isRegistryType; }
            set
            {
                _isRegistryType = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public FileViewModel(string pathWithArguments, bool isRegistryType)
        {
            if (string.IsNullOrEmpty(pathWithArguments) || 
                string.Compare(pathWithArguments, "Error", StringComparison.InvariantCultureIgnoreCase) == 0)
                throw new ArgumentException("The path string is empty.");
            var lines = ArgumentLineDelimiter.ParsePathLine(pathWithArguments).ToList();
            Path = lines[0];
            if (lines.Count > 1)
            {
                var sb = new StringBuilder();
                for(int i=1; i<lines.Count; i++)
                {
                    sb.Append(lines[i]);
                    sb.Append(" ");
                }
                CommandArguments = sb.ToString();
            }
            Name = System.IO.Path.GetFileName(Path);
            IsRegistryType = isRegistryType;
        }

        public FileViewModel(string path, string arguments, bool isRegistryType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The path string is empty.");

            Path = path;
            CommandArguments = arguments;
            Name = System.IO.Path.GetFileName(Path);
            IsRegistryType = isRegistryType;
        }
    }
}
