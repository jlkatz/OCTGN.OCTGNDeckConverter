// -----------------------------------------------------------------------
// <copyright file="CommandViewModel{T}.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{    
    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    /// <typeparam name="T">The type of the Command Parameter to be used with this</typeparam>
    public class CommandViewModel<T> : CommandViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandViewModel{T}"/> class.  
        /// </summary>
        /// <param name="displayName">The name displayed by a View</param>
        /// <param name="command">The RelayCommand object to execute</param>
        public CommandViewModel(string displayName, RelayCommand<T> command)
            : base(displayName, command)
        { 
        }
    }
}
