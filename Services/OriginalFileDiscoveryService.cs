using MW5_VR_Changer.Helpers;
using MW5_VR_Changer.Models;
using MW5_VR_Changer.Services.Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace MW5_VR_Changer.Services
{
    /// <summary>Sucht die drei Originaldateien automatisch über bekannte Steam- und AppData-Standardpfade.</summary>
    internal sealed class OriginalFileDiscoveryService : IOriginalFileDiscoveryService
    {
        #region Fields

        /// <summary>Regex zum Parsen der Steam <c>libraryfolders.vdf</c>, um zusätzliche Library-Pfade zu finden.</summary>
        private static readonly Regex SteamLibraryPathRegex = new(
            "\\\"path\\\"\\s*\\\"(?<path>[^\\\"]+)\\\"",
            RegexOptions.IgnoreCase);

        #endregion

        #region Methods & Events

        /// <inheritdoc/>
        public bool TryDiscover(out FilePathSet paths, out string statusMessage)
        {
            paths = new FilePathSet();
            statusMessage = string.Empty;

            var modListPath = FindModListPath(out var modListHint);
            var configFolderPath = FindConfigFolderPath(out var configHint);

            if (!string.IsNullOrWhiteSpace(modListPath))
            {
                paths.ModListPath = modListPath;
            }

            if (!string.IsNullOrWhiteSpace(configFolderPath))
            {
                var gameIni = Path.Combine(configFolderPath, AppConstants.GameIniFileName);
                var gusIni = Path.Combine(configFolderPath, AppConstants.GameUserSettingsFileName);

                if (File.Exists(gameIni))
                {
                    paths.GameIniPath = gameIni;
                }

                if (File.Exists(gusIni))
                {
                    paths.GameUserSettingsPath = gusIni;
                }
            }

            if (paths.IsComplete())
            {
                statusMessage = "Originaldateien automatisch gefunden.";

                return true;
            }

            statusMessage = BuildFailureMessage(paths, modListHint, configHint);
            return false;
        }

        /// <summary>Erstellt eine UI-geeignete Statusmeldung, wenn nicht alle Pfade gefunden wurden.</summary>
        /// <param name="partial">Teilweise gefundene Pfade.</param>
        /// <param name="modListHint">Optionaler Hinweis zur Steam-Quelle.</param>
        /// <param name="configHint">Optionaler Hinweis zur AppData-Quelle.</param>
        /// <returns>Statusmeldung für die UI.</returns>
        private static string BuildFailureMessage(FilePathSet partial, string? modListHint, string? configHint)
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(partial.ModListPath))
            {
                missing.Add(AppConstants.ModListFileName);
            }

            if (string.IsNullOrWhiteSpace(partial.GameIniPath))
            {
                missing.Add(AppConstants.GameIniFileName);
            }

            if (string.IsNullOrWhiteSpace(partial.GameUserSettingsPath))
            {
                missing.Add(AppConstants.GameUserSettingsFileName);
            }

            var hintParts = new[] { modListHint, configHint }.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            var hint = hintParts.Length > 0 ? $" Hinweise: {string.Join("; ", hintParts)}." : string.Empty;

            return $"Automatische Suche unvollständig. Fehlend: {string.Join(", ", missing)}. Bitte Pfade manuell auswählen.{hint}";
        }

        private static string? FindModListPath(out string? hint)
        {
            hint = null;

            foreach (var steamRoot in GetCandidateSteamRoots())
            {
                var modListPath = FindModListInSteamRoot(steamRoot, out var rootHint);
                if (!string.IsNullOrWhiteSpace(modListPath))
                {
                    hint = rootHint;
                    return modListPath;
                }
            }

            return null;
        }

        /// <summary>Durchsucht eine konkrete Steam-Root-Installation nach MW5.</summary>
        /// <param name="steamRoot">Root-Verzeichnis der Steam-Installation (z.B. <c>C:\Program Files (x86)\Steam</c>).</param>
        /// <param name="hint">Hinweis, ob Standardpfad oder Library-Treffer.</param>
        /// <returns>Pfad zur <c>modlist.json</c> oder <c>null</c>.</returns>
        private static string? FindModListInSteamRoot(string steamRoot, out string? hint)
        {
            hint = null;

            var steamAppsPath = Path.Combine(steamRoot, AppConstants.SteamAppsFolderName);
            var defaultCandidate = BuildModListPath(steamAppsPath);
            if (File.Exists(defaultCandidate))
            {
                hint = $"Steam: {steamRoot}";
                return defaultCandidate;
            }

            var libraryFoldersFile = Path.Combine(steamAppsPath, "libraryfolders.vdf");
            if (!File.Exists(libraryFoldersFile))
            {
                return null;
            }

            foreach (var libraryRoot in ReadSteamLibraryRoots(libraryFoldersFile))
            {
                var librarySteamAppsPath = Path.Combine(libraryRoot, AppConstants.SteamAppsFolderName);
                var candidate = BuildModListPath(librarySteamAppsPath);
                if (File.Exists(candidate))
                {
                    hint = $"Steam library: {libraryRoot}";
                    return candidate;
                }
            }

            return null;
        }

        /// <summary>Baut den erwarteten Pfad zur <c>modlist.json</c> für ein gegebenes <c>steamapps</c>-Verzeichnis.</summary>
        /// <param name="steamAppsPath">Pfad zum <c>steamapps</c>-Verzeichnis.</param>
        /// <returns>Vollständiger erwarteter Dateipfad.</returns>
        private static string BuildModListPath(string steamAppsPath)
        {
            return Path.Combine(
                steamAppsPath,
                AppConstants.SteamCommonFolderName,
                AppConstants.Mw5SteamGameFolderName,
                AppConstants.Mw5GameSubFolderName,
                AppConstants.ModsFolderName,
                AppConstants.ModListFileName);
        }

        /// <summary>Ermittelt Kandidaten-Verzeichnisse, in denen Steam installiert sein könnte.</summary>
        /// <returns>Existierende Verzeichnisse, die als Steam-Root in Frage kommen.</returns>
        private static IEnumerable<string> GetCandidateSteamRoots()
        {
            var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (!string.IsNullOrWhiteSpace(programFilesX86))
            {
                results.Add(Path.Combine(programFilesX86, AppConstants.SteamFolderName));
            }

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (!string.IsNullOrWhiteSpace(programFiles))
            {
                results.Add(Path.Combine(programFiles, AppConstants.SteamFolderName));
            }

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed && d.IsReady))
            {
                // Manche Nutzer haben Steam/SteamLibrary direkt im Laufwerksroot.
                results.Add(Path.Combine(drive.RootDirectory.FullName, AppConstants.SteamFolderName));
                results.Add(Path.Combine(drive.RootDirectory.FullName, AppConstants.SteamLibraryFolderName));
            }

            return results.Where(Directory.Exists);
        }

        /// <summary>Liest zusätzliche Steam-Library-Roots aus der Datei <c>libraryfolders.vdf</c>.</summary>
        /// <param name="libraryFoldersFile">Pfad zur <c>libraryfolders.vdf</c>.</param>
        /// <returns>Liste der gefundenen Library-Verzeichnisse.</returns>
        private static IEnumerable<string> ReadSteamLibraryRoots(string libraryFoldersFile)
        {
            var text = File.ReadAllText(libraryFoldersFile);
            var matches = SteamLibraryPathRegex.Matches(text);

            foreach (Match match in matches)
            {
                var raw = match.Groups["path"].Value;
                if (string.IsNullOrWhiteSpace(raw))
                {
                    continue;
                }

                var normalized = raw.Replace("\\\\", "\\");
                if (Directory.Exists(normalized))
                {
                    yield return normalized;
                }
            }
        }

        /// <summary>Versucht, den Konfigurationsordner unter AppData zu finden, in dem <c>Game.ini</c> liegt.</summary>
        /// <param name="hint">Hinweis, ob aktueller User oder ein anderer User-Verzeichnis-Treffer.</param>
        /// <returns>Pfad zum Konfigurationsordner oder <c>null</c>.</returns>
        private static string? FindConfigFolderPath(out string? hint)
        {
            hint = null;

            var currentUserLocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrWhiteSpace(currentUserLocalAppData))
            {
                var currentUserBase = Path.Combine(currentUserLocalAppData, AppConstants.Mw5AppDataFolderName);
                var currentUserConfig = Path.Combine(currentUserBase, "Saved", "Config", "WindowsNoEditor");

                if (Directory.Exists(currentUserConfig))
                {
                    hint = "AppData: current user";
                    return currentUserConfig;
                }
            }

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var usersRoot = Directory.GetParent(userProfile)?.FullName;
            if (string.IsNullOrWhiteSpace(usersRoot) || !Directory.Exists(usersRoot))
            {
                return null;
            }

            // Fallback: Wenn der aktuelle Benutzer nicht passt (z.B. portable Installation), werden andere Profile geprüft.
            foreach (var userDir in Directory.EnumerateDirectories(usersRoot))
            {
                var candidate = Path.Combine(userDir, "AppData", "Local", AppConstants.Mw5AppDataFolderName, AppConstants.Mw5ConfigFolderRelativeToAppData);
                if (Directory.Exists(candidate))
                {
                    hint = $"AppData user: {Path.GetFileName(userDir)}";
                    return candidate;
                }
            }

            return null;
        }

        #endregion
    }
}
