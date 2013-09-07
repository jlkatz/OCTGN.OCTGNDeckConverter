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

namespace OCTGNDeckConverter.Model
{
    /// <summary>
    /// This object keeps tracks of the parameters and data for an entire Deck conversion process.  This uses
    /// ConvertEngine to convert as needed based on the user parameters.
    /// </summary>
    public class Converter : INotifyPropertyChangedBase
    {
        /// <summary>
        /// Guid identifier for the OCTGN MTG Game
        /// </summary>
        public static readonly Guid MTGGameGuid = Guid.Parse("A6C8D2E8-7CD8-11DD-8F94-E62B56D89593");

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
        /// Property name constant
        /// </summary>
        private const string ConverterGamePropertyName = "ConverterGame";

        /// <summary>
        /// Private backing field
        /// </summary>
        private ConverterGame _ConverterGame;

        /// <summary>
        /// Gets or sets the ConverterGame instance which stores data about the Deck to convert
        /// </summary>
        public ConverterGame ConverterGame
        {
            get { return this._ConverterGame; }
            set { this.SetValue(ref this._ConverterGame, value, ConverterGamePropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        private const string ConverterDeckPropertyName = "ConverterDeck";

        /// <summary>
        /// Private backing field
        /// </summary>
        private ConverterDeck _ConverterDeck;

        /// <summary>
        /// Gets the ConverterDeck instance which stores data about the Deck to convert
        /// </summary>
        public ConverterDeck ConverterDeck
        {
            get { return this._ConverterDeck; }
            private set { this.SetValue(ref this._ConverterDeck, value, ConverterDeckPropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        private const string DeckFileNameWithoutExtensionPropertyName = "DeckFileNameWithoutExtension";

        /// <summary>
        /// Private backing field
        /// </summary>
        private string _DeckFileNameWithoutExtension = string.Empty;

        /// <summary>
        /// Gets or sets the name of the Deck file without the file-extension
        /// </summary>
        public string DeckFileNameWithoutExtension
        {
            get { return this._DeckFileNameWithoutExtension; }
            set { this.SetValue(ref this._DeckFileNameWithoutExtension, value, DeckFileNameWithoutExtensionPropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        private const string DeckFullPathNamePropertyName = "DeckFullPathName";

        /// <summary>
        /// Private backing field
        /// </summary>
        private string _DeckFullPathName = string.Empty;

        /// <summary>
        /// Gets or sets the Full Path Name of the Deck file to convert
        /// </summary>
        public string DeckFullPathName
        {
            get { return this._DeckFullPathName; }
            set { this.SetValue(ref this._DeckFullPathName, value, DeckFullPathNamePropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        private const string DeckURLPropertyName = "DeckURL";

        /// <summary>
        /// Private backing field
        /// </summary>
        private string _DeckURL = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the Deck file to convert
        /// </summary>
        public string DeckURL
        {
            get { return this._DeckURL; }
            set { this.SetValue(ref this._DeckURL, value, DeckURLPropertyName); }
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private Dictionary<string, string> _SectionsText = new Dictionary<string, string>();

        /// <summary>
        /// Gets a dictionary who's keys are each Section of a deck, and who's values are the text contents that the user inputted.
        /// </summary>
        public Dictionary<string, string> SectionsText
        {
            get { return this._SectionsText; }
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
            if (!this.ConverterGame.WaitForInitializationToComplete(TimeSpan.FromSeconds(30)))
            {
                throw new TimeoutException("Timeout while building the Card Database");
            }

            if (!this.DeckSourceType.HasValue)
            {
                return new Tuple<bool, string>(false, "Deck Source has not been chosen");
            }

            try
            {
                if (this.ConverterGame.Game.Id == Converter.MTGGameGuid)
                {
                    switch (this.DeckSourceType.Value)
                    {
                        case DeckSourceTypes.File:
                            this.ConverterDeck = ConvertEngine.ConvertMTGFile(this.DeckFullPathName, this.ConverterGame.Sets, this.ConverterGame.DeckSectionNames);
                            break;

                        case DeckSourceTypes.Webpage:
                            this.ConverterDeck = ConvertEngine.ConvertMTGURL(this.DeckURL, this.ConverterGame.Sets, this.ConverterGame.DeckSectionNames);
                            break;

                        case DeckSourceTypes.Text:
                            this.ConverterDeck = ConvertEngine.ConvertText(this.SectionsText, this.ConverterGame.Sets, this.ConverterGame.DeckSectionNames);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    switch (this.DeckSourceType.Value)
                    {
                        case DeckSourceTypes.Text:
                            this.ConverterDeck = ConvertEngine.ConvertText(this.SectionsText, this.ConverterGame.Sets, this.ConverterGame.DeckSectionNames);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
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
            this.CreateDeck().Save(this.ConverterGame.Game, fullPathName);
        }

        /// <summary>
        /// Creates and returns an OCTGN format Deck who's contents are the cards which were converted in the Wizard.
        /// </summary>
        /// <returns>an OCTGN format Deck who's contents are the cards which were converted in the Wizard.</returns>
        public Octgn.DataNew.Entities.Deck CreateDeck()
        {
            Octgn.DataNew.Entities.Deck deck = this.ConverterGame.Game.CreateDeck();

            Dictionary<string, Octgn.DataNew.Entities.ISection> deckSections = new Dictionary<string, Octgn.DataNew.Entities.ISection>();
            foreach (Octgn.DataNew.Entities.ISection section in deck.Sections)
            {
                deckSections.Add(section.Name, section);
            }

            foreach (ConverterSection converterSection in this.ConverterDeck.ConverterSections)
            {
                foreach (ConverterMapping converterMapping in converterSection.SectionMappings)
                {
                    if (converterMapping.SelectedOCTGNCard != null)
                    {
                        Octgn.DataNew.Entities.Card octgnCard = this.ConverterGame.Game.AllCards().First(c => c.Id == converterMapping.SelectedOCTGNCard.CardID);
                        Octgn.DataNew.Entities.MultiCard octgnMultiCard = octgnCard.ToMultiCard(converterMapping.Quantity);
                        deckSections[converterSection.SectionName].Cards.AddCard(octgnMultiCard);
                    }
                }
            }

            // Auto-inserting notes into the deck has been removed because it 
            // is annoying when it pops up every time you load a deck.
            //var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //var version = assembly.GetName().Version;
            //StringBuilder deckNotes = new StringBuilder();
            //deckNotes.AppendLine("Imported with Splat's OCTGN Deck Converter v" + version.Major + "." + version.Minor + "." + version.Build);
            //deckNotes.AppendLine(@"http://octgn.gamersjudgement.com/wordpress/mtg/deck-editor-plugins/");
            //deckNotes.AppendLine();
            //deckNotes.Append("Source: ");
            //deckNotes.AppendLine(this.DeckSourceType.Value.ToString());

            //switch(this.DeckSourceType.Value)
            //{
            //    case DeckSourceTypes.File:
            //        deckNotes.Append("File: ");
            //        deckNotes.AppendLine(this.DeckFullPathName);
            //        break;

            //    case DeckSourceTypes.Webpage:
            //        deckNotes.Append("URL: ");
            //        deckNotes.AppendLine(this.DeckURL);
            //        break;
            //}

            //deckNotes.Append("Name: ");
            //deckNotes.AppendLine(this.ConverterDeck.DeckName);
            //deck.Notes = deckNotes.ToString();

            deck.Notes = string.Empty;
            return deck;
        }

        #endregion Public Methods
    }
}
