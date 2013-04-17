// -----------------------------------------------------------------------
// <copyright file="InlineDialogVM.cs" company="jlkatz">
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
    /// A class to represent a dialog to be displayed by a View 
    /// </summary>
    public class InlineDialogVM : Model.INotifyPropertyChangedBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the InlineDialogVM class.
        /// </summary>
        /// <param name="inlineDialogPageVM">The content to be shown by the View inside this Dialog</param>
        public InlineDialogVM(InlineDialogPageVM inlineDialogPageVM)
        {
            if (inlineDialogPageVM == null)
            { 
                throw new ArgumentNullException(); 
            }

            this.InlineDialogPage = inlineDialogPageVM;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets or sets the delegate that will be executed when the Inline Dialog is closed.
        /// </summary>
        public CompletedCallback CallWhenCompletedMethod
        {
            get;
            set;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string CompletedPropertyName = "Completed";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private bool _Completed = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user has clicked a command to close the Inline Dialog or not.  (Such as Ok, Cancel, etc)
        /// </summary>
        public bool Completed
        {
            get { return this._Completed; }
            protected set { this.SetValue(ref this._Completed, value, CompletedPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string WasNotCancelledPropertyName = "WasNotCancelled";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private bool _WasNotCancelled = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user cancelled the Inline Dialog when closing it or not.
        /// </summary>
        public bool WasNotCancelled
        {
            get { return this._WasNotCancelled; }
            protected set { this.SetValue(ref this._WasNotCancelled, value, WasNotCancelledPropertyName); }
        }

        /// <summary>
        /// Gets the Page that contains the content to be displayed by a View 
        /// </summary>
        public InlineDialogPageVM InlineDialogPage
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Commands

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _CancelButtonCommand;

        /// <summary>
        /// Gets the Command which instructs the Inline Dialog to close by cancellation 
        /// </summary>
        public CommandViewModel CancelButtonCommand
        {
            get
            {
                if (this._CancelButtonCommand == null)
                {
                    this._CancelButtonCommand = new CommandViewModel
                    (
                        this.InlineDialogPage.CancelButtonText,
                        new RelayCommand
                        (
                            () => this.Close(false),
                            () => this.InlineDialogPage.CancelButtonEnabled
                        )
                    );
                }

                return this._CancelButtonCommand;
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _OkButtonCommand;

        /// <summary>
        /// Gets the Command which instructs the Inline Dialog to close by completion 
        /// </summary>
        public CommandViewModel OkButtonCommand
        {
            get
            {
                if (this._OkButtonCommand == null)
                {
                    this._OkButtonCommand = new CommandViewModel
                    (
                        this.InlineDialogPage.OkButtonText,
                        new RelayCommand
                        (
                            () => this.Close(true),
                            () => this.InlineDialogPage.OkButtonEnabled
                        )
                    );
                }

                return this._OkButtonCommand;
            }
        }

        #endregion Commands

        #region Public Methods

        /// <summary>
        /// Represents a callback method that will handle the Completed event.
        /// </summary>
        /// <param name="inlineDialogVM">The source of the event.</param>
        public delegate void CompletedCallback(InlineDialogVM inlineDialogVM);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Closes the Inline Dialog, and executes the code specified to run after closing
        /// </summary>
        /// <param name="wasNotCancelled">A value indicating whether the Inline Dialog was _not_ cancelled when closed or not</param>
        private void Close(bool wasNotCancelled)
        {
            this.WasNotCancelled = wasNotCancelled;

            if (this.CallWhenCompletedMethod != null)
            {
                this.CallWhenCompletedMethod(this);
            }

            this.Completed = true;
        }

        #endregion Private Methods
    }
}
