using MW5_VR_Changer.Enums;

namespace MW5_VR_Changer.Services.Interfaces
{
    /// <summary>Stellt UI-Dialoge bereit, um Dateien auszuwählen und den Modus zu bestätigen.</summary>
    public interface IFileDialogService
    {
        /// <summary>Öffnet einen Dialog zur Auswahl der Datei <c>modlist.json</c>.</summary>
        /// <returns>Vollständiger Dateipfad oder <c>null</c>, wenn abgebrochen wurde.</returns>
        string? PickModListFile();

        /// <summary>Öffnet einen Dialog zur Auswahl der Datei <c>Game.ini</c>.</summary>
        /// <returns>Vollständiger Dateipfad oder <c>null</c>, wenn abgebrochen wurde.</returns>
        string? PickGameIniFile();

        /// <summary>Öffnet einen Dialog zur Auswahl der Datei <c>GameUserSettings.ini</c>.</summary>
        /// <returns>Vollständiger Dateipfad oder <c>null</c>, wenn abgebrochen wurde.</returns>
        string? PickGameUserSettingsFile();

        /// <summary>Fragt den Nutzer, ob die aktuellen Originaldateien VR- oder NonVR-Konfiguration enthalten.</summary>
        /// <returns>Gewählter Modus oder <c>null</c>, wenn abgebrochen wurde.</returns>
        VrMode? ConfirmCurrentOriginalMode();
    }
}
