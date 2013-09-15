// -----------------------------------------------------------------------
// <copyright file="WizardPage_EnterText.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace OCTGNDeckConverter.ViewModel
{
    /// <summary>
    /// Represents a step in the Import Deck Wizard to be displayed by a View which
    /// allows the user to manually type or paste text to be parsed into Cards.
    /// </summary>
    public class WizardPage_EnterText : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Private backing field
        /// </summary>
        private bool _IsStartPage;

        /// <summary>
        /// Initializes a new instance of the WizardPage_EnterText class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        /// <param name="isStartPage">A value indicating whether this page is the first page shown to the user or not</param>
        public WizardPage_EnterText(ImportDeckWizardVM importDeckWizardVM, bool isStartPage)
            : base(importDeckWizardVM)
        {
            this._IsStartPage = isStartPage;
            this.CanMoveToNextStep = false;
            foreach (string sectionName in importDeckWizardVM.Converter.ConverterGame.DeckSectionNames)
            {
                SectionText sectionText = new SectionText(sectionName);
                sectionText.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.SectionText_PropertyChanged);
                this._SectionsText.Add(sectionText);
            }
        }

        /// <summary>
        /// Fired when a property on any of the SectionText objects changes.  It re-evaluates if any sections have text,
        /// and then decides if the user can move to the next step based on this.
        /// </summary>
        /// <param name="sender">The sender of the Event</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs property</param>
        private void SectionText_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ImportDeckWizardVM.Converter.SectionsText.Clear();

            // Set the text of each section in the Converter
            foreach (SectionText sectionText in this.SectionsText)
            {
                this.ImportDeckWizardVM.Converter.SectionsText[sectionText.SectionName] = sectionText.Text;
            }

            // Evaluate if we can move to the next step by checking if at least one section has text
            this.CanMoveToNextStep = this.ImportDeckWizardVM.Converter.SectionsText.Any(st => !string.IsNullOrWhiteSpace(st.Value));
        }

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this page is the first page shown to the user or not
        /// </summary>
        public override bool IsStartPage
        {
            get { return this._IsStartPage; }
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private List<SectionText> _SectionsText = new List<SectionText>();

        /// <summary>
        /// Gets a collection of SectionText objects, which each contain info about a Section and the text the user entered for it.
        /// </summary>
        public ReadOnlyCollection<SectionText> SectionsText
        {
            get { return this._SectionsText.AsReadOnly(); }
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
            get { return "Enter Text"; }
        }

        #endregion WizardPageVM Overrides
    }
}
