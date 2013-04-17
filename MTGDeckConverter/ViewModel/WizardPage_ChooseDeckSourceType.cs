// -----------------------------------------------------------------------
// <copyright file="WizardPage_ChooseDeckSourceType.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents a step in the Import Deck Wizard to be displayed by a View which
    /// allows the user to choose the Deck Source Type.
    /// </summary>
    public class WizardPage_ChooseDeckSourceType : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_ChooseDeckSourceType class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        public WizardPage_ChooseDeckSourceType(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        { 
        }

        #region Commands

        #region Choose DeckSourceType Command

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel<DeckSourceTypes> _ChooseDeckSourceTypeCommand;

        /// <summary>
        /// Gets the Command who's parameter dictates what the Deck Source Type will be
        /// </summary>
        public CommandViewModel<DeckSourceTypes> ChooseDeckSourceTypeCommand
        {
            get
            {
                if (this._ChooseDeckSourceTypeCommand == null)
                {
                    this._ChooseDeckSourceTypeCommand = new CommandViewModel<DeckSourceTypes>
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

                return this._ChooseDeckSourceTypeCommand;
            }
        }
        #endregion Choose DeckSourceType Command
        
        #endregion Commands

        #region WizardPageVM Overrides

        /// <summary>
        /// Gets a value indicating whether a View should show the Next Step command or not
        /// </summary>
        public override bool ShowNextStepCommand
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether a View should show the Start Over command or not
        /// </summary>
        public override bool ShowStartOverCommand
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the Title for this Page that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Choose Deck Source"; }
        }

        #endregion WizardPageVM Overrides
    }
}
