using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCTGNDeckConverter.Model.ConvertEngine.Game
{
    public class MTG : GameConverter
    {
        public static Guid GameGuidStatic = Guid.Parse("A6C8D2E8-7CD8-11DD-8F94-E62B56D89593");

        /// <summary>
        /// Gets the Guid identifier for MTG
        /// </summary>
        public override Guid GameGuid
        {
            get { return MTG.GameGuidStatic; }
        }

        /// <summary>
        /// Converts a text file that is known to match this Game into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings defined, but not yet populated with potential matching OCTGN cards</returns>
        protected override ConverterDeck ConvertFile(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = null;
            string extension = System.IO.Path.GetExtension(fullPathName);

            // Try to find a pre-defined FileConverter to handle ConvertFile
            File.FileConverter fileConverter = this.FindMatchingFileConverter(extension);

            if (fileConverter != null)
            {
                converterDeck = fileConverter.Convert(fullPathName, deckSectionNames);
            }
            else
            {
                // No pre-defined FileConverter was found, so the file is in another format, probably '.txt'
                string contents = System.IO.File.ReadAllText(fullPathName);
                string[] lines = TextConverter.SplitLines(contents);
                converterDeck = MTG.ConvertGenericFile(lines, deckSectionNames);
            }

            return converterDeck;
        }

        public static ConverterDeck ConvertGenericFile(IEnumerable<string> lines, IEnumerable<string> deckSectionNames)
        {
            if (File.MWS.DoesFileMatchMWSDeckFormat(lines))
            {
                // Since the contents match the MWS deck format, use that converter
                return File.MWS.ConvertMTGDeckWithSideBoardCardsListedEachLine(lines, deckSectionNames);
            }
            else
            {
                //// Look for the 'Sideboard' section

                List<string> mainDeckLines = new List<string>();
                List<string> sideboardLines = new List<string>();
                bool sideboardStringPassed = false;

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Match match_StartsWithSideboard = MTG.Regex_StartsWithSideboard.Match(line);
                        if (match_StartsWithSideboard.Success)
                        {
                            sideboardStringPassed = true;
                        }
                        else
                        {
                            if (!sideboardStringPassed)
                            {
                                mainDeckLines.Add(line);
                            }
                            else
                            {
                                sideboardLines.Add(line);
                            }
                        }
                    }
                }

                Dictionary<string, IEnumerable<string>> sectionLines = new Dictionary<string, IEnumerable<string>>();

                sectionLines.Add("Main", mainDeckLines);
                sectionLines.Add("Sideboard", sideboardLines);

                return TextConverter.ConvertDeckWithSeparateSections(sectionLines, deckSectionNames);
            }
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
                converterDeck = webpageConverter.Convert(url, deckSectionNames, ConvertGenericFile);
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
        /// Generates a collection of FileConverters containing all of the possible File formats for MTG
        /// </summary>
        protected override IEnumerable<File.FileConverter> GenerateCompatibleFileConverters()
        {
            return new List<File.FileConverter>()
            {
                new File.Apprentice(),
                new File.Cockatrice(),
                new File.MWS(),
                new File.Octgn(),
            };
        }

        /// <summary>
        /// Generates a collection of WebpageConverters containing all of the possible Websites for MTG
        /// </summary>
        protected override IEnumerable<Webpage.WebpageConverter> GenerateCompatibleWebpageConverters()
        {
            return new List<Webpage.WebpageConverter>()
            {
                new Webpage.CCGDecks_com(),
                new Webpage.DeckCheck_de(),
                new Webpage.DeckLists_net(),
                new Webpage.EssentialMagic_com(),
                new Webpage.StarCityGames_com(),
                new Webpage.TappedOut_net(),
                new Webpage.TCGPlayer_com(),
            };
        }

        /// <summary>
        /// Regex pattern for determining if the text begins with Sideboard.  This signifies that subsequent cards all reside in the Sideboard.
        /// </summary>
        private static Regex Regex_StartsWithSideboard = new Regex(@"^\s*[sS][iI][dD][eE][bB][oO][aA][rR][dD]", RegexOptions.IgnoreCase);
    }
}
