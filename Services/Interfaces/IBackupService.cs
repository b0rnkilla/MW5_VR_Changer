using MW5_VR_Changer.Enums;
using MW5_VR_Changer.Models;

namespace MW5_VR_Changer.Services.Interfaces
{
    /// <summary>Kapselt die Erstellung und Validierung der lokalen VR/NonVR-Backups.</summary>
    public interface IBackupService
    {
        /// <summary>Ermittelt, ob die Backup-Dateien für VR und NonVR vollständig vorhanden sind.</summary>
        /// <returns>Aktueller Backup-Status.</returns>
        BackupStatus GetBackupStatus();

        /// <summary>Erstellt oder aktualisiert das Backup für den angegebenen Modus aus den Originaldateien.</summary>
        /// <param name="originalPaths">Original-Dateipfade, die gesichert werden sollen.</param>
        /// <param name="mode">Zielmodus (VR oder NonVR), in dessen Ordner gesichert wird.</param>
        void CreateBackup(FilePathSet originalPaths, VrMode mode);
    }
}
