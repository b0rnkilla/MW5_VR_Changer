using MW5_VR_Changer.Enums;
using System.IO;
using MW5_VR_Changer.Helpers;
using MW5_VR_Changer.Models;
using MW5_VR_Changer.Services.Interfaces;

namespace MW5_VR_Changer.Services
{
    /// <summary>Überschreibt die Originaldateien mit den Dateien aus dem gewählten Backup-Ordner.</summary>
    internal sealed class FileSwitchService : IFileSwitchService
    {
        #region Fields

        private readonly IAppEnvironmentService _appEnvironmentService;

        #endregion

        #region Constructor

        /// <summary>Initialisiert den Service.</summary>
        /// <param name="appEnvironmentService">Service für Arbeitsordner und Pfade.</param>
        public FileSwitchService(IAppEnvironmentService appEnvironmentService)
        {
            ArgumentNullException.ThrowIfNull(appEnvironmentService);
            _appEnvironmentService = appEnvironmentService;
        }

        #endregion

        #region Methods & Events

        /// <inheritdoc/>
        public void SwitchToMode(VrMode mode, FilePathSet originalPaths)
        {
            ArgumentNullException.ThrowIfNull(originalPaths);

            _appEnvironmentService.EnsureWorkingFoldersExist();

            ValidateTargetFile(originalPaths.ModListPath, AppConstants.ModListFileName, nameof(originalPaths.ModListPath));
            ValidateTargetFile(originalPaths.GameIniPath, AppConstants.GameIniFileName, nameof(originalPaths.GameIniPath));
            ValidateTargetFile(originalPaths.GameUserSettingsPath, AppConstants.GameUserSettingsFileName, nameof(originalPaths.GameUserSettingsPath));

            var sourceFolder = _appEnvironmentService.GetModeFolderPath(mode);

            // Reihenfolge ist bewusst: erst modlist.json (Mods), danach die beiden Config-Dateien.
            CopyBackupToOriginal(Path.Combine(sourceFolder, AppConstants.ModListFileName), originalPaths.ModListPath);
            CopyBackupToOriginal(Path.Combine(sourceFolder, AppConstants.GameIniFileName), originalPaths.GameIniPath);
            CopyBackupToOriginal(Path.Combine(sourceFolder, AppConstants.GameUserSettingsFileName), originalPaths.GameUserSettingsPath);
        }

        /// <summary>Kopiert eine Backup-Datei in den Originalpfad und überschreibt sie.</summary>
        /// <param name="sourceFilePath">Pfad zur Backup-Datei.</param>
        /// <param name="targetFilePath">Pfad zur Originaldatei.</param>
        private static void CopyBackupToOriginal(string sourceFilePath, string targetFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Backup source file not found.", sourceFilePath);
            }

            if (!File.Exists(targetFilePath))
            {
                throw new FileNotFoundException("Original target file not found.", targetFilePath);
            }

            File.Copy(sourceFilePath, targetFilePath, overwrite: true);
        }

        /// <summary>Validiert, dass der Pfad gesetzt ist und den erwarteten Dateinamen hat.</summary>
        /// <param name="path">Zu prüfender Dateipfad.</param>
        /// <param name="expectedFileName">Erwarteter Dateiname.</param>
        /// <param name="paramName">Parametername für Fehlermeldungen.</param>
        private static void ValidateTargetFile(string path, string expectedFileName, string paramName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("File path is required.", paramName);
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
