using MW5_VR_Changer.Models;

namespace MW5_VR_Changer.Services.Interfaces
{
    /// <summary>Sucht die MW5 Originaldateien automatisch über bekannte Standardpfade.</summary>
    public interface IOriginalFileDiscoveryService
    {
        /// <summary>Versucht, alle benötigten Originaldateien zu finden.</summary>
        /// <param name="paths">Gefundene Pfade (vollständig oder teilweise).</param>
        /// <param name="statusMessage">Statusmeldung für die UI.</param>
        /// <returns><c>true</c>, wenn alle Pfade gefunden wurden; andernfalls <c>false</c>.</returns>
        bool TryDiscover(out FilePathSet paths, out string statusMessage);
    }
}
