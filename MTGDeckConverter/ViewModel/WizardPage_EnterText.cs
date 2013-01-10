using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{
    class WizardPage_EnterText : ImportDeckWizardPageVM
    {
        public WizardPage_EnterText(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        {
            this.CanMoveToNextStep = false;
        }

        #region Public Properties

        internal const string MainDeckTextPropertyName = "MainDeckText";
        private string _MainDeckText;
        public string MainDeckText
        {
            get { return _MainDeckText; }
            set
            {
                if (this.SetValue(ref _MainDeckText, value, MainDeckTextPropertyName))
                {
                    this.ImportDeckWizardVM.Converter.MainDeckText = _MainDeckText;
                    this.CanMoveToNextStep = !string.IsNullOrWhiteSpace(_MainDeckText);
                }
            }
        }

        internal const string SideBoardTextPropertyName = "SideBoardText";
        private string _SideBoardText;
        public string SideBoardText
        {
            get { return _SideBoardText; }
            set
            {
                if (this.SetValue(ref _SideBoardText, value, SideBoardTextPropertyName))
                {
                    this.ImportDeckWizardVM.Converter.SideBoardText = _SideBoardText;
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
            get { return "Enter Text"; }
        }

        #endregion WizardPageVM Overrides
    }
}
