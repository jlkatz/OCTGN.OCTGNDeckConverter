// -----------------------------------------------------------------------
// <copyright file="InlineDialogPage_MessageVM.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents dialog content to be displayed by a View which shows the user
    /// a message with title.
    /// </summary>
    public class InlineDialogPage_MessageVM : InlineDialogPageVM
    {
        /// <summary>
        /// Initializes a new instance of the InlineDialogPage_MessageVM class.
        /// It will display the message without any title.
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        public InlineDialogPage_MessageVM(string message)
            : this(message, string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the InlineDialogPage_MessageVM class.
        /// It will display the message with the title.
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="title">The title for this message</param>
        public InlineDialogPage_MessageVM(string message, string title)
        {
            this.Message = message;
            this._Title = title;
        }

        /// <summary>
        /// Gets the Message of this Dialog
        /// </summary>
        public string Message
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Private backing field of the Title to be shown on the Message Dialog
        /// </summary>
        private string _Title;

        /// <summary>
        /// Gets the Title of this Message Dialog
        /// </summary>
        public override string Title
        {
            get { return this._Title; }
        }

        /// <summary>
        /// Gets a value indicating whether the Cancel Button should be shown by a View
        /// </summary>
        public override bool CancelButtonVisible
        {
            get { return false; }
        }
    }
}
