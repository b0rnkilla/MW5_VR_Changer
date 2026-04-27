using Microsoft.Win32;
using MW5_VR_Changer.Enums;
using MW5_VR_Changer.Helpers;
using MW5_VR_Changer.Services.Interfaces;
using System.Windows;

namespace MW5_VR_Changer.Services
{
    /// <summary>Implementiert einfache Datei- und Bestätigungsdialoge für die UI.</summary>
    internal sealed class FileDialogService : IFileDialogService
    {
        #region Fields

        /// <summary>Standardfilter, um notfalls alle Dateien auswählen zu können.</summary>
        private const string AllFilesFilter = "All files|*.*";

        #endregion

        #region Methods & Events

        /// <inheritdoc/>
        public string? PickModListFile()
        {
            return PickFile(AppConstants.ModListFileName, $"{AppConstants.ModListFileName}|{AppConstants.ModListFileName}|{AllFilesFilter}");
        }

        /// <inheritdoc/>
        public string? PickGameIniFile()
        {
            return PickFile(AppConstants.GameIniFileName, $"{AppConstants.GameIniFileName}|{AppConstants.GameIniFileName}|{AllFilesFilter}");
        }

        /// <inheritdoc/>
        public string? PickGameUserSettingsFile()
        {
            return PickFile(AppConstants.GameUserSettingsFileName, $"{AppConstants.GameUserSettingsFileName}|{AppConstants.GameUserSettingsFileName}|{AllFilesFilter}");
        }

        /// <inheritdoc/>
        public VrMode? ConfirmCurrentOriginalMode()
        {
            var result = MessageBox.Show(
                "Bitte bestätigen: Enthalten die aktuell aktiven Originaldateien VR-Einstellungen?\n\nJa = VR\nNein = NonVR",
                "Modus bestätigen",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            return result switch
            {
                MessageBoxResult.Yes => VrMode.VR,
                MessageBoxResult.No => VrMode.NonVR,
                _ => null
            };
        }

        /// <summary>Öffnet einen Dateiauswahldialog mit Filter auf einen erwarteten Dateinamen.</summary>
        /// <param name="expectedFileName">Dateiname, auf den der Dialog primär gefiltert wird.</param>
        /// <param name="filter">OpenFileDialog-Filterstring.</param>
        /// <returns>Gewählter Pfad oder <c>null</c>, wenn abgebrochen wurde.</returns>
        private static string? PickFile(string expectedFileName, string filter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = $"Select {expectedFileName}"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        #endregion
    }
}
