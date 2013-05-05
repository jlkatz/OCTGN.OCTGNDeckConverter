// -----------------------------------------------------------------------
// <copyright file="CommandViewModel.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : CommandViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CommandViewModel class.  
        /// </summary>
        /// <param name="displayName">The name displayed by a View</param>
        /// <param name="command">The RelayCommand object to execute</param>
        public CommandViewModel(string displayName, RelayCommand command)
            : base(displayName, command)
        { 
        }
    }
}
