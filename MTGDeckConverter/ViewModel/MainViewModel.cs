using GalaSoft.MvvmLight;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _ImportDeckWizardVM = new ImportDeckWizardVM();
        }

        private ImportDeckWizardVM _ImportDeckWizardVM;
        public ImportDeckWizardVM ImportDeckWizardVM
        {
            get { return _ImportDeckWizardVM; }
        }

        public override void Cleanup()
        {
            ConverterDatabase.SingletonInstance.Cleanup();

            SettingsManager.SingletonInstance.SaveSettingsManager();

            base.Cleanup();
        }
    }
}