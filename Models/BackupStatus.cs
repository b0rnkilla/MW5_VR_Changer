namespace MW5_VR_Changer.Models
{
    /// <summary>Beschreibt, ob die lokalen Backup-Dateien für VR und NonVR vollständig vorhanden sind.</summary>
    public class BackupStatus
    {
        /// <summary>Gibt an, ob das VR-Backup vollständig ist.</summary>
        public bool VrBackupComplete { get; set; }

        /// <summary>Gibt an, ob das NonVR-Backup vollständig ist.</summary>
        public bool NonVrBackupComplete { get; set; }

        /// <summary>Gibt an, ob der Modus-Switch aktiviert werden darf.</summary>
        public bool CanSwitch => VrBackupComplete && NonVrBackupComplete;
    }
}