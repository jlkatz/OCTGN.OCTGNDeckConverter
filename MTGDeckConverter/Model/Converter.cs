// -----------------------------------------------------------------------
// <copyright file="Converter.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octgn.Core.DataExtensionMethods;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// This object keeps tracks of the parameters and data for an entire Deck conversion process.  This uses
    /// ConvertEngine to convert as needed based on the user parameters.
    /// </summary>
    public class Converter : INotifyPropertyChangedBase
    {
        /// <summary>
        /// Initializes a new instance of the Converter class.
        /// Creates a new blank Converter ready to convert from anything
        /// </summary>
        public Converter()
        {
            this.DeckSourceType = null;
        }

        #region Public Properties

        /// <summary>
        /// Gets Singleton-instance of the ConverterDatabase
        /// </summary>
        public ConverterDatabase ConverterDatabase
        {
            get { return ConverterDatabase.SingletonInstance; }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string ConverterDeckPropertyName = "ConverterDeck";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ConverterDeck _ConverterDeck;

        /// <summary>
        /// Gets the ConverterDeck instance which stores data about the Deck to convert
        /// </summary>
        public ConverterDeck ConverterDeck
        {
            get { return this._ConverterDeck; }
            private set { this.SetValue(ref this._ConverterDeck, value, ConverterDeckPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string DeckFileNameWithoutExtensionPropertyName = "DeckFileNameWithoutExtension";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _DeckFileNameWithoutExtension = string.Empty;

        /// <summary>
        /// Gets or sets the name of the Deck file without the file-extension
        /// </summary>
        public string DeckFileNameWithoutExtension
        {
            get { return this._DeckFileNameWithoutExtension; }
            set { this.SetValue(ref this._DeckFileNameWithoutExtension, value, DeckFileNameWithoutExtensionPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string DeckFullPathNamePropertyName = "DeckFullPathName";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _DeckFullPathName = string.Empty;

        /// <summary>
        /// Gets or sets the Full Path Name of the Deck file to convert
        /// </summary>
        public string DeckFullPathName
        {
            get { return this._DeckFullPathName; }
            set { this.SetValue(ref this._DeckFullPathName, value, DeckFullPathNamePropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string DeckURLPropertyName = "DeckURL";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _DeckURL = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the Deck file to convert
        /// </summary>
        public string DeckURL
        {
            get { return this._DeckURL; }
            set { this.SetValue(ref this._DeckURL, value, DeckURLPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string MainDeckTextPropertyName = "MainDeckText";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _MainDeckText = string.Empty;

        /// <summary>
        /// Gets or sets the text which represents the cards in the Main Deck section of the Deck
        /// </summary>
        public string MainDeckText
        {
            get { return this._MainDeckText; }
            set { this.SetValue(ref this._MainDeckText, value, MainDeckTextPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SideBoardTextPropertyName = "SideBoardText";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _SideBoardText = string.Empty;

        /// <summary>
        /// Gets or sets the text which represents the cards in the Sideboard section of the deck
        /// </summary>
        public string SideBoardText
        {
            get { return this._SideBoardText; }
            set { this.SetValue(ref this._SideBoardText, value, SideBoardTextPropertyName); }
        }

        /// <summary>
        /// Gets or sets an enumeration which denotes the source where the Deck is located.  This is initially null.
        /// </summary>
        public DeckSourceTypes? DeckSourceType
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the ConverterDeck property to a new instance which represents the data that is converted
        /// using the parameters defined in this object.  Returns a Tuple where Item1 (bool) indicates if
        /// the conversion was successful, and Item2 (string) contains any error messages if unsuccessful.
        /// </summary>
        /// <returns>A Tuple where Item1 indicates if the conversion was successful, and Item2 contains any error messages if unsuccessful.</returns>
        public Tuple<bool, string> Convert()
        {
            // Before attempting to convert, ensure that the card database is fully built.
            // If OCTGN takes too long building the database, give up
            if (!this.ConverterDatabase.WaitForInitializationToComplete(TimeSpan.FromSeconds(30)))
            {
                throw new TimeoutException("Timeout while building the Card Database");
            }

            if (!this.DeckSourceType.HasValue)
            {
                return new Tuple<bool, string>(false, "Deck Source has not been chosen");
            }

            try
            {
                switch (this.DeckSourceType.Value)
                {
                    case DeckSourceTypes.File:
                        this.ConverterDeck = ConvertEngine.ConvertFile(this.DeckFullPathName, this.ConverterDatabase.Sets);
                        break;

                    case DeckSourceTypes.Webpage:
                        this.ConverterDeck = ConvertEngine.ConvertURL(this.DeckURL, this.ConverterDatabase.Sets);
                        break;

                    case DeckSourceTypes.Text:
                        this.ConverterDeck = ConvertEngine.ConvertText(this.MainDeckText, this.SideBoardText, this.ConverterDatabase.Sets);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                return new Tuple<bool, string>(false, e.ToString());
            }

            this.ConverterDeck.AutoSelectPotentialCards();
            this.ConverterDeck.UpdateCardCounts();

            if (string.IsNullOrWhiteSpace(this.ConverterDeck.DeckName))
            {
                this.ConverterDeck.DeckName = this.DeckFileNameWithoutExtension;
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Saves the deck to disk in the Octgn 3 format
        /// </summary>
        /// <param name="fullPathName">The full path name of the location to save the converted deck.</param>
        public void SaveDeck(string fullPathName)
        {
            this.CreateDeck().Save(this.ConverterDatabase.GameDefinition, fullPathName);
        }

        /// <summary>
        /// Creates and returns an OCTGN format Deck who's contents are the cards which were converted in the Wizard.
        /// </summary>
        /// <returns>an OCTGN format Deck who's contents are the cards which were converted in the Wizard.</returns>
        public Octgn.DataNew.Entities.Deck CreateDeck()
        {
            Octgn.DataNew.Entities.Deck deck = this.ConverterDatabase.GameDefinition.CreateDeck();

            // [0] = "Main"
            // [1] = "Sideboard"
            // [2] = "Command Zone"
            // [3] = "Planes/Schemes"
            Octgn.DataNew.Entities.ISection mainDeckSection = deck.Sections.First(s => s.Name.Equals("Main", StringComparison.InvariantCultureIgnoreCase));
            Octgn.DataNew.Entities.ISection sideboardSection = deck.Sections.First(s => s.Name.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase));

            List<Tuple<Octgn.DataNew.Entities.ISection, IEnumerable<ConverterMapping>>> pairSectionAndMappingsList = 
                new List<Tuple<Octgn.DataNew.Entities.ISection, IEnumerable<ConverterMapping>>>()
            {
                new Tuple<Octgn.DataNew.Entities.ISection, IEnumerable<ConverterMapping>>(mainDeckSection, this.ConverterDeck.MainDeck),
                new Tuple<Octgn.DataNew.Entities.ISection, IEnumerable<ConverterMapping>>(sideboardSection, this.ConverterDeck.SideBoard),
            };

            foreach (var pair in pairSectionAndMappingsList)
            {
                foreach (ConverterMapping converterMapping in pair.Item2)
                {
                    if (converterMapping.SelectedOCTGNCard != null)
                    {
                        Octgn.DataNew.Entities.Card octgnCard = this.ConverterDatabase.GameDefinition.AllCards().First(c => c.Id == converterMapping.SelectedOCTGNCard.CardID);
                        Octgn.DataNew.Entities.MultiCard octgnMultiCard = octgnCard.ToMultiCard(converterMapping.Quantity);
                        Octgn.DataNew.Entities.ISection octgnDeckSection = pair.Item1;
                        octgnDeckSection.Cards.AddCard(octgnMultiCard);
                    }
                }
            }

            return deck;
        }

        #endregion Public Methods
    }
}
