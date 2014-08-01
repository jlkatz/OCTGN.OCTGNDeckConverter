using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Game
{
    public class CoC : GameConverter
    {
        public static Guid GameGuidStatic = Guid.Parse("43054c18-2362-43e0-a434-72f8d0e8477c");

        /// <summary>
        /// Gets the Guid identifier for CoC
        /// </summary>
        public override Guid GameGuid
        {
            get { return CoC.GameGuidStatic; }
        }

        /// <summary>
        /// Converts a text file that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings defined, but not yet populated with potential matching OCTGN cards</returns>
        protected override ConverterDeck ConvertFile(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            throw new NotImplementedException("There are no compatible File Converters for CoC yet");
        }

        /// <summary>
        /// Converts a URL that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings defined, but not yet populated with potential matching OCTGN cards</returns>
        protected override ConverterDeck ConvertURL(string url, IEnumerable<string> deckSectionNames)
        {
            throw new NotImplementedException("There are no compatible File Converters for CoC yet");
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

        protected override IEnumerable<File.FileConverter> GenerateCompatibleFileConverters()
        {
            // There are no compatible File Converters for CoC yet.
            return new List<File.FileConverter>();
        }

        protected override IEnumerable<Webpage.WebpageConverter> GenerateCompatibleWebpageConverters()
        {
            // There are no compatible File Converters for CoC yet.
            return new List<Webpage.WebpageConverter>();
        }
    }
}
