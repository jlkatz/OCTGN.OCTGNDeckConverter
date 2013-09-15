// -----------------------------------------------------------------------
// <copyright file="ImportDeckWizardPageVM.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.ViewModel
{
    /// <summary>
    /// A base class for all Wizard page ViewModels.  Provides navigation framework.
    /// </summary>
    public abstract class ImportDeckWizardPageVM : Model.INotifyPropertyChangedBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportDeckWizardPageVM class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Import Deck Wizard ViewModel</param>
        public ImportDeckWizardPageVM(ImportDeckWizardVM importDeckWizardVM)
        {
            if (importDeckWizardVM == null)
            {
                throw new ArgumentNullException();
            }

            this.ImportDeckWizardVM = importDeckWizardVM;
        }

        /// <summary>
        /// Gets the parent ImportDeckWizardVM
        /// </summary>
        public ImportDeckWizardVM ImportDeckWizardVM
        {
            get;
            private set;
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string CanMoveToNextStepPropertyName = "CanMoveToNextStep";

        /// <summary>
        /// Private backing field
        /// </summary>
        private bool _CanMoveToNextStep = true;
        
        /// <summary>
        /// Gets or sets a value indicating whether the Wizard can move from this Page to the next or not
        /// </summary>
        public bool CanMoveToNextStep
        {
            get { return this._CanMoveToNextStep; }
            protected set { this.SetValue(ref this._CanMoveToNextStep, value, CanMoveToNextStepPropertyName); }
        }

        /// <summary>
        /// Gets a value indicating whether this Page is the first page shown of the Wizard or not
        /// </summary>
        public abstract bool IsStartPage { get; }

        /// <summary>
        /// Gets a value indicating whether a View should show the Next Step command or not
        /// </summary>
        public abstract bool ShowNextStepCommand { get; }

        /// <summary>
        /// Gets a value indicating whether a View should show the Start Over command or not
        /// </summary>
        public abstract bool ShowStartOverCommand { get; }

        /// <summary>
        /// Gets the Subtitle for this Page that should be shown by a View
        /// </summary>
        public virtual string Subtitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the Title for this Page that should be shown by a View
        /// </summary>
        public virtual string Title
        {
            get { return string.Empty; }
        }
    }
}
