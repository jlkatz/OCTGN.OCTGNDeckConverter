// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="TODO">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
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
            this.InitializeComponent();

            // When the MainWindow is closed, call Cleanup on the ViewModelLocator
            this.Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _WindowTitle;

        /// <summary>
        /// Gets the title of the Window
        /// </summary>
        public string WindowTitle
        {
            get
            {
                if (this._WindowTitle == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var version = assembly.GetName().Version;
                    this._WindowTitle = "MTG Deck Converter (for OCTGN 3) v" + version.Major + "." + version.Minor + "." + version.Build;

                    // Uncomment when making in-development builds.
                    ////this._WindowTitle = this._WindowTitle + " (Development Build)";
                }

                return this._WindowTitle;
            }
        }
    }
}