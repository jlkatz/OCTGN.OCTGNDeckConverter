// -----------------------------------------------------------------------
// <copyright file="ConverterCard.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MTGDeckConverter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    /// Contains the Octgn information about a Card in a Set.
    /// </summary>
    public class ConverterCard
    {
        /// <summary>
        /// Initializes a new instance of the ConverterCard class.
        /// </summary>
        /// <param name="cardID">Octgn Guid of the Card</param>
        /// <param name="name">Octgn Name of the Card</param>
        /// <param name="set">Octgn Set name that the Card belongs to</param>
        /// <param name="multiverseID">WoTC Multiverse ID</param>
        public ConverterCard(Guid cardID, string name, string set, int multiverseID)
        {
            if (name == null) 
            { 
                throw new ArgumentNullException(); 
            }

            this.CardID = cardID;
            this.Name = name;
            this.Set = set;
            this.MultiverseID = multiverseID;
        }

        /// <summary>
        /// Gets the Octgn Guid of this Card
        /// </summary>
        public Guid CardID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Octgn Name of this Card
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Octgn Set name that this Card belongs to
        /// </summary>
        public string Set
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the WoTC Multiverse ID that represents this Card
        /// </summary>
        public int MultiverseID
        {
            get;
            private set;
        }
    }
}
