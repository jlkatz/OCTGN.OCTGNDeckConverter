// -----------------------------------------------------------------------
// <copyright file="ConverterMapping.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model
{
    /// <summary>
    /// Represents a parsed Card, Quantity, and possibly Set.  It contains a collection of potential
    /// OCTGN Cards that could be a match, which are exposed to the user so they can choose.
    /// </summary>
    public class ConverterMapping : INotifyPropertyChangedBase
    {
        /// <summary>
        /// Initializes a new instance of the ConverterMapping class.
        /// </summary>
        /// <param name="cardName">The parsed name of the Card</param>
        /// <param name="cardSet">The parsed name (or abbreviation) of the Set the converted Card belongs to.  If unavailable, use string.Empty</param>
        /// <param name="quantity">The quantity of this Card that is included in the Deck</param>
        public ConverterMapping(string cardName, string cardSet, int quantity)
        {
            this.CardName = cardName;
            this.CardSet = cardSet;
            this.Quantity = quantity;
        }

        #region Public Properties

        /// <summary>
        /// Gets the parsed name of the Card
        /// </summary>
        public string CardName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parsed name (or abbreviation) of the Set the Card belongs to.  If unavailable, it is string.Empty
        /// </summary>
        public string CardSet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the quantity of this Card that is included in the Deck
        /// </summary>
        public int Quantity
        {
            get;
            private set;
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private ObservableCollection<ConverterCard> _PotentialOCTGNCards = new ObservableCollection<ConverterCard>();

        /// <summary>
        /// Private backing field
        /// </summary>
        private ReadOnlyObservableCollection<ConverterCard> _PotentialOCTGNCardsReadOnly;
        
        /// <summary>
        /// Gets the collection of potential OCTGN Cards that match the Imported card.  Sorted by MultiverseID.
        /// </summary>
        public ReadOnlyObservableCollection<ConverterCard> PotentialOCTGNCards
        {
            get 
            {
                if (this._PotentialOCTGNCardsReadOnly == null)
                { 
                    this._PotentialOCTGNCardsReadOnly = new ReadOnlyObservableCollection<ConverterCard>(this._PotentialOCTGNCards); 
                }

                return this._PotentialOCTGNCardsReadOnly;
            }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string SelectedOCTGNCardPropertyName = "SelectedOCTGNCard";

        /// <summary>
        /// Private backing field
        /// </summary>
        private ConverterCard _SelectedOCTGNCard;
        
        /// <summary>
        /// Gets or sets the currently selected OCTGN Card.  This card will be the one used when generating the OCTGN deck.
        /// </summary>
        public ConverterCard SelectedOCTGNCard
        {
            get { return this._SelectedOCTGNCard; }
            set { this.SetValue(ref this._SelectedOCTGNCard, value, SelectedOCTGNCardPropertyName); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the potentialCard to the PotentialOCTGNCards Collection in MultiverseID order.
        /// </summary>
        /// <param name="potentialCard">The ConverterCard to add to the PotentialOCTGNCards Collection</param>
        /// <returns>True if added, false if not</returns>
        public bool AddPotentialOCTGNCard(ConverterCard potentialCard)
        {
            if (potentialCard == null)
            {
                throw new ArgumentNullException();
            }

            if (this._PotentialOCTGNCards.Contains(potentialCard))
            { 
                return false; 
            }

            if (this._PotentialOCTGNCards.Count == 0)
            {
                // Add it since it is the only item
                this._PotentialOCTGNCards.Add(potentialCard);
            }
            else if (this._PotentialOCTGNCards.Last().MultiverseID > potentialCard.MultiverseID)
            {
                // Add it at the end
                this._PotentialOCTGNCards.Add(potentialCard);
            }
            else
            {
                int index = 0;
                for (int i = 0; i < this._PotentialOCTGNCards.Count; i++)
                {
                    if (this._PotentialOCTGNCards[i].MultiverseID > potentialCard.MultiverseID)
                    { 
                        break; 
                    }
                    else
                    { 
                        index = i; 
                    }
                }

                this._PotentialOCTGNCards.Insert(index, potentialCard);
            }

            return true;
        }

        /// <summary>
        /// Sets the SelectedOCTGNCard to the ConverterCard with the highest Multiverse ID
        /// </summary>
        public void AutoSelectPotentialOCTGNCard()
        {
            if (this.PotentialOCTGNCards.Count == 0)
            {
                this.SelectedOCTGNCard = null;
            }
            else
            {
                ConverterCard maxMultiverseIDCard = this.PotentialOCTGNCards.First();
                foreach (ConverterCard cc in this.PotentialOCTGNCards)
                {
                    if (cc.MultiverseID > maxMultiverseIDCard.MultiverseID)
                    {
                        maxMultiverseIDCard = cc;
                    }
                }

                this.SelectedOCTGNCard = maxMultiverseIDCard;
            }
        }

        /// <summary>
        /// Searches through all converterSets for Cards which potentially match this CardName/CardSet.
        /// Each potential card is added as a potential Card.
        /// </summary>
        /// <param name="converterSets">The list of ConverterSets to search in for potential matches</param>
        public void PopulateWithPotentialCards(Dictionary<Guid, ConverterSet> converterSets)
        {
            // Names and sets contain many accents or other special characters.  Replace them
            // all with 'normal' letters for comparison, since many sources use them instead.
            List<dynamic> replacementChars = new List<dynamic>()
            {
                new { Actual = "Æ", Normalized = "Ae" },

                new { Actual = "Â", Normalized = "A" },
                new { Actual = "Á", Normalized = "A" },
                new { Actual = "â", Normalized = "a" },
                new { Actual = "á", Normalized = "a" },
                
                new { Actual = "Ê", Normalized = "E" },
                new { Actual = "É", Normalized = "E" },
                new { Actual = "ê", Normalized = "e" },
                new { Actual = "é", Normalized = "e" },
                
                new { Actual = "Î", Normalized = "I" },
                new { Actual = "Í", Normalized = "I" },
                new { Actual = "î", Normalized = "i" },
                new { Actual = "í", Normalized = "i" },
                
                new { Actual = "Ô", Normalized = "O" },
                new { Actual = "Ó", Normalized = "O" },
                new { Actual = "ô", Normalized = "o" },
                new { Actual = "ó", Normalized = "o" },
                
                new { Actual = "Û", Normalized = "U" },
                new { Actual = "Ú", Normalized = "U" },
                new { Actual = "û", Normalized = "u" },
                new { Actual = "ú", Normalized = "u" },

                new { Actual = '’', Normalized = '\'' },
                new { Actual = ":", Normalized = string.Empty },
                new { Actual = "-", Normalized = string.Empty },
                new { Actual = "'", Normalized = string.Empty },
            };

            // Some cards have 2+ names, so create an array of all the names in order
            string[] converterMappingNames = this.CardName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (ConverterSet converterSet in converterSets.Values)
            {
                if (converterSet.IncludeInSearches)
                {
                    foreach (ConverterCard converterCard in converterSet.ConverterCards)
                    {
                        string[] converterCardNames = converterCard.Name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        int numIndexToCompare = Math.Min(converterMappingNames.Length, converterCardNames.Length);

                        // Compare all names.  If one card has less names than the other, then just compare indexes where 
                        // both names are present because sometimes another format only includes the first name
                        bool isNameMatch = true;
                        for (int i = 0; i < numIndexToCompare; i++)
                        {
                            // Remove extra whitespace
                            string converterCardName = converterCardNames[i].Trim();
                            string converterMappingName = converterMappingNames[i].Trim();

                            foreach (dynamic replacementChar in replacementChars)
                            {
                                converterCardName = converterCardName.Replace(replacementChar.Actual, replacementChar.Normalized);
                                converterMappingName = converterMappingName.Replace(replacementChar.Actual, replacementChar.Normalized);
                            }

                            if (!converterCardName.Equals(converterMappingName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                isNameMatch = false;
                                break;
                            }
                        }

                        bool isSetMatch = true;
                        if (isNameMatch && !string.IsNullOrWhiteSpace(this.CardSet))
                        {
                            // LoTR - Pay attention to Set
                            if (converterSet.OctgnSet.GameId == ConvertEngine.Game.LoTR.GameGuidStatic)
                            {
                                string converterCardSet = converterCard.Set;
                                string converterMappingSet = this.CardSet;

                                // Some sources omit 'The Hobbit - ' in the set name, so remove it from the comparison
                                if (converterCardSet.StartsWith("The Hobbit", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    converterCardSet = converterCardSet.Substring(13);
                                }
                                if (converterMappingSet.StartsWith("The Hobbit", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    converterMappingSet = converterMappingSet.Substring(13);
                                }

                                // Some sources omit 'The ' in the set name, so remove it from the comparison
                                if (converterCardSet.StartsWith("The ", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    converterCardSet = converterCardSet.Substring(4);
                                }
                                if (converterMappingSet.StartsWith("The ", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    converterMappingSet = converterMappingSet.Substring(4);
                                }

                                foreach (dynamic replacementChar in replacementChars)
                                {
                                    converterCardSet = converterCardSet.Replace(replacementChar.Actual, replacementChar.Normalized);
                                    converterMappingSet = converterMappingSet.Replace(replacementChar.Actual, replacementChar.Normalized);
                                }

                                if (!converterCardSet.Equals(converterMappingSet, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    isSetMatch = false;
                                }
                            }
                        }

                        if (isNameMatch && isSetMatch)
                        {
                            this.AddPotentialOCTGNCard(converterCard);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the potentialCard from the PotentialOCTGNCards Collection
        /// </summary>
        /// <param name="potentialCard">The ConverterCard to remove from the PotentialOCTGNCards Collection</param>
        /// <returns>True if removed, false if not</returns>
        public bool RemovePotentialOCTGNCard(ConverterCard potentialCard)
        {
            return this._PotentialOCTGNCards.Remove(potentialCard);
        }

        #endregion Public Methods
    }
}
