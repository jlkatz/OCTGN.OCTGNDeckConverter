using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCTGNDeckConverter.Model.ConvertEngine.File
{
    public class SpellBookBuilderText : FileConverter
    {
        protected override string Extension
        {
            get { return ".txt"; }
        }

        /// <summary>
        /// Reads the file which is in the SpellBookBuilder Text format, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public override ConverterDeck Convert(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            string contents = System.IO.File.ReadAllText(fullPathName);
            IEnumerable<string> lines = TextConverter.SplitLines(contents);
            return this.Convert(lines, deckSectionNames);
        }

        /// <summary>
        /// Reads the lines of text which is in the SpellBookBuilder Text format, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="lines">The lines of text of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public ConverterDeck Convert(IEnumerable<string> lines, IEnumerable<string> deckSectionNames)
        {
            string deckName = lines.FirstOrDefault(l => SpellBookBuilderText.RegexMatch_DeckName(l) != null);

            string mage;
            try
            {
                mage = (from l in lines select SpellBookBuilderText.RegexMatch_Mage(l)).First(m => m != null);
            }
            catch (Exception e)
            {
                e.Data.Add("Description", "Could not find a Mage in this deck.");
                throw;
            }

            List<ParsedCard> cards = new List<ParsedCard>();
            foreach (string line in lines)
            {
                ParsedCard potentialCard = SpellBookBuilderText.RegexMatch_SBBTCard(line);
                if (potentialCard != null)
                {
                    cards.Add(potentialCard);
                }
            }

            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);
            
            // Insert the Deck Name
            if (deckName != null)
            {
                converterDeck.DeckName = deckName;
            }

            // Add the Mage with quantity of 1
            ConverterSection mageSection = converterDeck.ConverterSections.First(s => s.SectionName.Equals("Mage", StringComparison.InvariantCultureIgnoreCase));
            mageSection.AddConverterMapping(new ConverterMapping(mage, string.Empty, 1));

            // Add each other card with quantity to the corresponding Section 
            foreach (ParsedCard card in cards)
            {
                ConverterSection correspondingSection = converterDeck.ConverterSections.First(s => s.SectionName.Equals(card.Section, StringComparison.InvariantCultureIgnoreCase));
                correspondingSection.AddConverterMapping(new ConverterMapping(card.Name, string.Empty, card.Quantity));
            }

            return converterDeck;
        }

        /// <summary>
        /// Returns a value indicating whether the lines from a file are can be converted as SpellBookBuilder Text format or not
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to check for SpellBookBuilder Text format</param>
        /// <returns>a value indicating whether the lines from a file are in the SpellBookBuilder format or not</returns>
        /// <remarks>SpellBookBuilder formatted files can be created at http://forum.arcanewonders.com/sbb/index.php</remarks>
        public static bool DoesFileMatchSpellBookBuilderTextDeckFormat(IEnumerable<string> lines)
        {
            // The deck must contain a Mage, and at least one other card
            return
                lines.Any(l => RegexMatch_Mage(l) != null) &&
                lines.Any(l => RegexMatch_SBBTCard(l) != null);
        }

        #region Regex

        /// <summary>
        /// Regex pattern for determining if the text is a SpellBookBuilder Text Card, and extracting the Name, Section, Quantity, and Cost
        /// </summary>
        private static Regex Regex_SBBTCardCard = new Regex(@"^\s*([^\t|\n]+)\t([^\t|\n]+)\t(\d+)\t([^\t|\n]+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ParsedCard if the line is properly formatted as a SpellBookBuilder Text Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ParsedCard instance with info from the line if properly formatted, null otherwise</returns>
        private static ParsedCard RegexMatch_SBBTCard(string line)
        {
            Match m = Regex_SBBTCardCard.Match(line);
            if (m.Success)
            {
                return new ParsedCard()
                {
                    Name = m.Groups[1].Value,
                    Section = m.Groups[2].Value,
                    Quantity = Int32.Parse(m.Groups[3].Value),
                    Cost = m.Groups[4].Value.Equals("NaN", StringComparison.InvariantCultureIgnoreCase) ? 0 : Int32.Parse(m.Groups[4].Value),
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Regex pattern for determining if the text is the Deck-Name, and extracting the Deck-Name
        /// (A deck name line begins with 'Name:')
        /// </summary>
        private static Regex Regex_DeckName = new Regex(@"^\s*[nN][aA][mM][eE][:]\s*(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns the Deck Name if line is properly formatted as a Deck Name line, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>The Deck Name</returns>
        private static string RegexMatch_DeckName(string line)
        {
            Match m = SpellBookBuilderText.Regex_DeckName.Match(line);
            return m.Success ?
                m.Groups[1].Value :
                null;
        }

        /// <summary>
        /// Regex pattern for determining if the text is the Mage, and extracting the Mage name
        /// (A mage line begins with 'Mage:')
        /// </summary>
        private static Regex Regex_Mage = new Regex(@"^\s*[mM][aA][gG][eE][:]\s*(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns the Mage if line is properly formatted as a Mage line, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>The Mage Name</returns>
        private static string RegexMatch_Mage(string line)
        {
            Match m = SpellBookBuilderText.Regex_Mage.Match(line);
            return m.Success ?
                m.Groups[1].Value :
                null;
        }

        #endregion Regex

        /// <summary>
        /// Represents the data parsed for a Card in the SpellBookBuilder Text format
        /// </summary>
        private class ParsedCard
        {
            public int Cost { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string Section { get; set; }
        }
    }
}
