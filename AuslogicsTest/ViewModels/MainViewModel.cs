using AuslogicsTest.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuslogicsTest.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        #region fields
        private bool _isBusy;
        private bool _isRegistry = true;
        private bool _isStartMenu;
        private readonly ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();
        #endregion

        #region properties
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsRegistry
        {
            get { return _isRegistry; }
            set
            {
                _isRegistry = value;
                if (_isRegistry)
                    FillDataFromRegistry();
                else
                    RemoveRegistryData();
                NotifyPropertyChanged();
            }
        }

        public bool IsStartMenu
        {
            get { return _isStartMenu; }
            set
            {
                _isStartMenu = value;
                if (_isStartMenu)
                    FillDataFromStartMenu();
                else
                    RemoveStartMenuData();
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<FileViewModel> Files
        {
            get { return _files; }
        }
        #endregion

        public MainViewModel()
        {
            FillDataFromRegistry();
        }

        private void FillDataFromRegistry()
        {
            Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() => IsBusy = true);
                var registrylocalMachineFiles = RegistryReader.ReadRegistry("HKEY_LOCAL_MACHINE", "Run")?.ToList();
                var registryCurrentUserFiles = RegistryReader.ReadRegistry("HKEY_CURRENT_USER", "Run")?.ToList();
                if (registrylocalMachineFiles != null)
                    App.Current.Dispatcher.Invoke(() => registrylocalMachineFiles.ForEach(file => Files.Add(file)));
                if (registryCurrentUserFiles != null)
                    App.Current.Dispatcher.Invoke(() => registryCurrentUserFiles.ForEach(file => Files.Add(file)));
            }).ContinueWith((t) => App.Current.Dispatcher.Invoke(() => IsBusy = false));
        }

        private static Task StartSTATask(Action func)
        {
            var tcs = new TaskCompletionSource<object>();
            var thread = new Thread(() =>
            {
                try
                {
                    func();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        private void FillDataFromStartMenu()
        {
            var task = StartSTATask(() =>
            {
                App.Current.Dispatcher.Invoke(() => IsBusy = true);
                var smlocalMachineFiles = StartMenuReader.ReadStartMenu(true)?.ToList();
                var smCurrentUserFiles = StartMenuReader.ReadStartMenu(false)?.ToList();
                if (smlocalMachineFiles != null)
                    App.Current.Dispatcher.Invoke(() => smlocalMachineFiles.ForEach(file => Files.Add(file)));
                if (smCurrentUserFiles != null)
                    App.Current.Dispatcher.Invoke(() => smCurrentUserFiles.ForEach(file => Files.Add(file)));
            });
            task.ContinueWith((t) => App.Current.Dispatcher.Invoke(() => IsBusy = false));
            task.Start();
        }

        private void RemoveRegistryData()
        {
            var regFiles = Files.Where(file => file.IsRegistryType).ToList();
            regFiles.ForEach(file => Files.Remove(file));
        }

        private void RemoveStartMenuData()
        {
            var smFiles = Files.Where(file => !file.IsRegistryType).ToList();
            smFiles.ForEach(file => Files.Remove(file));
        }
    }
}
