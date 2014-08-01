using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.File
{
    public class Apprentice : FileConverter
    {
        /// <summary>
        /// Gets the file extension string for the Apprentice deck format
        /// </summary>
        protected override string Extension 
        {
            get { return ".dec"; }
        }

        /// <summary>
        /// Reads the lines which is in the Apprentice format, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public override ConverterDeck Convert(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            // Apprentice's format is identical to MWS
            return MWS.ConvertMTGDeckWithSideBoardCardsListedEachLine(fullPathName, deckSectionNames);
        }
    }
}
