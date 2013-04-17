// -----------------------------------------------------------------------
// <copyright file="WizardPage_EnterWebpage.cs" company="jlkatz">
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
    /// allows the user to enter the web URL of the location of the Deck the wish to import.
    /// </summary>
    public class WizardPage_EnterWebpage : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_EnterWebpage class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        public WizardPage_EnterWebpage(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        {
            this.CanMoveToNextStep = false;
        }

        #region Public Properties

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string URLPropertyName = "URL";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _URL;

        /// <summary>
        /// Gets or sets the web URL location that the user enters from the View
        /// </summary>
        public string URL
        {
            get 
            { 
                return this._URL; 
            }

            set 
            {
                if (this.SetValue(ref this._URL, value, URLPropertyName))
                {
                    this.ImportDeckWizardVM.Converter.DeckURL = this._URL;
                    this.CanMoveToNextStep = !string.IsNullOrWhiteSpace(this._URL);
                }
            }
        }

        #endregion Public Properties

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
            get { return "Enter Webpage URL"; }
        }

        #endregion WizardPageVM Overrides
    }
}
