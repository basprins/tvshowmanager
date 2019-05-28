using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using PerfectCode.FileSystemIO;
using PerfectCode.Logging;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShowManager.UserInterface;
using PerfectCode.TVShowManager.UserInterface.ViewModels;

namespace PerfectCode.TVShowManager
{
    public class TVShowManager
    {
        private static readonly ILogger Log = new Logger(typeof(TVShowManager));

        [STAThread]
        public static void Main(string[] args)
        {
            Logger.Initialize(@"log.config");

            Log.Info("===TV Show Manager started===");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var app = new App();

            var resourceLocater = new Uri("/TV Show Manager;component/app.xaml", UriKind.Relative);
            System.Windows.Application.LoadComponent(app, resourceLocater);

            DispatcherHelper.Initialize();

            var fileSystem = new FileSystem();
            var tvShowRestApiClient = new TVShowRestClient();

            var tvShowLibrary = new TVShowLibrary(
                fileSystem, 
                tvShowRestApiClient);

            var mainViewModel = new MainViewModel(
                new TVShowFactory(), 
                tvShowLibrary, 
                new DialogService(), 
                fileSystem, 
                tvShowRestApiClient, 
                new SeasonsFactory(fileSystem));

            var mainWindow = new MainWindow { DataContext = mainViewModel };
            app.Run(mainWindow);

            Log.Info("===TV Show Manager stopped===");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            MessageBox.Show(unhandledExceptionEventArgs.ExceptionObject.ToString());

            Log.Error($"An unhandled exception occurred: {unhandledExceptionEventArgs.ExceptionObject}");

            Environment.Exit(-1);
        }
    }
}
