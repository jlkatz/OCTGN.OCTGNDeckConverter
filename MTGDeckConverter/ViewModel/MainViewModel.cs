// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="TODO">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this._ImportDeckWizardVM = new ImportDeckWizardVM();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ImportDeckWizardVM _ImportDeckWizardVM;

        /// <summary>
        /// Gets the ImportDeckWizardVM instance, which cannot be changed for the life of MainViewModel
        /// </summary>
        public ImportDeckWizardVM ImportDeckWizardVM
        {
            get { return this._ImportDeckWizardVM; }
        }

        /// <summary>
        /// Cleans up resources and saves settings before quitting
        /// </summary>
        public override void Cleanup()
        {
            ConverterDatabase.SingletonInstance.Cleanup();
            SettingsManager.SingletonInstance.SaveSettingsManager();
            base.Cleanup();
        }
    }
}