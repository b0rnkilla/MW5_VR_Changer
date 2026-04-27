using MW5_VR_Changer.Enums;

namespace MW5_VR_Changer.Services.Interfaces
{
    /// <summary>Stellt Pfade und Ordnerfunktionen für das Laufzeit-Umfeld der Anwendung bereit.</summary>
    public interface IAppEnvironmentService
    {
        /// <summary>Verzeichnis, in dem die ausführbare Datei liegt.</summary>
        string AppRootPath { get; }

        /// <summary>Arbeitsverzeichnis der App, in dem Backups abgelegt werden.</summary>
        string WorkingRootPath { get; }

        /// <summary>Stellt sicher, dass alle benötigten Arbeitsordner existieren.</summary>
        void EnsureWorkingFoldersExist();

        /// <summary>Gibt den Pfad zum Backup-Ordner für den angegebenen Modus zurück.</summary>
        /// <param name="mode">Gewünschter Modus.</param>
        /// <returns>Vollständiger Pfad zum Mode-Ordner.</returns>
        string GetModeFolderPath(VrMode mode);
    }
}
