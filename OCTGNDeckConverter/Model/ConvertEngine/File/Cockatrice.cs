using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OCTGNDeckConverter.Model.ConvertEngine.File
{
    public class Cockatrice : FileConverter
    {
        /// <summary>
        /// Gets the file extension string for the Cockatrice deck format
        /// </summary>
        protected override string Extension
        {
            get { return ".cod"; }
        }

        /// <summary>
        /// Reads the Cockatrice format file, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public override ConverterDeck Convert(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);
            using (XmlTextReader reader = new XmlTextReader(fullPathName))
            {
                reader.WhitespaceHandling = WhitespaceHandling.None;

                XmlDocument xd = new XmlDocument();
                xd.Load(reader);

                XmlNode xnodDE = xd.DocumentElement;

                if (xnodDE.Name != "cockatrice_deck")
                {
                    throw new InvalidOperationException("File is not a Cockatrice Deck");
                }

                ConverterSection mtgMainDeckConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Main", StringComparison.InvariantCultureIgnoreCase));
                ConverterSection mtgSideboardConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase));

                foreach (XmlNode child in xnodDE.ChildNodes)
                {
                    if (child.Name.Equals("deckname", StringComparison.InvariantCultureIgnoreCase))
                    {
                        converterDeck.DeckName = child.InnerText;
                    }
                    else if (child.Name.Equals("zone", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (child.Attributes.GetNamedItem("name").Value.Equals("main", StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (XmlNode cardXmlNode in child.ChildNodes)
                            {
                                mtgMainDeckConverterSection.AddConverterMapping
                                (
                                    new ConverterMapping
                                    (
                                        cardXmlNode.Attributes.GetNamedItem("name").Value,
                                        string.Empty,
                                        int.Parse(cardXmlNode.Attributes.GetNamedItem("number").Value)
                                    )
                                );
                            }
                        }
                        else if (child.Attributes.GetNamedItem("name").Value.Equals("side", StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (XmlNode cardXmlNode in child.ChildNodes)
                            {
                                mtgSideboardConverterSection.AddConverterMapping
                                (
                                    new ConverterMapping
                                    (
                                        cardXmlNode.Attributes.GetNamedItem("name").Value,
                                        string.Empty,
                                        int.Parse(cardXmlNode.Attributes.GetNamedItem("number").Value)
                                    )
                                );
                            }
                        }
                    }
                }
            }

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }
    }
}
