using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{
    public class WizardPage_EnterWebpage : ImportDeckWizardPageVM
    {
        public WizardPage_EnterWebpage(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        {
            this.CanMoveToNextStep = false;
        }

        #region Public Properties

        internal const string URLPropertyName = "URL";
        private string _URL;
        public string URL
        {
            get { return _URL; }
            set 
            {
                if (this.SetValue(ref _URL, value, URLPropertyName))
                {
                    this.ImportDeckWizardVM.Converter.DeckURL = _URL;
                    this.CanMoveToNextStep = !string.IsNullOrWhiteSpace(_URL);
                }
            }
        }

        #endregion Public Properties

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
            get { return "Enter Webpage URL"; }
        }

        #endregion WizardPageVM Overrides
    }
}
