using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public class DeckLists_net : WebpageConverter
    {
        /// <summary>
        /// Gets the base URL for the decklists.net website
        /// </summary>
        protected override string BaseURL
        {
            get { return "decklists.net"; }
        }

        /// <summary>
        /// Converts a URL from decklists.net into a ConverterDeck which is populated with all cards and deck name.
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
            // Note, it doesn't get the Deck Name properly yet
            return WebpageConverter.ConvertURLUsingDeckIDInURL(
                url, 
                "deckId",
                @"http://www.decklists.net/index.php?option=com_ohmydeckdb&view=deck&controller=deck&deckId=",
                @"&layout=download&tmpl=component&frm=mws",
                deckSectionNames,
                convertGenericFileFunc);
        }
    }
}
