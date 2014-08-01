using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public class TappedOut_net : WebpageConverter
    {
        /// <summary>
        /// Regex pattern for extracting the Deck ID from a tappedout.net URL
        /// </summary>
        private static Regex Regex_tappedout_net = new Regex(@"^.*tappedout\.net/mtg-decks/(.*)/.*", RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets the base URL for the tappedout.net website
        /// </summary>
        protected override string BaseURL
        {
            get { return "tappedout.net"; }
        }

        /// <summary>
        /// Converts a URL from tappedout.net into a ConverterDeck which is populated with all cards and deck name.
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
            Match m = Regex_tappedout_net.Match(url);
            if (m.Success)
            {
                string deckName = m.Groups[1].Value;
                return WebpageConverter.ConvertDownloadURL(
                    @"http://tappedout.net/mtg-decks/" + deckName + @"/?fmt=txt",
                    deckSectionNames,
                    convertGenericFileFunc);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

    }
}
