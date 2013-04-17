// -----------------------------------------------------------------------
// <copyright file="WizardPage_SelectFile.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents a step in the Import Deck Wizard to be displayed by a View which
    /// allows the user to select the file on their system which is a Deck in another format.
    /// </summary>
    public class WizardPage_SelectFile : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_SelectFile class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        public WizardPage_SelectFile(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        {
            this.CanMoveToNextStep = false;
        }

        #region Commands

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _SelectDeckCommand;

        /// <summary>
        /// Gets the Command which will show an OpenFileDialog so the user can select the file on their system which is a Deck in another format.
        /// </summary>
        public CommandViewModel SelectDeckCommand
        {
            get
            {
                if (this._SelectDeckCommand == null)
                {
                    this._SelectDeckCommand = new CommandViewModel
                    (
                        "Select Deck...",
                        new RelayCommand
                        (
                            () =>
                            {
                                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                                // Attempt to set the initial directory if it exists
                                // (It might not exist if the last location was a USB stick for example)
                                if (System.IO.Directory.Exists(Model.SettingsManager.SingletonInstance.OpenFileDirectory))
                                {
                                    dlg.InitialDirectory = Model.SettingsManager.SingletonInstance.OpenFileDirectory;
                                }

                                dlg.Multiselect = false;

                                bool? dlgresult = dlg.ShowDialog();

                                // If the user did not cancel the Open File Dialog
                                if (dlgresult.HasValue && dlgresult.Value == true)
                                {
                                    this.ImportDeckWizardVM.Converter.DeckFullPathName = dlg.FileName;
                                    this.ImportDeckWizardVM.Converter.DeckFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                                    Model.SettingsManager.SingletonInstance.OpenFileDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);
                                    this.CanMoveToNextStep = true;
                                }
                            }

                        )
                    );
                }

                return this._SelectDeckCommand;
            }
        }

        #endregion Commands

        #region WizardPageVM Overrides

        /// <summary>
        /// Gets a value indicating whether a View should show the Next Step command or not
        /// </summary>
        public override bool ShowNextStepCommand
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether a View should show the Start Over command or not
        /// </summary>
        public override bool ShowStartOverCommand
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the Title for this Page that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Select File"; }
        }

        #endregion WizardPageVM Overrides
    }
}
