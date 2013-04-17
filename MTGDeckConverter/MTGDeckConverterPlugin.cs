// -----------------------------------------------------------------------
// <copyright file="MTGDeckConverterPlugin.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octgn.Core.DataManagers;
using Octgn.Core.Plugin;

namespace MTGDeckConverter
{
    /// <summary>
    /// Implements IDeckBuilderPlugin so it can be used as a Plugin for OCTGN.
    /// </summary>
    public class MTGDeckConverterPlugin : IDeckBuilderPlugin
    {
        /// <summary>
        /// Gets the Menu Items to add for the plugin.
        /// </summary>
        public IEnumerable<IPluginMenuItem> MenuItems
        {
            get
            {
                return new List<IPluginMenuItem> { new MTGDeckConverterPluginMenuItem() };
            }
        }

        /// <summary>
        /// Happens when the Deck Editor is opened.
        /// </summary>
        /// <param name="gameManager">Game Manager</param>
        public void OnLoad(GameManager gameManager)
        {
            // Nothing for now
        }

        /// <summary>
        /// Gets the Id.  All plugins are required to have a unique GUID
        /// </summary>
        public Guid Id
        {
            get 
            {
                return Guid.Parse("ace74ec1-647a-4085-9402-b73a0cd9a24e");
            }
        }

        /// <summary>
        /// Gets the display name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                return "Magic: the Gathering Deck Converter"; 
            }
        }

        /// <summary>
        /// Gets the minimum version of Octgn allowed
        /// </summary>
        public Version RequiredByOctgnVersion
        {
            get 
            {
                return Version.Parse("3.1.0.0"); 
            }
        }

        /// <summary>
        /// Gets the Version of the plugin.
        /// </summary>
        public Version Version
        {
            get
            {
                // This code will pull the version from the assembly.
                return System.Reflection.Assembly.GetCallingAssembly().GetName().Version;
            }
        }
    }
}
