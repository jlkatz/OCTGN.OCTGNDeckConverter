// -----------------------------------------------------------------------
// <copyright file="WizardPage_EnterText.cs" company="jlkatz">
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
    /// allows the user to manually type or paste text to be parsed into Cards.
    /// </summary>
    public class WizardPage_EnterText : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_EnterText class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        public WizardPage_EnterText(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        {
            this.CanMoveToNextStep = false;
        }

        #region Public Properties

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string MainDeckTextPropertyName = "MainDeckText";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _MainDeckText;

        /// <summary>
        /// Gets or sets the Main Deck text that the user enters from a View
        /// </summary>
        public string MainDeckText
        {
            get 
            { 
                return this._MainDeckText; 
            }

            set
            {
                if (this.SetValue(ref this._MainDeckText, value, MainDeckTextPropertyName))
                {
                    this.ImportDeckWizardVM.Converter.MainDeckText = this._MainDeckText;
                    this.CanMoveToNextStep = !string.IsNullOrWhiteSpace(this._MainDeckText);
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string SideBoardTextPropertyName = "SideBoardText";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _SideBoardText;

        /// <summary>
        /// Gets or sets the Sideboard text that the user enters from a View
        /// </summary>
        public string SideBoardText
        {
            get 
            {
                return this._SideBoardText; 
            }

            set
            {
                if (this.SetValue(ref this._SideBoardText, value, SideBoardTextPropertyName))
                {
                    this.ImportDeckWizardVM.Converter.SideBoardText = this._SideBoardText;
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
            get { return "Enter Text"; }
        }

        #endregion WizardPageVM Overrides
    }
}
