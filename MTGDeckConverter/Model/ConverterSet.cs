// -----------------------------------------------------------------------
// <copyright file="ConverterSet.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// Contains the Octgn information about a Set, and a reference to the Octgn.Data.Set object
    /// </summary>
    public class ConverterSet
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ConverterSet class.
        /// It represents a Set from Octgn, and provides a list of ConverterCards which are
        /// found in the Set.
        /// </summary>
        /// <param name="octgnSet">The Octgn Set</param>
        public ConverterSet(Octgn.DataNew.Entities.Set octgnSet)
        {
            if (octgnSet == null) 
            {
                throw new ArgumentNullException(); 
            }
            
            this.OctgnSet = octgnSet;
            this.IncludeInSearches = true;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets the referenced Octgn Set object
        /// </summary>
        public Octgn.DataNew.Entities.Set OctgnSet
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this Set should be included in searches for potentially matching Cards
        /// </summary>
        public bool IncludeInSearches
        {
            get;
            set;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ConverterCard _MaxMultiverseIDCard;

        /// <summary>
        /// Gets the maximum Multiverse ID number of any Card in this Set.  If none of the Cards had
        /// a valid MultiverseID, then int.MaxValue is returned.
        /// (The reason int.MaxValue is returned in that case is because Sets which don't have Multiverse IDs
        /// tend to be the newest sets, and WoTC probably hasn't updated the Gatherer database yet)
        /// </summary>
        public int MaxMultiverseID
        {
            get
            {
                return this._MaxMultiverseIDCard != null ?
                    this._MaxMultiverseIDCard.MultiverseID :
                    int.MaxValue;
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private List<ConverterCard> _ConverterCards = new List<ConverterCard>();

        /// <summary>
        /// Gets the List of ConverterCards which belong to this Set
        /// </summary>
        public ReadOnlyCollection<ConverterCard> ConverterCards
        {
            get { return this._ConverterCards.AsReadOnly(); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates a new ConverterCard with the given parameters and adds it to it's ConverterCards List
        /// </summary>
        /// <param name="cardID">Octgn Guid of the Card</param>
        /// <param name="name">Octgn Name of the Card</param>
        /// <param name="multiverseID">WoTC Multiverse ID</param>
        internal void AddNewConverterCard(Guid cardID, string name, int multiverseID)
        {
            ConverterCard newConverterCard = new ConverterCard(cardID, name, this.OctgnSet.Name, multiverseID);
            this._ConverterCards.Add(newConverterCard);

            if 
            (
                this._MaxMultiverseIDCard == null ||
                newConverterCard.MultiverseID > this._MaxMultiverseIDCard.MultiverseID
            )
            {
                this._MaxMultiverseIDCard = newConverterCard;
            }
        }

        /// <summary>
        /// Sorts the items in ConverterCards by Name
        /// </summary>
        internal void SortConverterCards()
        {
            this._ConverterCards = this._ConverterCards.OrderBy(cc => cc.Name).ToList();
        }

        #endregion Public Methods
    }
}
