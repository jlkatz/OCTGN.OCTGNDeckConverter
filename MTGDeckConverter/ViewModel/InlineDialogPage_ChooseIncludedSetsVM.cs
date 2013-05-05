// -----------------------------------------------------------------------
// <copyright file="InlineDialogPage_ChooseIncludedSetsVM.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents dialog content to be displayed by a View which prompts the user 
    /// to select which Sets are to be included in searches for potentially matching Cards
    /// </summary>
    public class InlineDialogPage_ChooseIncludedSetsVM : InlineDialogPageVM
    {
        /// <summary>
        /// Gets a List of ConverterSet objects ordered by MaxMultiverseID descending
        /// </summary>
        public List<ConverterSet> Sets
        {
            get
            {
                return (
                    from set in ConverterDatabase.SingletonInstance.Sets.Values
                    orderby set.MaxMultiverseID descending
                    select set
                    ).ToList();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Cancel Button should be shown by a View
        /// </summary>
        public override bool CancelButtonVisible
        { 
            get { return false; } 
        }

        /// <summary>
        /// Gets the Title for this dialog that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Choose Included Sets When Searching for Cards"; }
        }
    }
}
