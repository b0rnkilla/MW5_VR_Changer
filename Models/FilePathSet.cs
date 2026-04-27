namespace MW5_VR_Changer.Models
{
    /// <summary>Enthält die drei Original-Dateipfade, die im Spiel überschrieben werden.</summary>
    public class FilePathSet
    {
        /// <summary>Vollständiger Pfad zur Datei <c>modlist.json</c>.</summary>
        public string ModListPath { get; set; } = string.Empty;

        /// <summary>Vollständiger Pfad zur Datei <c>Game.ini</c>.</summary>
        public string GameIniPath { get; set; } = string.Empty;

        /// <summary>Vollständiger Pfad zur Datei <c>GameUserSettings.ini</c>.</summary>
        public string GameUserSettingsPath { get; set; } = string.Empty;

        /// <summary>Prüft, ob alle drei Pfade gesetzt sind.</summary>
        /// <returns><c>true</c>, wenn alle Pfade nicht leer sind; andernfalls <c>false</c>.</returns>
        public bool IsComplete()
        {
            return !string.IsNullOrWhiteSpace(ModListPath)
                && !string.IsNullOrWhiteSpace(GameIniPath)
                && !string.IsNullOrWhiteSpace(GameUserSettingsPath);
        }
    }
}
