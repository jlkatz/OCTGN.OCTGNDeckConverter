// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
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
        /// Backing store for the DataContext, which should always be of type ImportDeckWizardVM
        /// </summary>
        private ImportDeckWizardVM _ImportDeckWizardVM;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.MainWindow_DataContextChanged);
        }

        /// <summary>
        /// Fired when the DataContext of MainWindow changes.  It will store a reference to the DataContext if it is
        /// of type ImportDeckWizardVM, and subscribe to it's Close event.  Previous DataContexts will be unsubscribed.
        /// </summary>
        /// <param name="sender">The sender of the Event</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs property</param>
        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this._ImportDeckWizardVM != null)
            {
                this._ImportDeckWizardVM.Close -= new System.EventHandler(this.ImportDeckWizardVM_Close);
            }

            this._ImportDeckWizardVM = null;

            if (e.NewValue != null && e.NewValue is ImportDeckWizardVM)
            {
                this._ImportDeckWizardVM = e.NewValue as ImportDeckWizardVM;
                this._ImportDeckWizardVM.Close += new System.EventHandler(this.ImportDeckWizardVM_Close);
            }
        }

        /// <summary>
        /// This method is called via subscription when the DataContext (ImportDeckWizardVM)'s Close event fires
        /// </summary>
        /// <param name="sender">The sender of the Event, which should be _ImportDeckWizardVM.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs property</param>
        private void ImportDeckWizardVM_Close(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Gets the MainViewModel which serves as the DataContext for this Window
        /// </summary>
        public ImportDeckWizardVM ImportDeckWizardVM
        {
            get { return this._ImportDeckWizardVM; }
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
                    this._WindowTitle = "MTG Deck Converter (for OCTGN 3.1) v" + version.Major + "." + version.Minor + "." + version.Build;

                    // Uncomment when making in-development builds.
                    ////this._WindowTitle = this._WindowTitle + " (Development Build)";
                }

                return this._WindowTitle;
            }
        }
    }
}