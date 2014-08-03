// -----------------------------------------------------------------------
// <copyright file="TextConvertEngine.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OCTGNDeckConverter.Model.ConvertEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class TextConverter
    {
        /// <summary>
        /// Converts user input text into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="sectionsText">A collection of section names (keys), and the user input text of all cards in the section (values)</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertText(Dictionary<string, string> sectionsText, Dictionary<Guid, ConverterSet> converterSets, IEnumerable<string> deckSectionNames)
        {
            Dictionary<string, IEnumerable<string>> sectionsLines = new Dictionary<string, IEnumerable<string>>();

            // Key = Section Name
            // Value = Section card lines as a blob of text
            foreach (KeyValuePair<string, string> section in sectionsText)
            {
                sectionsLines.Add(section.Key, TextConverter.SplitLines(section.Value));
            }

            ConverterDeck converterDeck = TextConverter.ConvertDeckWithSeparateSections(sectionsLines, deckSectionNames);
            converterDeck.PopulateConverterMappings(converterSets);
            return converterDeck;
        }

        /// <summary>
        /// Reads each line in each section, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="sectionLines">A collection of deck sections with Item1 as Name, and Item2 as a collection of lines of text which are cards</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        internal static ConverterDeck ConvertDeckWithSeparateSections(Dictionary<string, IEnumerable<string>> sectionLines, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            foreach (KeyValuePair<string, IEnumerable<string>> section in sectionLines)
            {
                ConverterSection converterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals(section.Key, StringComparison.InvariantCultureIgnoreCase));
                foreach (string line in section.Value)
                {
                    // The line is the Deck Name?  Record it.
                    string potentialDeckName = TextConverter.RegexMatch_DeckName(line);

                    if (potentialDeckName != null)
                    {
                        converterDeck.DeckName = potentialDeckName;
                    }
                    else
                    {
                        ConverterMapping potentialCard = TextConverter.ParseLineForCardAndQuantity(line);
                        
                        if (potentialCard != null)
                        {
                            converterSection.AddConverterMapping(potentialCard);
                        }
                    }
                }
            }

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }

        internal static ConverterMapping ParseLineForCardAndQuantity(string line)
        {
            // Is the line a Comment?
            if (RegexMatch_Comment(line) != null)
            {
                return null;
            }

            // The line is a regular main deck Card/Quantity entry, without any Set info?  Record it
            ConverterMapping potentialRegularCard = RegexMatch_RegularCard(line);
            if (potentialRegularCard != null)
            {
                return potentialRegularCard;
            }

            ConverterMapping potentialRegularCardQuantityAfterName = RegexMatch_RegularCardQuantityAfterName(line);
            if (potentialRegularCardQuantityAfterName != null)
            {
                return potentialRegularCardQuantityAfterName;
            }

            // The line is a main deck Card/Quantity entry with Set info?  Record it
            ConverterMapping potentialMWSMainDeckCard = File.MWS.RegexMatch_MWSMainDeckCard(line);
            if (potentialMWSMainDeckCard != null)
            {
                return potentialMWSMainDeckCard;
            }

            // The line is not a valid card entry
            return null;
        }

        #region Regex

        /// <summary>
        /// Regex pattern for determining if the text is a Comment, and extracting the Comment 
        /// (A comment line begins with '//')
        /// </summary>
        private static Regex Regex_Comment = new Regex(@"^\s*[/][/](.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns the Comment if line is properly formatted as a Comment, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>The Comment</returns>
        private static string RegexMatch_Comment(string line)
        {
            Match m = TextConverter.Regex_Comment.Match(line);
            return m.Success ?
                m.Groups[1].Value :
                null;
        }

        /// <summary>
        /// Regex pattern for determining if the text is the Deck-Name, and extracting the Deck-Name
        /// (A deck name line begins with '// NAME:')
        /// </summary>
        private static Regex Regex_DeckName = new Regex(@"^\s*[/][/]\s*[nN][aA][mM][eE][:]\s*(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns the Deck Name if line is properly formatted as a Deck Name comment, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>The Deck Name</returns>
        internal static string RegexMatch_DeckName(string line)
        {
            Match m = TextConverter.Regex_DeckName.Match(line);
            return m.Success ?
                m.Groups[1].Value :
                null;
        }

        /// <summary>
        /// Regex pattern for determining if the text is a regular Card, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularCard = new Regex(@"^\s*(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        public static ConverterMapping RegexMatch_RegularCard(string line)
        {
            Match m = Regex_RegularCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    RegexMatch_RemoveNameAppendendParenthesis(m.Groups[2].Value),
                    string.Empty,
                    int.Parse(m.Groups[1].Value)
                );
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Regex pattern for determining if the text is a regular Card, but with the quantity after the name, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularCardQuantityAfterName = new Regex(@"^\s*([a-zA-Z].+)\s+[xX]*\s*(\d+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Card, but with the quantity after the name, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        public static ConverterMapping RegexMatch_RegularCardQuantityAfterName(string line)
        {
            // Format: (Name #x)
            Match m = Regex_RegularCardQuantityAfterName.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    RegexMatch_RemoveNameAppendendParenthesis(m.Groups[1].Value),
                    string.Empty,
                    int.Parse(m.Groups[2].Value)
                );
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Regex pattern for determining if the text contains parenthesis at the end, and extracting the Card-Name and Inside-Parenthesis-Text.
        /// This is used on card Names once the Quantity and Set have already been removed.
        /// </summary>
        private static Regex Regex_RemoveNameAppendedParenthesis = new Regex(@"\s*(.+)\s*\((\(/\)\(/\))\)|\s*(.+)\s*\((.*)\)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns the card name with extra trailing garbage removed
        /// </summary>
        /// <param name="name">The name which may contain extra text in parenthesis</param>
        /// <returns>The Card Name with extra text in parenthesis removed</returns>
        internal static string RegexMatch_RemoveNameAppendendParenthesis(string name)
        {
            Match m = Regex_RemoveNameAppendedParenthesis.Match(name);
            if (m.Success)
            {
                string matchedName = m.Groups[1].Success ?
                    m.Groups[1].Value :
                    m.Groups[3].Value;

                string matchedParenthesisContents = m.Groups[2].Success ?
                    m.Groups[2].Value.Replace('’', '\'') :
                    m.Groups[4].Value.Replace('’', '\'');

                if (string.IsNullOrWhiteSpace(matchedParenthesisContents))
                {
                    return matchedName;
                }
                else
                {
                    // There was text in the parenthesis.  Check for special cases (some cards' Name includes parenthesis)

                    // TODO: In the future, get a list of all possible cards for the Game, and then extract the parenthesis from each of those for comparison
                    // That would be less brittle than hard-coding each special case here.
                    List<string> cardNamesWithParenthesis = new List<string>();
                    cardNamesWithParenthesis.Add(@"B.F.M. (Big Furry Monster)");  // MTG Set Unglued
                    cardNamesWithParenthesis.Add(@"Erase (Not the Urza's Legacy One)");  // MTG Set Unhinged

                    if (cardNamesWithParenthesis.Any(cnwp => cnwp.Equals(matchedParenthesisContents, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        // The parenthesis and its contents are actually part of the Card Name
                        return name;
                    }
                    else
                    {
                        return matchedName;
                    }
                }
            }
            else
            {
                return name;
            }
        }

        #endregion Regex

        /// <summary>
        /// Returns the input text split into an array, split on newlines.  Empty lines are not included.
        /// </summary>
        /// <param name="text">Text to be split on each new line</param>
        /// <returns>text split into an array, split on newlines.  Empty lines are not included.</returns>
        /// <seealso cref="http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net"/>
        public static string[] SplitLines(string text)
        {
            return text == null ?
                new string[] { } :
                text.Split
                (
                    new string[] { "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                );
        }
    }
}
