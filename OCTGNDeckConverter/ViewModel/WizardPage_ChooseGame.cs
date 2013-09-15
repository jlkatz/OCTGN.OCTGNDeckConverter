// -----------------------------------------------------------------------
// <copyright file="WizardPage_ChooseGame.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace OCTGNDeckConverter.ViewModel
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WizardPage_ChooseGame : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_ChooseGame class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        public WizardPage_ChooseGame(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        { 
        }

        #region Commands

        /// <summary>
        /// The private backing field for ChooseGameCommand
        /// </summary>
        private RelayCommand<Octgn.DataNew.Entities.Game> _ChooseGameCommand;

        /// <summary>
        /// Gets the Command who's parameter dictates what the Deck Source Type will be
        /// </summary>
        public RelayCommand<Octgn.DataNew.Entities.Game> ChooseGameCommand
        {
            get
            {
                if (this._ChooseGameCommand == null)
                {
                    this._ChooseGameCommand = new RelayCommand<Octgn.DataNew.Entities.Game>
                    (
                        (game) =>
                        {
                            this.ImportDeckWizardVM.Converter.ConverterGame = Model.ConverterDatabase.SingletonInstance.GetConverterGame(game);
                            this.ImportDeckWizardVM.MoveToNextStep();
                        }

                    );
                }

                return this._ChooseGameCommand;
            }
        }

        #endregion Commands

        #region WizardPageVM Overrides

        /// <summary>
        /// Gets a value indicating whether this page is the first page shown to the user or not
        /// </summary>
        public override bool IsStartPage
        {
            get { return true; }
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
            get { return false; }
        }

        /// <summary>
        /// Gets the Title for this Page that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Choose Game"; }
        }

        #endregion WizardPageVM Overrides
    }
}
