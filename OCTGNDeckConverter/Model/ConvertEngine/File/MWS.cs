using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCTGNDeckConverter.Model.ConvertEngine.File
{
    public class MWS : FileConverter
    {
        /// <summary>
        /// Gets the file extension string for the Magic Workstation deck format
        /// </summary>
        protected override string Extension
        {
            get { return ".mwDeck"; }
        }

        /// <summary>
        /// Reads the file which is in the MWS format, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public override ConverterDeck Convert(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            return MWS.ConvertMTGDeckWithSideBoardCardsListedEachLine(fullPathName, deckSectionNames);
        }

        /// <summary>
        /// Reads the file which has Sideboard cards denoted on each line, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public static ConverterDeck ConvertMTGDeckWithSideBoardCardsListedEachLine(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            string contents = System.IO.File.ReadAllText(fullPathName);
            return MWS.ConvertMTGDeckWithSideBoardCardsListedEachLine(TextConverter.SplitLines(contents), deckSectionNames);
        }

        /// <summary>
        /// Reads the lines which has Sideboard cards denoted on each line, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public static ConverterDeck ConvertMTGDeckWithSideBoardCardsListedEachLine(IEnumerable<string> lines, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            ConverterSection mtgMainDeckConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Main", StringComparison.InvariantCultureIgnoreCase));
            ConverterSection mtgSideboardConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase));

            foreach (string line in lines)
            {
                // The line is the Deck Name?  Record it.
                string potentialDeckName = TextConverter.RegexMatch_DeckName(line);

                if (potentialDeckName != null)
                {
                    converterDeck.DeckName = potentialDeckName;
                }
                else
                {
                    // Ordering: The most specific pattern is listed first, and each more generalized pattern follows
                    if (RegexMatch_MWSSideBoardCard(line) != null)
                    {
                        // The line is a MWS sideboard "SB: Quantity [SET] Card" entry
                        mtgSideboardConverterSection.AddConverterMapping(RegexMatch_MWSSideBoardCard(line));
                    }
                    else if (RegexMatch_MWSMainDeckCard(line) != null)
                    {
                        // The line is a MWS main deck "Quantity [SET] Card" entry
                        mtgMainDeckConverterSection.AddConverterMapping(RegexMatch_MWSMainDeckCard(line));
                    }
                    else if (RegexMatch_RegularMTGSideBoardCard(line) != null)
                    {
                        // The line is a regular sideboard "Quantity Card" entry, without any Set info
                        mtgSideboardConverterSection.AddConverterMapping(RegexMatch_RegularMTGSideBoardCard(line));
                    }
                    else
                    {
                        ConverterMapping potentialCard = TextConverter.ParseLineForCardAndQuantity(line);
                        if (potentialCard != null)
                        {
                            mtgMainDeckConverterSection.AddConverterMapping(potentialCard);
                        }
                        else
                        {
                            // The line is not a valid card entry
                        }
                    }
                }
            }

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }

        /// <summary>
        /// Returns a value indicating whether the lines from a file are can be converted as MWS format or not
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to check for MWS format</param>
        /// <returns>a value indicating whether the lines from a file are in the MWS format or not</returns>
        public static bool DoesFileMatchMWSDeckFormat(IEnumerable<string> lines)
        {
            // Do sideboard cards appear to be specified on each line?  If so, then it is a MWS formatted deck
            return lines.Any(l => RegexMatch_RegularMTGSideBoardCard(l) != null || RegexMatch_MWSSideBoardCard(l) != null);
        }

        #region Regex

        /// <summary>
        /// Regex pattern for determining if the text is a Magic Workstation Main-Deck-Card, and extracting the Name, Set, and Quantity
        /// </summary>
        private static Regex Regex_MWSMainDeckCard = new Regex(@"^\s*(\d+)\s+\[(.*?)\]\s+(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a MWS Main Deck Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        internal static ConverterMapping RegexMatch_MWSMainDeckCard(string line)
        {
            Match m = Regex_MWSMainDeckCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    TextConverter.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[3].Value),
                    m.Groups[2].Value,
                    int.Parse(m.Groups[1].Value)
                );
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Regex pattern for determining if the text is a Magic Workstation Sideboard-Card, and extracting the Name, Set, and Quantity
        /// </summary>
        private static Regex Regex_MWSSideBoardCard = new Regex(@"^\s*[sS][bB][:]\s+(\d+)\s+\[(.*?)\]\s+(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a MWS Sideboard Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_MWSSideBoardCard(string line)
        {
            Match m = Regex_MWSSideBoardCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    TextConverter.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[3].Value),
                    m.Groups[2].Value,
                    int.Parse(m.Groups[1].Value)
                );
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Regex pattern for determining if the text is a regular MTG Sideboard-Card, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularMTGSideboardCard = new Regex(@"^\s*[sS][bB][:]\s+(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+|Æ.+|æ.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular MTG Sideboard Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_RegularMTGSideBoardCard(string line)
        {
            Match m = Regex_RegularMTGSideboardCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    TextConverter.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[2].Value),
                    string.Empty,
                    int.Parse(m.Groups[1].Value)
                );
            }
            else
            {
                return null;
            }
        }

        #endregion Regex
    }
}
