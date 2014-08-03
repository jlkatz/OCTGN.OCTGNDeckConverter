using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Game
{
    public class MW : GameConverter
    {
        public static Guid GameGuidStatic = Guid.Parse("9acef3d0-efa8-4d3f-a10c-54812baecdda");

        /// <summary>
        /// Gets the Guid identifier for MW
        /// </summary>
        
        public override Guid GameGuid
        {
            get { return MW.GameGuidStatic; }
        }

        /// <summary>
        /// Converts a text file that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings defined, but not yet populated with potential matching OCTGN cards</returns>
        protected override ConverterDeck ConvertFile(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            throw new NotImplementedException("There are no compatible File Converters for MW yet");
        }

        /// <summary>
        /// Converts a URL that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings defined, but not yet populated with potential matching OCTGN cards</returns>
        protected override ConverterDeck ConvertURL(string url, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = null;

            // Try to find a pre-defined WebpageConverter to handle ConvertFile
            Webpage.WebpageConverter webpageConverter = this.FindMatchingWebpageConverter(url);

            if (webpageConverter != null)
            {
                converterDeck = webpageConverter.Convert(url, deckSectionNames, null);
            }
            else
            {
                throw new InvalidOperationException("There was a problem importing the deck from the given url, or the website has not been implemented yet");
            }

            return converterDeck;
        }

        /// <summary>
        /// Converts user input text that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="sectionsText">A collection of section names (keys), and the user input text of all cards in the section (values)</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        protected override ConverterDeck ConvertText(Dictionary<string, string> sectionsText, Dictionary<Guid, ConverterSet> converterSets, IEnumerable<string> deckSectionNames)
        {
            return TextConverter.ConvertText(sectionsText, converterSets, deckSectionNames);
        }

        /// <summary>
        /// Generates a collection of FileConverters containing all of the possible File formats for MW
        /// </summary>
        protected override IEnumerable<File.FileConverter> GenerateCompatibleFileConverters()
        {
            // There are no compatible File Converters for MW yet.
            return new List<File.FileConverter>();
        }

        /// <summary>
        /// Generates a collection of WebpageConverters containing all of the possible Websites for MW
        /// </summary>
        protected override IEnumerable<Webpage.WebpageConverter> GenerateCompatibleWebpageConverters()
        {
            return new List<Webpage.WebpageConverter>()
            {
                new Webpage.ArcaneWonders_com(),
            };
        }
    }
}
