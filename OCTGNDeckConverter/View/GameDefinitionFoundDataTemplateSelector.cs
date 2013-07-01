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

namespace OCTGNDeckConverter.View
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
        /// Returns the DataTemplate corresponding to whether the ConverterDatabase was able to find the Game or not.
        /// </summary>
        /// <param name="item">The parameter is not used.</param>
        /// <param name="container">The parameter is not used.</param>
        /// <returns>The DataTemplate corresponding to whether the ConverterDatabase was able to find the Game or not.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                ViewModel.WizardPage_ChooseDeckSourceType vm = item as ViewModel.WizardPage_ChooseDeckSourceType;
                if (vm != null)
                {
                    if (vm.ImportDeckWizardVM.Converter.ConverterGame != null)
                    {
                        return this.FoundTemplate;
                    }
                }
            }

            return this.NotFoundTemplate;
        }
    }
}