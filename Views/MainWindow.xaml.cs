using System.Windows;
using MW5_VR_Changer.ViewModels;

namespace MW5_VR_Changer
{
    /// <summary>Hauptfenster der Anwendung (View), das an das `MainViewModel` gebunden ist.</summary>
    public partial class MainWindow : Window
    {
        /// <summary>Initialisiert das Hauptfenster und setzt das ViewModel als `DataContext`.</summary>
        /// <param name="viewModel">Zentrales ViewModel der Anwendung.</param>
        public MainWindow(MainViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            InitializeComponent();
            DataContext = viewModel;
        }
    }
}