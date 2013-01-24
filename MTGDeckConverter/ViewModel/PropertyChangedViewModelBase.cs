// -----------------------------------------------------------------------
// <copyright file="PropertyChangedViewModelBase.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Octgn.MTGDeckConverter.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// A base class for ViewModel objects which implement INotifyPropertyChanged with some helper methods
    /// </summary>
    public class PropertyChangedViewModelBase : ViewModelBase
    {   
        /// <summary>
        /// Setter helper method, this should be called whenever setting a property which should fire PropertyChanged events
        /// </summary>
        /// <typeparam name="T">Any object</typeparam>
        /// <param name="property">Property that is being changed (by reference)</param>
        /// <param name="value">New property value</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="broadcast">Indicates whether this property change should be broadcast using the MVVM Light Framework's Messaging feature (false by default)</param>
        /// <returns>True if the property was changed and event fired, false if old and new values are equal.</returns>
        /// <seealso cref="http://www.pochet.net/blog/2010/06/25/inotifypropertychanged-implementations-an-overview/"/>
        protected bool SetValue<T>(ref T property, T value, string propertyName, bool broadcast = false)
        {
            if (object.Equals(property, value))
            {
                return false;
            }

            var oldValue = property;
            property = value;

            this.RaisePropertyChanged<T>(propertyName, oldValue, value, broadcast);

            return true;
        }
    }
}
