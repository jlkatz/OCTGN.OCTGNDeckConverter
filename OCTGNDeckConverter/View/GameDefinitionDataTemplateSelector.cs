// -----------------------------------------------------------------------
// <copyright file="GameDefinitionDataTemplateSelector.cs" company="jlkatz">
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
    /// Provides a way to choose a DataTemplate based on whether the Game ID (Guid).
    /// </summary>
    public class GameDefinitionDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the DataTemplate to be used if the GameDefinition is MTG.
        /// </summary>
        public DataTemplate MTGTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DataTemplate to be used if the GameDefinition is LoTR.
        /// </summary>
        public DataTemplate LoTRTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DataTemplate to be used if the GameDefinition is MW.
        /// </summary>
        public DataTemplate MWTemplate
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
                Guid gameID = (Guid)item;

                if (gameID == Model.ConvertEngine.Game.MTG.GameGuidStatic)
                {
                    return this.MTGTemplate;
                }
                else if (gameID == Model.ConvertEngine.Game.LoTR.GameGuidStatic)
                {
                    return this.LoTRTemplate;
                }
                else if (gameID == Model.ConvertEngine.Game.MW.GameGuidStatic)
                {
                    return this.MWTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
