// -----------------------------------------------------------------------
// <copyright file="INotifyPropertyChangedBase.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// A base class for object which implement INotifyPropertyChanged with some helper methods
    /// </summary>
    public abstract class INotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// This Event is fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handler for Property Changed event
        /// </summary>
        /// <param name="name">The property name</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Setter helper method, this should be called whenever setting a property which should fire PropertyChanged events
        /// </summary>
        /// <typeparam name="T">Any object</typeparam>
        /// <param name="property">Property that is being changed (by reference)</param>
        /// <param name="value">New property value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property was changed and event fired, false if old and new values are equal.</returns>
        /// <seealso cref="http://www.pochet.net/blog/2010/06/25/inotifypropertychanged-implementations-an-overview/"/>
        protected bool SetValue<T>(ref T property, T value, string propertyName)
        {
            if (object.Equals(property, value))
            {
                return false;
            }

            property = value;

            this.OnPropertyChanged(propertyName);

            return true;
        }

        #endregion
    }
}
