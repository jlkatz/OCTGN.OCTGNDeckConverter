using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OCTGNDeckConverter.Model.ConvertEngine.File
{
    public class Octgn : FileConverter
    {
        /// <summary>
        /// Gets the file extension string for the Octgn deck format
        /// </summary>
        protected override string Extension
        {
            get { return ".o8d"; }
        }

        /// <summary>
        /// Reads the OCTGN format file, and returns a ConverterDeck which is populated with all cards.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        /// <remarks>This will purposely ignore the Guids, which may help importing from OCTGN2 or other unofficial sets</remarks>
        public override ConverterDeck Convert(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);
            using (XmlTextReader reader = new XmlTextReader(fullPathName))
            {
                reader.WhitespaceHandling = WhitespaceHandling.None;

                XmlDocument xd = new XmlDocument();
                xd.Load(reader);

                XmlNode xnodDE = xd.DocumentElement;

                if (xnodDE.Name != "deck")
                {
                    throw new InvalidOperationException("File is not a Octgn Deck");
                }

                foreach (XmlNode child in xnodDE.ChildNodes)
                {
                    if (child.Name.Equals("section", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ConverterSection converterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals(child.Attributes.GetNamedItem("name").Value, StringComparison.InvariantCultureIgnoreCase));
                        foreach (XmlNode cardXmlNode in child.ChildNodes)
                        {
                            converterSection.AddConverterMapping
                            (
                                new ConverterMapping
                                (
                                    cardXmlNode.InnerText,
                                    string.Empty,
                                    int.Parse(cardXmlNode.Attributes.GetNamedItem("qty").Value)
                                )
                            );
                        }
                    }
                }
            }

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }
    }
}
