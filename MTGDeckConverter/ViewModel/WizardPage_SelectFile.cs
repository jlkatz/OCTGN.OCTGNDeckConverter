using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{
    public class WizardPage_SelectFile : ImportDeckWizardPageVM
    {
        public WizardPage_SelectFile(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        {
            this.CanMoveToNextStep = false;
        }

        #region Commands

        #region Select Deck Command
        private CommandViewModel _SelectDeckCommand;
        public CommandViewModel SelectDeckCommand
        {
            get
            {
                if (_SelectDeckCommand == null)
                {
                    _SelectDeckCommand = new CommandViewModel
                    (
                        "Select Deck...",
                        new RelayCommand
                        (
                            () =>
                            {
                                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                                //Attempt to set the initial directory if it exists
                                //(It might not exist if the last location was a USB stick for example)
                                if (System.IO.Directory.Exists(Model.SettingsManager.SingletonInstance.OpenFileDirectory))
                                {
                                    dlg.InitialDirectory = Model.SettingsManager.SingletonInstance.OpenFileDirectory;
                                }

                                dlg.Multiselect = false;

                                bool? dlgresult = dlg.ShowDialog();

                                //If the user did not cancel the Open File Dialog
                                if (dlgresult.HasValue && dlgresult.Value == true)
                                {
                                    this.ImportDeckWizardVM.Converter.DeckFullPathName = dlg.FileName;
                                    this.ImportDeckWizardVM.Converter.DeckName = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                                    Model.SettingsManager.SingletonInstance.OpenFileDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);
                                    this.CanMoveToNextStep = true;
                                }
                            }
                        )
                    );
                }
                return _SelectDeckCommand;
            }
        }
        #endregion Select Deck Command

        #endregion Commands

        #region WizardPageVM Overrides

        public override bool ShowNextStepCommand
        {
            get { return true; }
        }

        public override bool ShowStartOverCommand
        {
            get { return true; }
        }

        public override string Title
        {
            get { return "Select File"; }
        }

        #endregion WizardPageVM Overrides
    }
}
