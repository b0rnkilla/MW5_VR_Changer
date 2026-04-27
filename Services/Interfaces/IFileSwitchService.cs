using MW5_VR_Changer.Enums;
using MW5_VR_Changer.Models;

namespace MW5_VR_Changer.Services.Interfaces
{
    /// <summary>Schaltet den aktiven Modus um, indem die Backup-Dateien in die Originalpfade kopiert werden.</summary>
    public interface IFileSwitchService
    {
        /// <summary>Aktiviert den angegebenen Modus, indem Originaldateien überschrieben werden.</summary>
        /// <param name="mode">Zielmodus (VR oder NonVR).</param>
        /// <param name="originalPaths">Original-Dateipfade, die überschrieben werden.</param>
        void SwitchToMode(VrMode mode, FilePathSet originalPaths);
    }
}
