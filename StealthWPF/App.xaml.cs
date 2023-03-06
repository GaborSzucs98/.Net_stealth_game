using StealthWPF.ViewModel;
using StealthWPF.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StealthWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    { 
        private MainViewModel? _mainViewModel;
        private MainWindow? _mainWindow;

        public App()
        {
            Startup += OnStartup;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            _mainViewModel = new MainViewModel();

            _mainWindow = new MainWindow();

            _mainWindow.DataContext = _mainViewModel;

            _mainWindow.Show();
        }
    }
}
