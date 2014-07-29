// -----------------------------------------------------------------------
// <copyright file="ConvertEngine.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OCTGNDeckConverter.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Converts an input location (File, URL, string) into a collection of ConverterMapping cards
    /// </summary>
    public static class ConvertEngine
    {
        /// <summary>
        /// File extension for MTG Apprentice Deck files
        /// </summary>
        public const string FileExtensionApprentice = ".dec";

        /// <summary>
        /// File extension for MTG Magic Workstation Deck files
        /// </summary>
        public const string FileExtensionMWS = ".mwDeck";

        /// <summary>
        /// File extension for MTG Cockatrice Deck files
        /// </summary>
        public const string FileExtensionCockatrice = ".cod";

        /// <summary>
        /// File extension for OCTGN Deck files
        /// </summary>
        public const string FileExtensionOctgn = ".o8d";

        /// <summary>
        /// Converts a text file that is known to be MTG into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="fullPathName">The full path name of the MTG Deck file to convert</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertMTGFile(string fullPathName, Dictionary<Guid, ConverterSet> converterSets, IEnumerable<string> deckSectionNames)
        {
            string extension = System.IO.Path.GetExtension(fullPathName);
            string text = System.IO.File.ReadAllText(fullPathName);
            string[] lines = ConvertEngine.SplitLines(text);
            ConverterDeck converterDeck = null;

            if
            (
                extension.Equals(FileExtensionMWS, StringComparison.InvariantCultureIgnoreCase) ||
                extension.Equals(FileExtensionApprentice, StringComparison.InvariantCultureIgnoreCase)
            )
            {
                converterDeck = ConvertEngine.ConvertMTGDeckWithSideBoardCardsListedEachLine(lines, deckSectionNames);
            }
            else if (extension.Equals(FileExtensionCockatrice, StringComparison.InvariantCultureIgnoreCase))
            {
                converterDeck = ConvertEngine.ConvertCockatrice(fullPathName, deckSectionNames);
            }
            else if (extension.Equals(FileExtensionOctgn, StringComparison.InvariantCultureIgnoreCase))
            {
                converterDeck = ConvertEngine.ConvertOctgn(fullPathName, deckSectionNames);
            }
            else
            {
                //// The file is in another format, probably '.txt'.  

                // Do sideboard cards appear to be specified on each line?  If so, process it that way
                if (lines.Any(l => RegexMatch_RegularMTGSideBoardCard(l) != null || RegexMatch_MWSSideBoardCard(l) != null))
                {
                    converterDeck = ConvertEngine.ConvertMTGDeckWithSideBoardCardsListedEachLine(lines, deckSectionNames);
                }
                else
                {
                    //// Look for the 'Sideboard' section

                    List<string> mainDeckLines = new List<string>();
                    List<string> sideboardLines = new List<string>();

                    bool sideboardStringPassed = false;
                    foreach (string line in lines)
                    {
                        Match match_StartsWithSideboard = ConvertEngine.Regex_StartsWithSideboard.Match(line);
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

                    Dictionary<string, IEnumerable<string>> sectionLines = new Dictionary<string, IEnumerable<string>>();

                    sectionLines.Add("Main", mainDeckLines);
                    sectionLines.Add("Sideboard", sideboardLines);

                    converterDeck = ConvertEngine.ConvertDeckWithSeparateSections(sectionLines, deckSectionNames);
                }
            }

            converterDeck.PopulateConverterMappings(converterSets);

            return converterDeck;
        }

        /// <summary>
        /// Converts a URL for a known website into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertMTGURL(string url, Dictionary<Guid, ConverterSet> converterSets, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck =
                url.ToLower().Contains("ccgdecks.com") ? ConvertEngine.ConvertURL_ccgdecks_com(url, deckSectionNames) :
                url.ToLower().Contains("deckcheck.de") ? ConvertEngine.ConvertURL_deckcheck_de(url, deckSectionNames) :
                url.ToLower().Contains("essentialmagic.com") ? ConvertEngine.ConvertURL_essentialmagic_com(url, deckSectionNames) :
                url.ToLower().Contains("starcitygames.com") ? ConvertEngine.ConvertURL_starcitygames_com(url, deckSectionNames) :
                url.ToLower().Contains("tcgplayer.com") ? ConvertEngine.ConvertURL_tcgplayer_com(url, deckSectionNames) :
                url.ToLower().Contains("tappedout.net") ? ConvertEngine.ConvertURL_tappedout_net(url, deckSectionNames) :
                url.ToLower().Contains("decklists.net") ? ConvertEngine.ConvertURL_decklists_net(url, deckSectionNames) :
                null;

            if (converterDeck == null)
            {
                throw new InvalidOperationException("There was a problem importing the deck from the given url, or the website has not been implemented yet");
            }

            converterDeck.PopulateConverterMappings(converterSets);

            return converterDeck;
        }

        /// <summary>
        /// Converts a URL for a known website into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertLoTRURL(string url, Dictionary<Guid, ConverterSet> converterSets, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck =
                url.ToLower().Contains("cardgamedb.com") ? ConvertEngine.ConvertURL_cardgamedb_com_LoTR(url, deckSectionNames) :
                null;

            if (converterDeck == null)
            {
                throw new InvalidOperationException("There was a problem importing the deck from the given url, or the website has not been implemented yet");
            }

            converterDeck.PopulateConverterMappings(converterSets);

            return converterDeck;
        }

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
                sectionsLines.Add
                (
                    section.Key, 
                    ConvertEngine.SplitLines(section.Value)
                );
            }

            ConverterDeck converterDeck = ConvertEngine.ConvertDeckWithSeparateSections(sectionsLines, deckSectionNames);
            converterDeck.PopulateConverterMappings(converterSets);
            return converterDeck;
        }

        #region Converters

        /// <summary>
        /// Reads the Cockatrice format file, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Cockatrice Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        private static ConverterDeck ConvertCockatrice(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            XmlTextReader reader = new XmlTextReader(fullPathName);
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

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }

        /// <summary>
        /// Reads the OCTGN format file, and returns a ConverterDeck which is populated with all cards.
        /// </summary>
        /// <param name="fullPathName">The full path name of the OCTGN Deck file to convert</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        /// <remarks>This will purposely ignore the Guids, which may help importing from OCTGN2 or other unofficial sets</remarks>
        private static ConverterDeck ConvertOctgn(string fullPathName, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            XmlTextReader reader = new XmlTextReader(fullPathName);
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

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }

        /// <summary>
        /// Reads the file which has Sideboard cards denoted on each line, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="lines">An array of all the lines of text from a Deck file</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        private static ConverterDeck ConvertMTGDeckWithSideBoardCardsListedEachLine(IEnumerable<string> lines, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            ConverterSection mtgMainDeckConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Main", StringComparison.InvariantCultureIgnoreCase));
            ConverterSection mtgSideboardConverterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase));

            foreach (string line in lines)
            {
                // The line is the Deck Name?  Record it.
                string potentialDeckName = ConvertEngine.RegexMatch_DeckName(line);

                if (potentialDeckName != null)
                {
                    converterDeck.DeckName = potentialDeckName;
                }
                else
                {
                    // The line is not a Comment?
                    if (ConvertEngine.RegexMatch_Comment(line) == null)
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
                        else if (RegexMatch_RegularCard(line) != null)
                        {
                            // The line is a regular main deck "Quantity Card" entry, without any Set info
                            mtgMainDeckConverterSection.AddConverterMapping(RegexMatch_RegularCard(line));
                        }
                        else if (RegexMatch_RegularCardQuantityAfterName(line) != null)
                        {
                            // The line is a regular main deck "Card Quantity" entry, without any Set info
                            mtgMainDeckConverterSection.AddConverterMapping(RegexMatch_RegularCardQuantityAfterName(line));
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
        /// Reads each line in each section, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="sectionLines">A collection of deck sections with Item1 as Name, and Item2 as a collection of lines of text which are cards</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        private static ConverterDeck ConvertDeckWithSeparateSections(Dictionary<string, IEnumerable<string>> sectionLines, IEnumerable<string> deckSectionNames)
        {
            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            foreach (KeyValuePair<string, IEnumerable<string>> section in sectionLines)
            {
                ConverterSection converterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals(section.Key, StringComparison.InvariantCultureIgnoreCase));
                foreach (string line in section.Value)
                {
                    // The line is the Deck Name?  Record it.
                    string potentialDeckName = ConvertEngine.RegexMatch_DeckName(line);

                    if (potentialDeckName != null)
                    {
                        converterDeck.DeckName = potentialDeckName;
                    }
                    else
                    {
                        // The line is not a Comment?
                        if (ConvertEngine.RegexMatch_Comment(line) == null)
                        {
                            // The line is a regular main deck Card/Quantity entry, without any Set info?  Record it
                            ConverterMapping potentialRegularCard = RegexMatch_RegularCard(line);
                            ConverterMapping potentialRegularCardQuantityAfterName = RegexMatch_RegularCardQuantityAfterName(line);
                            if (potentialRegularCard != null)
                            {
                                converterSection.AddConverterMapping(potentialRegularCard);
                            }
                            else if (potentialRegularCardQuantityAfterName != null)
                            {
                                converterSection.AddConverterMapping(potentialRegularCardQuantityAfterName);
                            }
                            else
                            {
                                // The line is a MWS main deck Card/Quantity entry with Set info?  Record it
                                ConverterMapping potentialMWSMainDeckCard = RegexMatch_MWSMainDeckCard(line);
                                if (potentialMWSMainDeckCard != null)
                                {
                                    converterSection.AddConverterMapping(potentialMWSMainDeckCard);
                                }
                                else
                                {
                                    // The line is not a valid card entry
                                }
                            }
                        }
                    }
                }
            }

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }

        /// <summary>
        /// Converts a URL which needs to be altered to get the download link, then uses it to return a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="inputURL">The originally requested URL of the Deck</param>
        /// <param name="deckIdKey">The key for the URL Parameter which contains the ID value for the deck</param>
        /// <param name="urlPrepend">The URL part which precedes the deckIdKey</param>
        /// <param name="urlPostpend">The URL part which follows the deckIdKey</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURLUsingDeckIDInURL(string inputURL, string deckIdKey, string urlPrepend, string urlPostpend, IEnumerable<string> deckSectionNames)
        {
            int deckID;
            string deckIDString;

            // This requires .Net Full Profile, so don't use it
            ////Uri uri = new Uri(inputURL);
            ////if (int.TryParse(HttpUtility.ParseQueryString(uri.Query).Get(deckIdKey), out deckID))

            // Try to get the value for the parameter with the name in deckIdKey
            if (ConvertEngine.GetParams(inputURL).TryGetValue(deckIdKey, out deckIDString))
            {
                // Try to convert the found value into an integer
                if (int.TryParse(deckIDString, out deckID))
                {
                    // Construct a URL which is the download link for the Deck, then convert it
                    string downloadableDeckURL = urlPrepend + deckID + urlPostpend;
                    return ConvertEngine.ConvertDownloadURL(downloadableDeckURL, deckSectionNames);
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads a URL which is a download link to a text file or equivalent, then parses it to return a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck which is a download link to a text file or equivalent</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertDownloadURL(string url, IEnumerable<string> deckSectionNames)
        {
            // Get the file and content
            Tuple<string, IEnumerable<string>> nameAndLines = ConvertEngine.ReadURLFileToLines(url);

            // Convert as SideBoardCardsListedEachLine
            ConverterDeck converterDeck = ConvertEngine.ConvertMTGDeckWithSideBoardCardsListedEachLine(nameAndLines.Item2, deckSectionNames);

            // If no Deck Name was given, use the filename
            if (string.IsNullOrWhiteSpace(converterDeck.DeckName))
            {
                converterDeck.DeckName = nameAndLines.Item1;
            }

            return converterDeck;
        }
        
        /// <summary>
        /// Converts a LoTR URL from cardgamedb.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_cardgamedb_com_LoTR(string url, IEnumerable<string> deckSectionNames)
        {
            object htmlWebInstance = HtmlAgilityPackWrapper.HtmlWeb_CreateInstance();
            object htmlDocumentInstance = HtmlAgilityPackWrapper.HtmlWeb_InvokeMethod_Load(htmlWebInstance, url);
            object htmlDocument_DocumentNode = HtmlAgilityPackWrapper.HtmlDocument_GetProperty_DocumentNode(htmlDocumentInstance);

            // Find the block of javascript that contains the raw deck JSON
            object rawDeckJavascriptNode = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectSingleNode(htmlDocument_DocumentNode, "//script[contains(text(), 'var rawdeck =')]");
            string rawDeckJavascriptText = HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(rawDeckJavascriptNode);

            // The string is javascript, and the variable 'rawdeck' is assigned a big string which is the JSON data
            string[] rawDeckJavascriptLines = rawDeckJavascriptText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string rawDeckLine = rawDeckJavascriptLines.First(l => l.Contains("var rawdeck ="));

            // Trim everything except the JSON
            int openingCurlyBraceIndex = rawDeckLine.IndexOf('{');
            int closingCurlyBraceIndex = rawDeckLine.LastIndexOf('}');
            string rawDeckJSONString = rawDeckLine.Substring(openingCurlyBraceIndex, closingCurlyBraceIndex - openingCurlyBraceIndex + 1);

            dynamic rawDeckJSON = JsonConvert.DeserializeObject(rawDeckJSONString);

            ConverterDeck converterDeck = new ConverterDeck(deckSectionNames);

            foreach (dynamic card in rawDeckJSON.hero)
            {
                ConverterSection converterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals("hero", StringComparison.InvariantCultureIgnoreCase));
                ConverterMapping converterMapping = new ConverterMapping(
                    (string)card.name,
                    (string)card.setname,
                    int.Parse((string)card.quantity));
                converterSection.AddConverterMapping(converterMapping);
            }

            foreach (dynamic card in rawDeckJSON.cards)
            {
                ConverterSection converterSection = converterDeck.ConverterSections.First(cs => cs.SectionName.Equals((string)card.type, StringComparison.InvariantCultureIgnoreCase));
                ConverterMapping converterMapping = new ConverterMapping(
                    (string)card.name,
                    (string)card.setname,
                    int.Parse((string)card.quantity));
                converterSection.AddConverterMapping(converterMapping);
            }

            return converterDeck;
        }

        /// <summary>
        /// Converts a URL from ccgdecks.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_ccgdecks_com(string url, IEnumerable<string> deckSectionNames)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "id", @"http://ccgdecks.com/to_mws.php?id=", string.Empty, deckSectionNames);
        }

        /// <summary>
        /// Converts a URL from deckcheck.de into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_deckcheck_de(string url, IEnumerable<string> deckSectionNames)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "id", @"http://www.deckcheck.de/export.php?id=", @"&format=mws", deckSectionNames);
        }

        /// <summary>
        /// Converts a URL from essentialmagic.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_essentialmagic_com(string url, IEnumerable<string> deckSectionNames)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "ID", @"http://www.essentialmagic.com/Decks/ExportToApprentice.asp?ID=", string.Empty, deckSectionNames);
        }

        /// <summary>
        /// Converts a URL from starcitygames.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_starcitygames_com(string url, IEnumerable<string> deckSectionNames)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "DeckID", @"http://sales.starcitygames.com//deckdatabase/download_deck.php?DeckID=", @"&Mode=app", deckSectionNames);
        }

        /// <summary>
        /// Converts a URL from magic.tcgplayer.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_tcgplayer_com(string url, IEnumerable<string> deckSectionNames)
        {
            // Note, it doesn't get the Deck Name properly yet
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "deck_id", @"http://magic.tcgplayer.com/db/Deck_MWS.asp?ID=", string.Empty, deckSectionNames);
        }

        /// <summary>
        /// Regex pattern for extracting the Deck ID from a tappedout.net URL
        /// </summary>
        private static Regex Regex_tappedout_net = new Regex(@"^.*tappedout\.net/mtg-decks/(.*)/.*", RegexOptions.IgnoreCase);

        /// <summary>
        /// Converts a URL from tappedout.net into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_tappedout_net(string url, IEnumerable<string> deckSectionNames)
        {
            Match m = ConvertEngine.Regex_tappedout_net.Match(url);
            if (m.Success)
            {
                string deckName = m.Groups[1].Value;
                return ConvertEngine.ConvertDownloadURL(@"http://tappedout.net/mtg-decks/" + deckName + @"/?fmt=txt", deckSectionNames);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Converts a URL from decklists.net into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="deckSectionNames">List of the name of each section for the deck being converted.</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_decklists_net(string url, IEnumerable<string> deckSectionNames)
        {
            // Note, it doesn't get the Deck Name properly yet
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "deckId", @"http://www.decklists.net/index.php?option=com_ohmydeckdb&view=deck&controller=deck&deckId=", @"&layout=download&tmpl=component&frm=mws", deckSectionNames);
        }

        #region Regex Helpers

        /// <summary>
        /// Regex pattern for determining if the text is a Comment, and extracting the Comment 
        /// (A comment line begins with '//')
        /// </summary>
        private static Regex Regex_Comment = new Regex(@"^\s*[/][/](.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is the Deck-Name, and extracting the Deck-Name
        /// (A deck name line begins with '// NAME:')
        /// </summary>
        private static Regex Regex_DeckName = new Regex(@"^\s*[/][/]\s*[nN][aA][mM][eE][:]\s*(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a regular Card, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularCard = new Regex(@"^\s*(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a regular Card, but with the quantity after the name, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularCardQuantityAfterName = new Regex(@"^\s*([a-zA-Z].+)\s+[xX]*\s*(\d+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a regular MTG Sideboard-Card, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularMTGSideboardCard = new Regex(@"^\s*[sS][bB][:]\s+(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a Magic Workstation Main-Deck-Card, and extracting the Name, Set, and Quantity
        /// </summary>
        private static Regex Regex_MWSMainDeckCard = new Regex(@"^\s*(\d+)\s+\[(.*?)\]\s+(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a Magic Workstation Sideboard-Card, and extracting the Name, Set, and Quantity
        /// </summary>
        private static Regex Regex_MWSSideBoardCard = new Regex(@"^\s*[sS][bB][:]\s+(\d+)\s+\[(.*?)\]\s+(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text begins with Sideboard.  This signifies that subsequent cards all reside in the Sideboard.
        /// </summary>
        private static Regex Regex_StartsWithSideboard = new Regex(@"^\s*[sS][iI][dD][eE][bB][oO][aA][rR][dD]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text contains parenthesis at the end, and extracting the Card-Name and Inside-Parenthesis-Text.
        /// This is used on card Names once the Quantity and Set have already been removed.
        /// </summary>
        private static Regex Regex_RemoveNameAppendedParenthesis = new Regex(@"\s*(.+)\s*\((\(/\)\(/\))\)|\s*(.+)\s*\((.*)\)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_RegularCard(string line)
        {
            Match m = ConvertEngine.Regex_RegularCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[2].Value),
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
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Card, but with the quantity after the name, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_RegularCardQuantityAfterName(string line)
        {
            // Format: (Name #x)
            Match m = ConvertEngine.Regex_RegularCardQuantityAfterName.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[1].Value),
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
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular MTG Sideboard Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_RegularMTGSideBoardCard(string line)
        {
            Match m = ConvertEngine.Regex_RegularMTGSideboardCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[2].Value),
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
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a MWS Main Deck Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_MWSMainDeckCard(string line)
        {
            Match m = ConvertEngine.Regex_MWSMainDeckCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[3].Value),
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
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a MWS Sideboard Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_MWSSideBoardCard(string line)
        {
            Match m = ConvertEngine.Regex_MWSSideBoardCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameAppendendParenthesis(m.Groups[3].Value),
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
        /// Returns the Deck Name if line is properly formatted as a Deck Name comment, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>The Deck Name</returns>
        private static string RegexMatch_DeckName(string line)
        {
            Match m = ConvertEngine.Regex_DeckName.Match(line);
            return m.Success ?
                m.Groups[1].Value :
                null;
        }

        /// <summary>
        /// Returns the Comment if line is properly formatted as a Comment, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>The Comment</returns>
        private static string RegexMatch_Comment(string line)
        {
            Match m = ConvertEngine.Regex_Comment.Match(line);
            return m.Success ?
                m.Groups[1].Value :
                null;
        }

        /// <summary>
        /// Returns the card name with extra trailing garbage removed
        /// </summary>
        /// <param name="name">The name which may contain extra text in parenthesis</param>
        /// <returns>The Card Name with extra text in parenthesis removed</returns>
        private static string RegexMatch_RemoveNameAppendendParenthesis(string name)
        {
            Match m = ConvertEngine.Regex_RemoveNameAppendedParenthesis.Match(name);
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

        #endregion Regex Helpers

        /// <summary>
        /// Reads the file at the URL location, and returns both the filename and an array of all lines of text found within
        /// </summary>
        /// <param name="url">Location to read into a filename and an array of lines</param>
        /// <returns>Returns 1: filename, 2: all text as array of lines</returns>
        private static Tuple<string, IEnumerable<string>> ReadURLFileToLines(string url)
        {
            List<string> lines = new List<string>();
            string filename;

            System.Net.WebRequest wreq = System.Net.WebRequest.Create(url);

            using (System.Net.WebResponse wresp = wreq.GetResponse())
            {
                filename = wresp.Headers["Content-Disposition"];  // "attachment; filename=\"Draft #1396152 deck.mwDeck\""
                using (System.IO.Stream s = wresp.GetResponseStream())
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(s))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }

            return new Tuple<string, IEnumerable<string>>(filename, lines);
        }

        #endregion Converters

        #region Helpers

        /// <summary>
        /// Returns the input text split into an array, split on newlines.  Empty lines are not included.
        /// </summary>
        /// <param name="text">Text to be split on each new line</param>
        /// <returns>text split into an array, split on newlines.  Empty lines are not included.</returns>
        /// <seealso cref="http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net"/>
        private static string[] SplitLines(string text)
        {
            return text == null ?
                new string[] { } :
                text.Split
                (
                    new string[] { "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                );
        }

        /// <summary>
        /// Returns the Parameters found in the given URL as a Dictionary of Key-Value strings
        /// </summary>
        /// <param name="uri">The URI string to parse for Parameters</param>
        /// <returns>Dictionary of Key-Value strings of the parameters</returns>
        /// <see cref="http://codereview.stackexchange.com/a/1592"/>
        private static Dictionary<string, string> GetParams(string uri)
        {
            var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            return matches.Cast<Match>().ToDictionary
            (
                m => Uri.UnescapeDataString(m.Groups[2].Value),
                m => Uri.UnescapeDataString(m.Groups[3].Value)
            );
        }

        #endregion Helpers
    }
}
