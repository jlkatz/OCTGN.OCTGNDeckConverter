// -----------------------------------------------------------------------
// <copyright file="InlineDialogPage_ConvertingCardsVM.cs" company="jlkatz">
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
    /// Represents dialog content to be displayed by a View which shows that
    /// the conversion is currently in progress.
    /// </summary>
    public class InlineDialogPage_ConvertingCardsVM : InlineDialogPageVM
    {
        /// <summary>
        /// Gets a value indicating whether the Cancel Button should be shown by a View
        /// </summary>
        public override bool CancelButtonVisible
        { 
            get { return false; } 
        }

        /// <summary>
        /// Gets a value indicating whether the Ok Button should be shown by a View
        /// </summary>
        public override bool OkButtonVisible
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the Title for this dialog that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Converting Cards"; }
        }
    }
}
