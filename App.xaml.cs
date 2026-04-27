using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MW5_VR_Changer.Services;
using MW5_VR_Changer.Services.Interfaces;
using MW5_VR_Changer.ViewModels;

namespace MW5_VR_Changer
{
    /// <summary>WPF-App-Einstiegspunkt, der Dependency Injection konfiguriert und das Hauptfenster startet.</summary>
    public partial class App : Application
    {
        #region Fields

        private ServiceProvider? _serviceProvider;

        #endregion

        #region Methods & Events

        /// <summary>Initialisiert Services, stellt Arbeitsordner sicher und zeigt das Hauptfenster an.</summary>
        /// <param name="sender">Event-Sender.</param>
        /// <param name="e">Startup-Argumente.</param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAppEnvironmentService, AppEnvironmentService>();
            services.AddSingleton<IBackupService, BackupService>();
            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<IFileSwitchService, FileSwitchService>();
            services.AddSingleton<IOriginalFileDiscoveryService, OriginalFileDiscoveryService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();

            var environmentService = _serviceProvider.GetRequiredService<IAppEnvironmentService>();
            environmentService.EnsureWorkingFoldersExist();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        #endregion
    }
}
