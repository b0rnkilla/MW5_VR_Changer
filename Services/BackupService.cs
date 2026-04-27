using System.IO;
using MW5_VR_Changer.Enums;
using MW5_VR_Changer.Helpers;
using MW5_VR_Changer.Models;
using MW5_VR_Changer.Services.Interfaces;

namespace MW5_VR_Changer.Services
{
    /// <summary>Erstellt lokale Backups der drei Originaldateien für VR und NonVR.</summary>
    internal sealed class BackupService : IBackupService
    {
        #region Fields

        private readonly IAppEnvironmentService _appEnvironmentService;

        #endregion

        #region Constructor

        /// <summary>Initialisiert den Service.</summary>
        /// <param name="appEnvironmentService">Service für Arbeitsordner und Pfade.</param>
        public BackupService(IAppEnvironmentService appEnvironmentService)
        {
            ArgumentNullException.ThrowIfNull(appEnvironmentService);
            _appEnvironmentService = appEnvironmentService;
        }

        #endregion

        #region Methods & Events

        /// <inheritdoc/>
        public BackupStatus GetBackupStatus()
        {
            _appEnvironmentService.EnsureWorkingFoldersExist();

            return new BackupStatus
            {
                VrBackupComplete = IsBackupComplete(VrMode.VR),
                NonVrBackupComplete = IsBackupComplete(VrMode.NonVR)
            };
        }

        /// <inheritdoc/>
        public void CreateBackup(FilePathSet originalPaths, VrMode mode)
        {
            ArgumentNullException.ThrowIfNull(originalPaths);

            _appEnvironmentService.EnsureWorkingFoldersExist();

            ValidateOriginalFile(originalPaths.ModListPath, AppConstants.ModListFileName, nameof(originalPaths.ModListPath));
            ValidateOriginalFile(originalPaths.GameIniPath, AppConstants.GameIniFileName, nameof(originalPaths.GameIniPath));
            ValidateOriginalFile(originalPaths.GameUserSettingsPath, AppConstants.GameUserSettingsFileName, nameof(originalPaths.GameUserSettingsPath));

            var targetFolder = _appEnvironmentService.GetModeFolderPath(mode);
            if (!Directory.Exists(targetFolder))
            {
                throw new DirectoryNotFoundException($"Backup folder not found: {targetFolder}");
            }

            // Die Kopie erfolgt atomar pro Datei. Ein partielles Backup ist zulässig und wird über den Status erkannt.
            CopyToBackup(originalPaths.ModListPath, Path.Combine(targetFolder, AppConstants.ModListFileName));
            CopyToBackup(originalPaths.GameIniPath, Path.Combine(targetFolder, AppConstants.GameIniFileName));
            CopyToBackup(originalPaths.GameUserSettingsPath, Path.Combine(targetFolder, AppConstants.GameUserSettingsFileName));
        }

        /// <summary>Prüft, ob alle drei Backup-Dateien für den angegebenen Modus vorhanden sind.</summary>
        /// <param name="mode">Zu prüfender Modus.</param>
        /// <returns><c>true</c>, wenn alle drei Dateien existieren; andernfalls <c>false</c>.</returns>
        private bool IsBackupComplete(VrMode mode)
        {
            var folder = _appEnvironmentService.GetModeFolderPath(mode);
            return File.Exists(Path.Combine(folder, AppConstants.ModListFileName))
                && File.Exists(Path.Combine(folder, AppConstants.GameIniFileName))
                && File.Exists(Path.Combine(folder, AppConstants.GameUserSettingsFileName));
        }

        /// <summary>Kopiert eine Datei in den Backup-Ordner und überschreibt eine vorhandene Datei.</summary>
        /// <param name="sourceFilePath">Quell-Dateipfad.</param>
        /// <param name="targetFilePath">Ziel-Dateipfad.</param>
        private static void CopyToBackup(string sourceFilePath, string targetFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Source file not found.", sourceFilePath);
            }

            var targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (string.IsNullOrWhiteSpace(targetDirectory) || !Directory.Exists(targetDirectory))
            {
                throw new DirectoryNotFoundException($"Target directory not found: {targetDirectory}");
            }

            File.Copy(sourceFilePath, targetFilePath, overwrite: true);
        }

        /// <summary>Validiert, dass der Pfad gesetzt ist, existiert und den erwarteten Dateinamen hat.</summary>
        /// <param name="path">Zu prüfender Dateipfad.</param>
        /// <param name="expectedFileName">Erwarteter Dateiname.</param>
        /// <param name="paramName">Parametername für Fehlermeldungen.</param>
        private static void ValidateOriginalFile(string path, string expectedFileName, string paramName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("File path is required.", paramName);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Original file not found.", path);
            }

            var fileName = Path.GetFileName(path);
            if (!string.Equals(fileName, expectedFileName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Expected file name '{expectedFileName}', but got '{fileName}'.", paramName);
            }
        }

        #endregion
    }
}
