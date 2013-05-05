// -----------------------------------------------------------------------
// <copyright file="GameDefinitionFoundDataTemplateSelector.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.View
{
    /// <summary>
    /// Provides a way to choose a DataTemplate based on whether the ConverterDatabase was able to find the GameDefinition or not.
    /// </summary>
    public class GameDefinitionFoundDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the DataTemplate to be used if the GameDefinition is found.
        /// </summary>
        public DataTemplate FoundTemplate
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the DataTemplate to be used if the GameDefinition is not found.
        /// </summary>
        public DataTemplate NotFoundTemplate
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Returns the DataTemplate corresponding to whether the ConverterDatabase was able to find the GameDefinition or not.
        /// </summary>
        /// <param name="item">The parameter is not used.</param>
        /// <param name="container">The parameter is not used.</param>
        /// <returns>The DataTemplate corresponding to whether the ConverterDatabase was able to find the GameDefinition or not.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return ConverterDatabase.SingletonInstance.GameDefinition != null ?
                this.FoundTemplate :
                this.NotFoundTemplate;
        }
    }
}