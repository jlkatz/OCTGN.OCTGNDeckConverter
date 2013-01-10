using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.ViewModel
{
    public class WizardPage_ChooseDeckSourceType : ImportDeckWizardPageVM
    {
        public WizardPage_ChooseDeckSourceType(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        { }

        #region Commands

        #region Choose DeckSourceType Command
        CommandViewModel<DeckSourceTypes> _ChooseDeckSourceTypeCommand;
        public CommandViewModel<DeckSourceTypes> ChooseDeckSourceTypeCommand
        {
            get
            {
                if (_ChooseDeckSourceTypeCommand == null)
                {
                    _ChooseDeckSourceTypeCommand = new CommandViewModel<DeckSourceTypes>
                    (
                        "Deck Source Type",
                        new RelayCommand<DeckSourceTypes>
                        (
                            (deckSourceType) =>
                            {
                                this.ImportDeckWizardVM.Converter.DeckSourceType = deckSourceType;
                                this.ImportDeckWizardVM.MoveToNextStep();
                            }
                        )
                    );
                }
                return _ChooseDeckSourceTypeCommand;
            }
        }
        #endregion Choose DeckSourceType Command
        
        #endregion Commands

        #region WizardPageVM Overrides

        public override bool ShowNextStepCommand
        {
            get { return false; }
        }

        public override bool ShowStartOverCommand
        {
            get { return false; }
        }

        public override string Title
        {
            get { return "Choose Deck Source"; }
        }

        #endregion WizardPageVM Overrides
    }
}
