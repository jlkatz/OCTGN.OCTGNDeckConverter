using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public class CCGDecks_com : WebpageConverter
    {
        /// <summary>
        /// Gets the base URL for the ccgdecks.com website
        /// </summary>
        protected override string BaseURL
        {
            get { return "ccgdecks.com"; }
        }

        /// <summary>
        /// Converts a URL from ccgdecks.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <param name="convertGenericFileFunc">
        /// Function to convert a collection of lines from a deck file into a ConverterDeck.  
        /// Used when downloading a Deck File from a webpage instead of scraping.
        /// </param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        public override ConverterDeck Convert(
            string url,
            IEnumerable<string> deckSectionNames,
            Func<IEnumerable<string>, IEnumerable<string>, ConverterDeck> convertGenericFileFunc)
        {
            return WebpageConverter.ConvertURLUsingDeckIDInURL(
                url,
                "id",
                @"http://ccgdecks.com/to_mws.php?id=",
                string.Empty,
                deckSectionNames,
                convertGenericFileFunc);
        }
    }
}
