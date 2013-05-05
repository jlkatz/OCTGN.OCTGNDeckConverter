// -----------------------------------------------------------------------
// <copyright file="InlineDialogPageVM.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// A base class to represent dialog content to be displayed by a View 
    /// </summary>
    public abstract class InlineDialogPageVM : Model.INotifyPropertyChangedBase
    {
        /// <summary>
        /// Gets the text of the Cancel Button to be shown by a View
        /// </summary>
        internal virtual string CancelButtonText 
        { 
            get { return "Cancel"; } 
        }

        /// <summary>
        /// Gets a value indicating whether the Cancel Button should be enabled on a View
        /// </summary>
        public virtual bool CancelButtonEnabled 
        { 
            get { return true; } 
        }

        /// <summary>
        /// Gets a value indicating whether the Cancel Button should be shown by a View
        /// </summary>
        public virtual bool CancelButtonVisible 
        { 
            get { return true; } 
        }

        /// <summary>
        /// Gets the text of the Ok Button to be shown by a View
        /// </summary>
        internal virtual string OkButtonText 
        { 
            get { return "Ok"; } 
        }

        /// <summary>
        /// Gets a value indicating whether the Ok Button should be enabled on a View
        /// </summary>
        public virtual bool OkButtonEnabled 
        { 
            get { return true; } 
        }

        /// <summary>
        /// Gets a value indicating whether the Ok Button should be shown by a View
        /// </summary>
        public virtual bool OkButtonVisible 
        { 
            get { return true; } 
        }

        /// <summary>
        /// Gets the Title of this Message Dialog
        /// </summary>
        public abstract string Title 
        { 
            get; 
        }
    }
}
