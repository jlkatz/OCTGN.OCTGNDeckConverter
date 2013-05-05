// -----------------------------------------------------------------------
// <copyright file="CommandViewModelBase.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class CommandViewModelBase : Model.INotifyPropertyChangedBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommandViewModelBase class.  
        /// </summary>
        /// <param name="displayName">The name displayed by a View</param>
        /// <param name="command">The RelayCommand object to execute</param>
        public CommandViewModelBase(string displayName, ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            this.DisplayName = displayName;
            this.Command = command;
        }

        #endregion Constructor

        #region Public Properties

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string DisplayNamePropertyName = "DisplayName";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _DisplayName;

        /// <summary>
        /// Gets or sets the name displayed by a View
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this.SetValue(ref this._DisplayName, value, DisplayNamePropertyName); }
        }

        /// <summary>
        /// Gets or sets the Command to be executed
        /// </summary>
        public ICommand Command { get; protected set; }

        /// <summary>
        /// Gets or sets the Command Parameter
        /// </summary>
        public object CommandParameter { get; protected set; }

        #endregion Public Properties
    }
}
