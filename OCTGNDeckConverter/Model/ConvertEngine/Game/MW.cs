using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
            ConverterDeck converterDeck = null;
            string contents = System.IO.File.ReadAllText(fullPathName);
            IEnumerable<string> lines = TextConverter.SplitLines(contents);
            if (File.SpellBookBuilderText.DoesFileMatchSpellBookBuilderTextDeckFormat(lines))
            {
                File.SpellBookBuilderText spellBookBuilderTextConverter = this.CompatibleFileConverters.First() as File.SpellBookBuilderText;
                converterDeck = spellBookBuilderTextConverter.Convert(fullPathName, deckSectionNames);
            }
            else
            {
                // The file format didn't match any known MW format, so just try the generic format
                converterDeck = MW.ConvertGenericFile(lines, deckSectionNames);

            }
            MW.AddMageStatsCard(converterDeck);
            return converterDeck;
        }

        private static ConverterDeck ConvertGenericFile(IEnumerable<string> lines, IEnumerable<string> deckSectionNames)
        {
            Dictionary<string, IEnumerable<string>> sectionLines = new Dictionary<string, IEnumerable<string>>();
            foreach(string sectionName in deckSectionNames)
            {
                sectionLines.Add(sectionName, new List<string>());
            }
            IEnumerable<string> currentSection = null;  // The first line needs to be a Section name

            foreach (string line in lines)
            {
                string correspondingSectionName = sectionLines.Keys.FirstOrDefault(sl => line.Trim().Equals(sl, StringComparison.InvariantCultureIgnoreCase));
                if (correspondingSectionName == null)
                {
                    ((List<string>)currentSection).Add(line);
                }
                else
                {
                    currentSection = sectionLines[correspondingSectionName];
                }
            }

            return TextConverter.ConvertDeckWithSeparateSections(sectionLines, deckSectionNames);
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

            MW.AddMageStatsCard(converterDeck);
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
            ConverterDeck converterDeck = TextConverter.ConvertText(sectionsText, converterSets, deckSectionNames);
            MW.AddMageStatsCard(converterDeck);
            return converterDeck;
        }

        /// <summary>
        /// Generates a collection of FileConverters containing all of the possible File formats for MW
        /// </summary>
        protected override IEnumerable<File.FileConverter> GenerateCompatibleFileConverters()
        {
            return new List<File.FileConverter>()
            {
                new File.SpellBookBuilderText(),
            };
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

        /// <summary>
        /// Adds a Mage Stats card to accompany the Mage card, since it is required in OCTGN
        /// </summary>
        private static void AddMageStatsCard(ConverterDeck converterDeck)
        {
            ConverterSection mageSection = converterDeck.ConverterSections.First(s => s.SectionName.Equals("Mage", StringComparison.InvariantCultureIgnoreCase));

            if (mageSection.SectionMappings.Any(sm => sm.CardName.EndsWith(" Stats")))
            {
                // The corresponding Stats card already appears to be added
                return;
            }

            string mageName = mageSection.SectionMappings.First().CardName;
            
            if (MW.RegexMatch_WizardSubtype(mageName))
            {
                // No matter what the subtype of Wizard is, it should have a 'Wizard Stats' card
                mageName = "Wizard";
            }

            mageSection.AddConverterMapping(new ConverterMapping(mageName + " Stats", string.Empty, 1));
        }

        /// <summary>
        /// Regex pattern for determining if the text is in the format 'Wizard (anything)'.
        /// </summary>
        private static Regex Regex_WizardSubtype = new Regex(@"^\s*[wW][iI][zZ][aA][rR][dD]\s\((.+)\)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a value indicating whether the text is in the format 'Wizard (anything)' or not
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>a value indicating whether the text is in the format 'Wizard (anything)' or not</returns>
        public static bool RegexMatch_WizardSubtype(string line)
        {
            return MW.Regex_WizardSubtype.Match(line).Success;
        }
    }
}
