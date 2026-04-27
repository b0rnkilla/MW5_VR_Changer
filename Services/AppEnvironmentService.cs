using System.IO;
using MW5_VR_Changer.Enums;
using MW5_VR_Changer.Helpers;
using MW5_VR_Changer.Services.Interfaces;

namespace MW5_VR_Changer.Services
{
    /// <summary>Ermittelt und verwaltet Anwendungs- und Arbeitsverzeichnisse für Backups.</summary>
    internal sealed class AppEnvironmentService : IAppEnvironmentService
    {
        #region Fields

        private readonly string _appRootPath;
        private readonly string _workingRootPath;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string AppRootPath => _appRootPath;

        /// <inheritdoc/>
        public string WorkingRootPath => _workingRootPath;

        #endregion

        #region Constructor

        /// <summary>Initialisiert das Umfeld basierend auf dem Verzeichnis der ausführbaren Datei.</summary>
        public AppEnvironmentService()
        {
            _appRootPath = AppContext.BaseDirectory;
            _workingRootPath = Path.Combine(_appRootPath, AppConstants.RootFolderName);
        }

        #endregion

        #region Methods & Events

        /// <inheritdoc/>
        public void EnsureWorkingFoldersExist()
        {
            Directory.CreateDirectory(_workingRootPath);
            Directory.CreateDirectory(GetModeFolderPath(VrMode.VR));
            Directory.CreateDirectory(GetModeFolderPath(VrMode.NonVR));
        }

        /// <inheritdoc/>
        public string GetModeFolderPath(VrMode mode)
        {
            var folderName = mode == VrMode.VR ? AppConstants.VrFolderName : AppConstants.NonVrFolderName;
            return Path.Combine(_workingRootPath, folderName);
        }

        #endregion
    }
}
