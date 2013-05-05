// -----------------------------------------------------------------------
// <copyright file="DeckSourceTypes.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// Types of sources a Deck to be imported could come from.
    /// </summary>
    public enum DeckSourceTypes
    {
        /// <summary>
        /// Represents a Deck that will be imported from a file
        /// </summary>
        File,

        /// <summary>
        /// Represents a Deck that will be imported from a webpage
        /// </summary>
        Webpage,

        /// <summary>
        /// Represents a Deck that will be imported from plain text
        /// </summary>
        Text,
    }
}
