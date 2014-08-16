// -----------------------------------------------------------------------
// <copyright file="ConverterDeck.cs" company="jlkatz">
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
    /// Contains a collection of all Sections for the Deck being converted.
    /// </summary>
    public class ConverterDeck : INotifyPropertyChangedBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ConverterDeck class
        /// </summary>
        /// <param name="deckSections">A collection of the names of each Section in the deck</param>
        public ConverterDeck(IEnumerable<string> deckSections)
        {
            if (deckSections == null)
            {
                throw new ArgumentNullException();
            }
            else if (deckSections.Count() == 0)
            {
                throw new InvalidOperationException("Decks must have at least one Section");
            }

            foreach (string deckSection in deckSections)
            {
                this._ConverterSections.Add(new ConverterSection(deckSection));
            }
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Private backing field
        /// </summary>
        private List<ConverterSection> _ConverterSections = new List<ConverterSection>();

        /// <summary>
        /// Gets a collection of ConverterSections that represent each possible Section of the Deck
        /// </summary>
        public ReadOnlyCollection<ConverterSection> ConverterSections
        {
            get { return this._ConverterSections.AsReadOnly(); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        private const string DeckNamePropertyName = "DeckName";

        /// <summary>
        /// Private backing field
        /// </summary>
        private string _DeckName = string.Empty;

        /// <summary>
        /// Gets or sets the name of this Deck.  When set, invalid filename characters are automatically replaced with '_'.
        /// </summary>
        public string DeckName
        {
            get
            {
                return this._DeckName;
            }

            set
            {
                if (value != null)
                {
                    // http://stackoverflow.com/a/847251
                    string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                    string invalidReStr = string.Format(@"[{0}]+", invalidChars);
                    string validFilenameValue = System.Text.RegularExpressions.Regex.Replace(value, invalidReStr, "_");

                    this.SetValue(ref this._DeckName, validFilenameValue, DeckNamePropertyName);
                }
                else
                {
                    this.SetValue(ref this._DeckName, string.Empty, DeckNamePropertyName);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the conversion of the Deck to Octgn was successful or not.
        /// </summary>
        public bool ConversionSuccessful
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tell each ConverterMapping in each Section to auto-select a potential OCTGN Card
        /// </summary>
        public void AutoSelectPotentialCards()
        {
            foreach (ConverterSection converterSection in this.ConverterSections)
            {
                foreach (ConverterMapping converterMapping in converterSection.SectionMappings)
                {
                    converterMapping.AutoSelectPotentialOCTGNCard();
                }
            }
        }

        /// <summary>
        /// Populates every ConverterMapping in each Section with potential cards from the converterSets
        /// </summary>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        public void PopulateConverterMappings(Dictionary<Guid, ConverterSet> converterSets)
        {
            foreach (ConverterSection converterSection in this.ConverterSections)
            {
                foreach (ConverterMapping converterMapping in converterSection.SectionMappings)
                {
                    converterMapping.PopulateWithPotentialCards(converterSets);
                }

                // Try matching each incorrectly formatted line, and assume a quantity of 1
                List<string> linesWithoutQuantityButMatchingName = new List<string>();
                foreach (string incorrectlyFormattedLine in converterSection.IncorrectlyFormattedLines)
                {
                    ConverterMapping cm = new ConverterMapping(incorrectlyFormattedLine, string.Empty, 1);
                    cm.PopulateWithPotentialCards(converterSets);
                    if (cm.PotentialOCTGNCards.Count > 0)
                    {
                        converterSection.AddConverterMapping(cm);
                        linesWithoutQuantityButMatchingName.Add(incorrectlyFormattedLine);
                    }
                }

                // Incorrectly formatted lines which were found to match a card name should be removed from incorrect list.
                foreach(string line in linesWithoutQuantityButMatchingName)
                {
                    converterSection.RemoveIncorrectlyFormattedLine(line);
                }
            }
        }

        /// <summary>
        /// Updates the MainDeckCount and SideBoardCount properties with an up-to-date 
        /// count of the number of Cards.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        public void UpdateCardCounts()
        {
            foreach (ConverterSection converterSection in this.ConverterSections)
            {
                converterSection.UpdateSectionCount();
            }
        }

        #endregion Public Methods
    }
}
