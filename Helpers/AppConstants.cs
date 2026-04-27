namespace MW5_VR_Changer.Helpers
{
    /// <summary>Zentrale Konstanten für Ordner- und Dateinamen der Anwendung.</summary>
    public static class AppConstants
    {
        /// <summary>Arbeitsordner der App (neben der ausführbaren Datei).</summary>
        public const string RootFolderName = "MW5VRC";

        /// <summary>Backup-Unterordner für VR.</summary>
        public const string VrFolderName = "VR";

        /// <summary>Backup-Unterordner für NonVR.</summary>
        public const string NonVrFolderName = "NonVR";

        /// <summary>Dateiname der MW5 Modliste.</summary>
        public const string ModListFileName = "modlist.json";

        /// <summary>Dateiname der Spiel-Konfigurationsdatei.</summary>
        public const string GameIniFileName = "Game.ini";

        /// <summary>Dateiname der User-Settings-Konfigurationsdatei.</summary>
        public const string GameUserSettingsFileName = "GameUserSettings.ini";

        /// <summary>Unterordner innerhalb der MW5 Installation, der die Spieldaten enthält.</summary>
        public const string Mw5GameSubFolderName = "MW5Mercs";

        /// <summary>Mods-Unterordner innerhalb der MW5 Installation.</summary>
        public const string ModsFolderName = "Mods";

        /// <summary>Standardordnername für Steam Bibliotheken.</summary>
        public const string SteamAppsFolderName = "steamapps";

        /// <summary>Unterordner von Steam, in dem Spiele standardmäßig liegen.</summary>
        public const string SteamCommonFolderName = "common";

        /// <summary>Standardordnername für eine Steam-Installation.</summary>
        public const string SteamFolderName = "Steam";

        /// <summary>Standardordnername für eine zusätzliche SteamLibrary.</summary>
        public const string SteamLibraryFolderName = "SteamLibrary";

        /// <summary>Ordnername des Spiels innerhalb der Steam-Library.</summary>
        public const string Mw5SteamGameFolderName = "MechWarrior 5 Mercenaries";

        /// <summary>AppData-Ordnername des Spiels unter LocalAppData.</summary>
        public const string Mw5AppDataFolderName = "MW5Mercs";

        /// <summary>Pfad relativ zum MW5 AppData-Root, in dem sich die Konfigurationsdateien befinden.</summary>
        public const string Mw5ConfigFolderRelativeToAppData = "Saved\\Config\\WindowsNoEditor";
    }
}
