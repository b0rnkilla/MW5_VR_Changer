using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using MW5_VR_Changer.Enums;
using MW5_VR_Changer.Helpers;
using MW5_VR_Changer.Models;
using MW5_VR_Changer.Services.Interfaces;

namespace MW5_VR_Changer.ViewModels
{
    /// <summary>
    /// Zentrales ViewModel der Anwendung.<br/>
    /// Es verwaltet Pfad-Ermittlung, Backup-Status und den VR/NonVR-Switch.
    /// </summary>
    public sealed partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly IBackupService _backupService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IFileSwitchService _fileSwitchService;
        private readonly IOriginalFileDiscoveryService _originalFileDiscoveryService;

        // Verhindert, dass UI-Änderungen (z.B. initiales Setzen oder Revert) sofort Switch-Logik auslösen.
        private bool _suppressModeSwitch;

        #endregion

        #region Properties

        [ObservableProperty]
        private string _gamePath = string.Empty;

        [ObservableProperty]
        private string _appDataPath = string.Empty;

        /// <summary>Gibt an, ob ein gültiger Spielpfad gesetzt ist (für UI-Feedback).</summary>
        public bool HasGamePath => !string.IsNullOrWhiteSpace(GamePath);

        /// <summary>Gibt an, ob ein gültiger AppData-Pfad gesetzt ist (für UI-Feedback).</summary>
        public bool HasAppDataPath => !string.IsNullOrWhiteSpace(AppDataPath);

        [ObservableProperty]
        private bool _isVrMode;

        [ObservableProperty]
        private bool _isSwitchEnabled;

        [ObservableProperty]
        private bool _vrBackupComplete;

        [ObservableProperty]
        private bool _nonVrBackupComplete;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        #endregion

        #region Constructor

        /// <summary>Initialisiert das ViewModel und startet die automatische Pfaderkennung.</summary>
        /// <param name="backupService">Service für Backup-Erstellung und -Status.</param>
        /// <param name="fileDialogService">Service für Dateiauswahl und Bestätigungsdialoge.</param>
        /// <param name="fileSwitchService">Service zum Umschalten zwischen VR und NonVR.</param>
        /// <param name="originalFileDiscoveryService">Service zur automatischen Pfaderkennung.</param>
        public MainViewModel(
            IBackupService backupService,
            IFileDialogService fileDialogService,
            IFileSwitchService fileSwitchService,
            IOriginalFileDiscoveryService originalFileDiscoveryService)
        {
            ArgumentNullException.ThrowIfNull(backupService);
            ArgumentNullException.ThrowIfNull(fileDialogService);
            ArgumentNullException.ThrowIfNull(fileSwitchService);
            ArgumentNullException.ThrowIfNull(originalFileDiscoveryService);

            _backupService = backupService;
            _fileDialogService = fileDialogService;
            _fileSwitchService = fileSwitchService;
            _originalFileDiscoveryService = originalFileDiscoveryService;

            _suppressModeSwitch = true;
            AutoDetectPaths();
            RefreshBackupStatus();
            _suppressModeSwitch = false;
        }

        #endregion

        #region Methods & Events

        /// <summary>Reagiert auf Änderungen des Spielpfads und aktualisiert UI-Abhängigkeiten.</summary>
        /// <param name="value">Neuer Spielpfad.</param>
        partial void OnGamePathChanged(string value)
        {
            OnPropertyChanged(nameof(HasGamePath));
            CreateBackupCommand.NotifyCanExecuteChanged();
        }

        /// <summary>Versucht, die Originaldateien automatisch zu finden und die Root-Pfade zu setzen.</summary>
        [RelayCommand]
        private void AutoDetectPaths()
        {
            try
            {
                if (_originalFileDiscoveryService.TryDiscover(out var paths, out var message))
                {
                    // Aus den gefundenen Dateien werden Root-Pfade abgeleitet, da die UI nur zwei Pfade verwaltet.
                    var gamePath = TryGetGameRootFromModList(paths.ModListPath);
                    if (!string.IsNullOrWhiteSpace(gamePath))
                    {
                        GamePath = gamePath;
                    }

                    var appDataPath = TryGetAppDataRootFromConfigFile(paths.GameIniPath);
                    if (!string.IsNullOrWhiteSpace(appDataPath))
                    {
                        AppDataPath = appDataPath;
                    }
                }

                StatusMessage = message;
            }
            catch (IOException ex)
            {
                StatusMessage = $"Fehler bei der automatischen Suche: {ex.Message}";
            }
            catch (UnauthorizedAccessException ex)
            {
                StatusMessage = $"Fehler bei der automatischen Suche: {ex.Message}";
            }
        }

        /// <summary>Reagiert auf Änderungen des AppData-Pfads und aktualisiert UI-Abhängigkeiten.</summary>
        /// <param name="value">Neuer AppData-Pfad.</param>
        partial void OnAppDataPathChanged(string value)
        {
            OnPropertyChanged(nameof(HasAppDataPath));
            CreateBackupCommand.NotifyCanExecuteChanged();
        }

        /// <summary>Wird bei UI-Änderungen des Toggles ausgelöst und startet das Umschalten.</summary>
        /// <param name="value">Neuer Toggle-Wert.</param>
        partial void OnIsVrModeChanged(bool value)
        {
            if (_suppressModeSwitch)
            {
                return;
            }

            var desiredMode = value ? VrMode.VR : VrMode.NonVR;
            TrySwitchMode(desiredMode);
        }

        private void RefreshBackupStatus()
        {
            var status = _backupService.GetBackupStatus();

            VrBackupComplete = status.VrBackupComplete;
            NonVrBackupComplete = status.NonVrBackupComplete;

            IsSwitchEnabled = status.CanSwitch;

            CreateBackupCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Leitet die drei Original-Dateipfade aus den beiden Root-Pfaden ab.<br/>
        /// Wird für Backup und Switch verwendet.
        /// </summary>
        /// <returns>Set der drei Original-Dateipfade oder ein leeres Set, wenn Root-Pfade fehlen.</returns>
        private FilePathSet GetCurrentOriginalPaths()
        {
            if (string.IsNullOrWhiteSpace(GamePath) || string.IsNullOrWhiteSpace(AppDataPath))
            {
                return new FilePathSet();
            }

            var modListPath = Path.Combine(GamePath, "MW5Mercs", "Mods", AppConstants.ModListFileName);
            var configFolderPath = Path.Combine(AppDataPath, "Saved", "Config", "WindowsNoEditor");

            return new FilePathSet
            {
                ModListPath = modListPath,
                GameIniPath = Path.Combine(configFolderPath, AppConstants.GameIniFileName),
                GameUserSettingsPath = Path.Combine(configFolderPath, AppConstants.GameUserSettingsFileName)
            };
        }

        /// <summary>Versucht, auf den gewünschten Modus umzuschalten und fängt erwartbare Fehler ab.</summary>
        /// <param name="desiredMode">Zielmodus.</param>
        private void TrySwitchMode(VrMode desiredMode)
        {
            if (!IsSwitchEnabled)
            {
                StatusMessage = "Umschalten ist erst möglich, wenn VR und NonVR vollständig gesichert sind.";
                RevertToggle(desiredMode);
                return;
            }

            try
            {
                _fileSwitchService.SwitchToMode(desiredMode, GetCurrentOriginalPaths());

                StatusMessage = desiredMode == VrMode.VR
                    ? "Auf VR umgeschaltet."
                    : "Auf NonVR umgeschaltet.";
            }
            catch (ArgumentException ex)
            {
                StatusMessage = $"Fehler beim Umschalten: {ex.Message}";
                RevertToggle(desiredMode);
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = $"Fehler beim Umschalten: {ex.Message}";
                RevertToggle(desiredMode);
            }
            catch (IOException ex)
            {
                StatusMessage = $"Fehler beim Umschalten: {ex.Message}";
                RevertToggle(desiredMode);
            }
            catch (UnauthorizedAccessException ex)
            {
                StatusMessage = $"Fehler beim Umschalten: {ex.Message}";
                RevertToggle(desiredMode);
            }
        }

        /// <summary>Setzt den Toggle wieder auf den vorherigen Zustand zurück, ohne Switch-Logik auszulösen.</summary>
        /// <param name="desiredMode">Der Modus, auf den ursprünglich gewechselt werden sollte.</param>
        private void RevertToggle(VrMode desiredMode)
        {
            // Revert muss „silent“ passieren, sonst würde OnIsVrModeChanged erneut schalten.
            _suppressModeSwitch = true;
            IsVrMode = desiredMode != VrMode.VR;
            _suppressModeSwitch = false;
        }

        /// <summary>Prüft, ob aktuell ein Backup erstellt werden kann.</summary>
        /// <returns><c>true</c>, wenn alle benötigten Originaldateien existieren; andernfalls <c>false</c>.</returns>
        private bool CanCreateBackup()
        {
            var paths = GetCurrentOriginalPaths();
            return paths.IsComplete()
                && File.Exists(paths.ModListPath)
                && File.Exists(paths.GameIniPath)
                && File.Exists(paths.GameUserSettingsPath);
        }

        /// <summary>Lässt den Nutzer die Datei <c>modlist.json</c> auswählen und setzt daraus den Spielpfad.</summary>
        [RelayCommand]
        private void BrowseGame()
        {
            var filePath = _fileDialogService.PickModListFile();
            if (filePath is null)
            {
                return;
            }

            var root = TryGetGameRootFromModList(filePath);
            if (string.IsNullOrWhiteSpace(root))
            {
                StatusMessage = "Ungültige Auswahl. Bitte die Datei 'modlist.json' im Spielordner auswählen.";
                return;
            }

            GamePath = root;
        }

        /// <summary>Lässt den Nutzer die Datei <c>Game.ini</c> auswählen und setzt daraus den AppData-Pfad.</summary>
        [RelayCommand]
        private void BrowseAppData()
        {
            var filePath = _fileDialogService.PickGameIniFile();
            if (filePath is null)
            {
                return;
            }

            var root = TryGetAppDataRootFromConfigFile(filePath);
            if (string.IsNullOrWhiteSpace(root))
            {
                StatusMessage = "Ungültige Auswahl. Bitte die Datei 'Game.ini' im AppData-Konfigurationsordner auswählen.";
                return;
            }

            AppDataPath = root;
        }

        /// <summary>Leitet den Spiel-Root aus einem Pfad zu <c>modlist.json</c> ab.</summary>
        /// <param name="modListPath">Pfad zur <c>modlist.json</c>.</param>
        /// <returns>Spiel-Root oder <c>null</c>, wenn das Muster nicht passt.</returns>
        private static string? TryGetGameRootFromModList(string? modListPath)
        {
            if (string.IsNullOrWhiteSpace(modListPath) || !File.Exists(modListPath))
            {
                return null;
            }

            var fileName = Path.GetFileName(modListPath);
            if (!string.Equals(fileName, AppConstants.ModListFileName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var modsDir = Directory.GetParent(modListPath);
            var mw5Dir = modsDir?.Parent;
            var gameDir = mw5Dir?.Parent;

            if (modsDir is null || mw5Dir is null || gameDir is null)
            {
                return null;
            }

            if (!string.Equals(modsDir.Name, "Mods", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!string.Equals(mw5Dir.Name, "MW5Mercs", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return gameDir.FullName;
        }

        /// <summary>Leitet den AppData-Root (MW5Mercs) aus einem Pfad zu einer Config-Datei ab.</summary>
        /// <param name="configFilePath">Pfad zu <c>Game.ini</c> oder <c>GameUserSettings.ini</c>.</param>
        /// <returns>AppData-Root oder <c>null</c>, wenn das Muster nicht passt.</returns>
        private static string? TryGetAppDataRootFromConfigFile(string? configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath) || !File.Exists(configFilePath))
            {
                return null;
            }

            var fileName = Path.GetFileName(configFilePath);
            if (!string.Equals(fileName, AppConstants.GameIniFileName, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(fileName, AppConstants.GameUserSettingsFileName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var configDir = Directory.GetParent(configFilePath);
            if (configDir is null || !string.Equals(configDir.Name, "WindowsNoEditor", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var configParent = configDir.Parent;
            var savedDir = configParent?.Parent;
            var mw5Root = savedDir?.Parent;

            if (configParent is null || savedDir is null || mw5Root is null)
            {
                return null;
            }

            if (!string.Equals(configParent.Name, "Config", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!string.Equals(savedDir.Name, "Saved", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!string.Equals(mw5Root.Name, "MW5Mercs", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return mw5Root.FullName;
        }

        /// <summary>Erstellt ein VR- oder NonVR-Backup basierend auf dem vom Nutzer bestätigten aktuellen Modus.</summary>
        [RelayCommand(CanExecute = nameof(CanCreateBackup))]
        private void CreateBackup()
        {
            try
            {
                var confirmedMode = _fileDialogService.ConfirmCurrentOriginalMode();
                if (confirmedMode is null)
                {
                    StatusMessage = "Backup-Erstellung abgebrochen.";
                    return;
                }

                _backupService.CreateBackup(GetCurrentOriginalPaths(), confirmedMode.Value);

                _suppressModeSwitch = true;
                IsVrMode = confirmedMode.Value == VrMode.VR;
                _suppressModeSwitch = false;

                RefreshBackupStatus();

                StatusMessage = confirmedMode.Value == VrMode.VR
                    ? "VR-Backup erstellt/aktualisiert."
                    : "NonVR-Backup erstellt/aktualisiert.";
            }
            catch (ArgumentException ex)
            {
                StatusMessage = $"Fehler beim Backup: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = $"Fehler beim Backup: {ex.Message}";
            }
            catch (IOException ex)
            {
                StatusMessage = $"Fehler beim Backup: {ex.Message}";
            }
            catch (UnauthorizedAccessException ex)
            {
                StatusMessage = $"Fehler beim Backup: {ex.Message}";
            }
        }

        #endregion
    }
}
