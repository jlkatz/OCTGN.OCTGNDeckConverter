using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Game
{
    public abstract class GameConverter
    {
        private List<File.FileConverter> compatibleFileConverters;

        private List<Webpage.WebpageConverter> compatibleWebpageConverters;

        public GameConverter()
        {
            this.compatibleFileConverters = new List<File.FileConverter>(this.GenerateCompatibleFileConverters());
            this.compatibleWebpageConverters = new List<Webpage.WebpageConverter>(this.GenerateCompatibleWebpageConverters());
        }

        /// <summary>
        /// Gets the Guid identifier for this Game that matches that found in OCTGN
        /// </summary>
        public abstract Guid GameGuid { get; }

        protected ReadOnlyCollection<File.FileConverter> CompatibleFileConverters
        {
            get { return this.compatibleFileConverters.AsReadOnly(); }
        }

        protected ReadOnlyCollection<Webpage.WebpageConverter> CompatibleWebpageConverters
        {
            get { return this.compatibleWebpageConverters.AsReadOnly(); }
        }

        /// <summary>
        /// Finds the first FileConverter that is capable of converting the file with the extension, or null if none found
        /// </summary>
        /// <param name="extension">The extention to check if matches any FileConverter</param>
        /// <returns>The first FileConverter that is capable of converting the file with the extension, or null if none found</returns>
        protected File.FileConverter FindMatchingFileConverter(string extension)
        {
            return this.compatibleFileConverters.FirstOrDefault(fc => fc.ExtensionMatches(extension));
        }

        /// <summary>
        /// Finds the first WebpageConverter that is capable of converting the webpage in the url, or null if none found
        /// </summary>
        /// <param name="url">The url to check if matches any WebpageConverter</param>
        /// <returns>The first WebpageConverter that is capable of converting the url, or null if none found</returns>
        protected Webpage.WebpageConverter FindMatchingWebpageConverter(string url)
        {
            return this.compatibleWebpageConverters.FirstOrDefault(wc => wc.URLMatches(url));
        }

        /// <summary>
        /// Converts a text file into a ConverterDeck which has all ConverterMappings populated with potential cards from the ConverterGame
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="converterGame">The ConverterGame instance that will be used for searching for matches</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential OCTGN cards from the converterSets</returns>
        public ConverterDeck ConvertFile(string fullPathName, ConverterGame converterGame)
        {
            this.ThrowIfConverterGameDoesntMatch(converterGame);

            ConverterDeck converterDeck = this.ConvertFile(fullPathName, converterGame.DeckSectionNames);

            converterDeck.PopulateConverterMappings(converterGame.Sets);

            return converterDeck;
        }

        /// <summary>
        /// Converts a text file that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings defined, but not yet populated with potential OCTGN cards</returns>
        protected abstract ConverterDeck ConvertFile(string fullPathName, IEnumerable<string> deckSectionNames);

        /// <summary>
        /// Converts a URL into a ConverterDeck which has all ConverterMappings populated with potential cards from the ConverterGame
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="converterGame">The ConverterGame instance that will be used for searching for matches</param>
        /// <returns>A ConverterDeck which represents the data that is converted using the contents of the file</returns>
        public ConverterDeck ConvertURL(string url, ConverterGame converterGame)
        {
            this.ThrowIfConverterGameDoesntMatch(converterGame);

            ConverterDeck converterDeck = this.ConvertURL(url, converterGame.DeckSectionNames);

            converterDeck.PopulateConverterMappings(converterGame.Sets);

            return converterDeck;
        }

        /// <summary>
        /// Converts a URL that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        protected abstract ConverterDeck ConvertURL(string url, IEnumerable<string> deckSectionNames);

        /// <summary>
        /// Converts user input text into a ConverterDeck which has all ConverterMappings populated with potential cards from the ConverterGame
        /// </summary>
        /// <param name="sectionsText">A collection of section names (keys), and the user input text of all cards in the section (values)</param>
        /// <param name="converterGame">The ConverterGame instance that will be used for searching for matches</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public ConverterDeck ConvertText(Dictionary<string, string> sectionsText, ConverterGame converterGame)
        {
            this.ThrowIfConverterGameDoesntMatch(converterGame);

            ConverterDeck converterDeck = this.ConvertText(sectionsText, converterGame.Sets, converterGame.DeckSectionNames);

            converterDeck.PopulateConverterMappings(converterGame.Sets);

            return converterDeck;
        }

        /// <summary>
        /// Converts user input text that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="sectionsText">A collection of section names (keys), and the user input text of all cards in the section (values)</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        protected abstract ConverterDeck ConvertText(Dictionary<string, string> sectionsText, Dictionary<Guid, ConverterSet> converterSets, IEnumerable<string> deckSectionNames);

        protected abstract IEnumerable<File.FileConverter> GenerateCompatibleFileConverters();

        protected abstract IEnumerable<Webpage.WebpageConverter> GenerateCompatibleWebpageConverters();

        /// <summary>
        /// Checks if the GameGuid matches the ConverterGame Guid.  If it doesn't, an InvalidOperationException is thrown.
        /// </summary>
        /// <param name="converterGame">The ConverterGame instance to check for matching Game Guid</param>
        private void ThrowIfConverterGameDoesntMatch(ConverterGame converterGame)
        {
            if (converterGame.Game.Id != this.GameGuid)
            {
                throw new InvalidOperationException("converterGame is not the correct Game");
            }
        }
    }
}
