using System.Windows;
using MTGDeckConverter.ViewModel;

namespace MTGDeckConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private string _WindowTitle;
        public string WindowTitle
        {
            get
            {
                if (_WindowTitle == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var version = assembly.GetName().Version;
                    _WindowTitle = "MTG Deck Converter (for OCTGN 3) v" + version.Major + "." + version.Minor + "." + version.Build;

                    //Uncomment when making in-development builds.
                    //_WindowTitle = _WindowTitle + " (Development Build)";
                }
                return _WindowTitle;
            }
        }

    }
}