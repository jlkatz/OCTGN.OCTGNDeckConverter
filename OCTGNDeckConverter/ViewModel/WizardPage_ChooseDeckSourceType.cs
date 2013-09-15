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
using OCTGNDeckConverter.Model;

namespace OCTGNDeckConverter.ViewModel
{
    /// <summary>
    /// Represents a step in the Import Deck Wizard to be displayed by a View which
    /// allows the user to choose the Deck Source Type.
    /// </summary>
    public class WizardPage_ChooseDeckSourceType : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Private backing field
        /// </summary>
        private bool _IsStartPage;

        /// <summary>
        /// Initializes a new instance of the WizardPage_ChooseDeckSourceType class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        /// <param name="allowFileSource">A value indicating whether this page should allow choosing the deck source from a file or not</param>
        /// <param name="allowWebpageSource">A value indicating whether this page should allow choosing the deck source from a webpage or not</param>
        /// <param name="isStartPage">A value indicating whether this page is the first page shown to the user or not</param>
        public WizardPage_ChooseDeckSourceType(ImportDeckWizardVM importDeckWizardVM, bool allowFileSource, bool allowWebpageSource, bool isStartPage)
            : base(importDeckWizardVM)
        {
            this._IsStartPage = isStartPage;
            this.AllowFileSource = allowFileSource;
            this.AllowWebpageSource = allowWebpageSource;
        }

        #region Commands

        #region Choose DeckSourceType Command

        /// <summary>
        /// Private backing field
        /// </summary>
        private RelayCommand<DeckSourceTypes> _ChooseDeckSourceTypeCommand;

        /// <summary>
        /// Gets the Command who's parameter dictates what the Deck Source Type will be
        /// </summary>
        public RelayCommand<DeckSourceTypes> ChooseDeckSourceTypeCommand
        {
            get
            {
                if (this._ChooseDeckSourceTypeCommand == null)
                {
                    this._ChooseDeckSourceTypeCommand = new RelayCommand<DeckSourceTypes>
                    (
                        (deckSourceType) =>
                        {
                            this.ImportDeckWizardVM.Converter.DeckSourceType = deckSourceType;
                            this.ImportDeckWizardVM.MoveToNextStep();
                        }

                    );
                }

                return this._ChooseDeckSourceTypeCommand;
            }
        }
        #endregion Choose DeckSourceType Command
        
        #endregion Commands

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this page should allow choosing the deck source from a file or not
        /// </summary>
        public bool AllowFileSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this page should allow choosing the deck source from a webpage or not
        /// </summary>
        public bool AllowWebpageSource
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region WizardPageVM Overrides

        /// <summary>
        /// Gets a value indicating whether this page is the first page shown to the user or not
        /// </summary>
        public override bool IsStartPage
        {
            get { return this._IsStartPage; }
        }

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
            get { return !this.IsStartPage; }
        }

        /// <summary>
        /// Gets the Subtitle for this Page that should be shown by a View
        /// </summary>
        public override string Subtitle
        {
            get { return this.ImportDeckWizardVM.Converter.ConverterGame.Game.Name; }
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
