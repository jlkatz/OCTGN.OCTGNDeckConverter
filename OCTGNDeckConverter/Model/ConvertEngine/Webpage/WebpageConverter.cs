using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public abstract class WebpageConverter
    {
        /// <summary>
        /// Gets the base URL string for the Website
        /// </summary>
        protected abstract string BaseURL { get; }

        /// <summary>
        /// Converts a URL into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <param name="convertGenericFileFunc">
        /// Function to convert a collection of lines from a deck file into a ConverterDeck.  
        /// Used when downloading a Deck File from a webpage instead of scraping.
        /// </param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        public abstract ConverterDeck Convert(
            string url,
            IEnumerable<string> deckSectionNames,
            Func<IEnumerable<string>, IEnumerable<string>, ConverterDeck> convertGenericFileFunc);

        /// <summary>
        /// Returns a value indicating whether the url matches this Website's base URL or not
        /// </summary>
        /// <param name="url">The url to attempt to match</param>
        /// <returns>a value indicating whether the url matches this Website's base URL or not</returns>
        public bool URLMatches(string url)
        {
            return url.ToLowerInvariant().Contains(this.BaseURL.ToLowerInvariant());
        }

        /// <summary>
        /// Converts a URL which needs to be altered to get the download link, then uses it to return a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="inputURL">The originally requested URL of the Deck</param>
        /// <param name="deckIdKey">The key for the URL Parameter which contains the ID value for the deck</param>
        /// <param name="urlPrepend">The URL part which precedes the deckIdKey</param>
        /// <param name="urlPostpend">The URL part which follows the deckIdKey</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <param name="convertGenericFileFunc">
        /// Function to convert a collection of lines from a deck file into a ConverterDeck.  
        /// Used when downloading a Deck File from a webpage instead of scraping.
        /// </param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        protected static ConverterDeck ConvertURLUsingDeckIDInURL(string inputURL, string deckIdKey, string urlPrepend, string urlPostpend, IEnumerable<string> deckSectionNames, Func<IEnumerable<string>, IEnumerable<string>, ConverterDeck> convertGenericFileFunc)
        {
            int deckID;
            string deckIDString;

            // Try to get the value for the parameter with the name in deckIdKey
            if (WebpageConverter.GetParams(inputURL).TryGetValue(deckIdKey, out deckIDString))
            {
                // Try to convert the found value into an integer
                if (int.TryParse(deckIDString, out deckID))
                {
                    // Construct a URL which is the download link for the Deck, then convert it
                    string downloadableDeckURL = urlPrepend + deckID + urlPostpend;
                    return WebpageConverter.ConvertDownloadURL(downloadableDeckURL, deckSectionNames, convertGenericFileFunc);
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads a URL which is a download link to a text file or equivalent, then parses it to return a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck which is a download link to a text file or equivalent</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <param name="convertGenericFileFunc">
        /// Function to convert a collection of lines from a deck file into a ConverterDeck.  
        /// Used when downloading a Deck File from a webpage instead of scraping.
        /// </param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        protected static ConverterDeck ConvertDownloadURL(string url, IEnumerable<string> deckSectionNames, Func<IEnumerable<string>, IEnumerable<string>, ConverterDeck> convertGenericFileFunc)
        {
            // Get the file and content
            Tuple<string, IEnumerable<string>> nameAndLines = WebpageConverter.ReadURLFileToLines(url);

            // Convert as SideBoardCardsListedEachLine
            ConverterDeck converterDeck = convertGenericFileFunc(nameAndLines.Item2, deckSectionNames);

            // If no Deck Name was given, use the filename
            if (string.IsNullOrWhiteSpace(converterDeck.DeckName))
            {
                converterDeck.DeckName = nameAndLines.Item1;
            }

            return converterDeck;
        }

        /// <summary>
        /// Reads the file at the URL location, and returns both the filename and an array of all lines of text found within
        /// </summary>
        /// <param name="url">Location to read into a filename and an array of lines</param>
        /// <returns>Returns 1: filename, 2: all text as array of lines</returns>
        private static Tuple<string, IEnumerable<string>> ReadURLFileToLines(string url)
        {
            List<string> lines = new List<string>();
            string filename;

            System.Net.WebRequest wreq = System.Net.WebRequest.Create(url);

            using (System.Net.WebResponse wresp = wreq.GetResponse())
            {
                filename = wresp.Headers["Content-Disposition"];  // "attachment; filename=\"Draft #1396152 deck.mwDeck\""
                using (System.IO.Stream s = wresp.GetResponseStream())
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(s))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }

            return new Tuple<string, IEnumerable<string>>(filename, lines);
        }

        /// <summary>
        /// Returns the Parameters found in the given URL as a Dictionary of Key-Value strings
        /// </summary>
        /// <param name="uri">The URI string to parse for Parameters</param>
        /// <returns>Dictionary of Key-Value strings of the parameters</returns>
        /// <see cref="http://codereview.stackexchange.com/a/1592"/>
        private static Dictionary<string, string> GetParams(string uri)
        {
            var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            return matches.Cast<Match>().ToDictionary
            (
                m => Uri.UnescapeDataString(m.Groups[2].Value),
                m => Uri.UnescapeDataString(m.Groups[3].Value)
            );
        }
    }
}
