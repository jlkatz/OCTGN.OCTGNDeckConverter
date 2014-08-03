using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public class ArcaneWonders_com : WebpageConverter
    {
        /// <summary>
        /// Gets the base URL for the arcanewonders.com website
        /// </summary>
        protected override string BaseURL
        {
            get { return "forum.arcanewonders.com"; }
        }

        /// <summary>
        /// Converts a MW URL from arcanewonders.com into a ConverterDeck which is populated with all cards and deck name.
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
            object htmlWebInstance = HtmlAgilityPackWrapper.HtmlWeb_CreateInstance();
            object htmlDocumentInstance = HtmlAgilityPackWrapper.HtmlWeb_InvokeMethod_Load(htmlWebInstance, url);
            object htmlDocument_DocumentNode = HtmlAgilityPackWrapper.HtmlDocument_GetProperty_DocumentNode(htmlDocumentInstance);

            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            // Find the div with id 'spellbook' because all deck data is contained inside it
            object spellbookDiv = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectSingleNode(htmlDocument_DocumentNode, "//div[@id='spellbook']");

            if (spellbookDiv == null)
            {
                throw new InvalidOperationException("Could not find the html div 'spellbook', are you sure this webpage has a deck?");
            }

            // Insert the Deck Name if available
            object spellbooknameSpan = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectSingleNode(spellbookDiv, "//span[@id='spellbookname']");
            if (spellbooknameSpan != null)
            {
                converterDeck.DeckName = HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(spellbooknameSpan);
            }

            // Convert the Mage card
            object firstMageSpan = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectNodes(spellbookDiv, "//span[@id='mage']").FirstOrDefault();
            if (firstMageSpan != null)
            {
                ConverterSection mageSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Mage", StringComparison.InvariantCultureIgnoreCase));
                mageSection.AddConverterMapping(new ConverterMapping(
                    HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(firstMageSpan), 
                    string.Empty, 
                    1));
            }

            // The div 'spells' contains all the cards, grouped by spell class
            object spellsDiv = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectSingleNode(spellbookDiv, "//div[@id='spells']");

            // Get a collection of all the span nodes inside the 'spells' div
            IEnumerable<object> cardAndSectionSpans = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectNodes(spellsDiv, "span");

            ConverterSection currentConverterSection = null;
            foreach (object cardOrSectionSpan in cardAndSectionSpans)
            {
                IEnumerable<object> attributes = HtmlAgilityPackWrapper.HtmlNode_GetProperty_Attributes(cardOrSectionSpan);
                string className = string.Empty;
                foreach (object attribute in attributes)
                {
                    if (HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Name(attribute).Equals("class", StringComparison.InvariantCultureIgnoreCase))
                    {
                        className = HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Value(attribute);
                        break;
                    }
                }

                if (className.Equals("spellClass", StringComparison.InvariantCultureIgnoreCase))
                {
                    string currentSpellClass = HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(cardOrSectionSpan);
                    currentConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals(currentSpellClass, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    string quantityAndCardString = HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(cardOrSectionSpan);
                    ConverterMapping converterMapping = TextConverter.RegexMatch_RegularCard(quantityAndCardString);
                    currentConverterSection.AddConverterMapping(converterMapping);
                }
            }

            return converterDeck;
        }
    }
}
