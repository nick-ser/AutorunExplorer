using AuslogicsTest.ViewModels;
using System.Windows;

namespace AuslogicsTest
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainVm = new MainViewModel();
            var mainView = new MainWindow()
            {
                DataContext = mainVm
            };
            mainView.Show();
        }
    }
}
