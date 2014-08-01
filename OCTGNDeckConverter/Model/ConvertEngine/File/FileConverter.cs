using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.File
{
    public abstract class FileConverter
    {
        /// <summary>
        /// Gets the file extension string for the deck format
        /// </summary>
        protected abstract string Extension { get; }

        /// <summary>
        /// Parses the lines for sections and cards, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        public abstract ConverterDeck Convert(string fullPathName, IEnumerable<string> deckSectionNames);

        /// <summary>
        /// Returns a value indicating whether the specified extension is a match for this FileConverter or not
        /// </summary>
        /// <param name="extension">The extention to check if matches this FileConverter</param>
        /// <returns>A value indicating whether the specified extension is a match for this FileConverter or not</returns>
        public bool ExtensionMatches(string extension)
        {
            return this.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
