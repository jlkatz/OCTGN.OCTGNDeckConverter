// -----------------------------------------------------------------------
// <copyright file="ConvertEngine.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MTGDeckConverter.Model
{
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
        /// File extension for Apprentice Deck files
        /// </summary>
        public const string FileExtensionApprentice = ".dec";

        /// <summary>
        /// File extension for Magic Workstation Deck files
        /// </summary>
        public const string FileExtensionMWS = ".mwDeck";

        /// <summary>
        /// File extension for Cockatrice Deck files
        /// </summary>
        public const string FileExtensionCockatrice = ".cod";

        /// <summary>
        /// File extension for OCTGN Deck files
        /// </summary>
        public const string FileExtensionOctgn = ".o8d";

        /// <summary>
        /// Converts a text file into a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertFile(string fullPathName, Dictionary<Guid, ConverterSet> converterSets)
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
                converterDeck = ConvertEngine.ConvertDeckWithSideBoardCardsListedEachLine(lines);
            }
            else if (extension.Equals(FileExtensionCockatrice, StringComparison.InvariantCultureIgnoreCase))
            {
                converterDeck = ConvertEngine.ConvertCockatrice(fullPathName);
            }
            else if (extension.Equals(FileExtensionOctgn, StringComparison.InvariantCultureIgnoreCase))
            {
                converterDeck = ConvertEngine.ConvertOctgn(fullPathName);
            }
            else
            {
                //// The file is in another format, probably '.txt'.  

                // Do sideboard cards appear to be specified on each line?  If so, process it that way
                if (lines.Any(l => RegexMatch_RegularSideBoardCard(l) != null || RegexMatch_MWSSideBoardCard(l) != null))
                {
                    converterDeck = ConvertEngine.ConvertDeckWithSideBoardCardsListedEachLine(lines);
                }
                else
                {
                    //// Look for the 'Sideboard' section

                    List<string> mainDeckLines = new List<string>();
                    List<string> sideBoardLines = new List<string>();

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
                                sideBoardLines.Add(line);
                            }
                        }
                    }

                    converterDeck = ConvertEngine.ConvertDeckWithSeparateSections(mainDeckLines, sideBoardLines);
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
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertURL(string url, Dictionary<Guid, ConverterSet> converterSets)
        {
            ConverterDeck converterDeck =
                url.ToLower().Contains("ccgdecks.com") ? ConvertEngine.ConvertURL_ccgdecks_com(url) :
                url.ToLower().Contains("deckcheck.de") ? ConvertEngine.ConvertURL_deckcheck_de(url) :
                url.ToLower().Contains("essentialmagic.com") ? ConvertEngine.ConvertURL_essentialmagic_com(url) :
                url.ToLower().Contains("starcitygames.com") ? ConvertEngine.ConvertURL_starcitygames_com(url) :
                url.ToLower().Contains("tcgplayer.com") ? ConvertEngine.ConvertURL_tcgplayer_com(url) :
                url.ToLower().Contains("tappedout.net") ? ConvertEngine.ConvertURL_tappedout_net(url) :
                url.ToLower().Contains("decklists.net") ? ConvertEngine.ConvertURL_decklists_net(url) :
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
        /// <param name="mainDeckText">The user input text of all cards in the Main Deck</param>
        /// <param name="sideBoardText">The user input text of all cards in the Sideboard</param>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        /// <returns>Returns a ConverterDeck which has all ConverterMappings populated with potential cards from the converterSets</returns>
        public static ConverterDeck ConvertText(string mainDeckText, string sideBoardText, Dictionary<Guid, ConverterSet> converterSets)
        {
            string[] mainDeckLines = ConvertEngine.SplitLines(mainDeckText);
            string[] sideBoardLines = ConvertEngine.SplitLines(sideBoardText);

            ConverterDeck converterDeck = ConvertEngine.ConvertDeckWithSeparateSections(mainDeckLines, sideBoardLines);

            converterDeck.PopulateConverterMappings(converterSets);

            return converterDeck;
        }

        #region Converters

        /// <summary>
        /// Reads the Cockatrice format file, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="fullPathName">The full path name of the Cockatrice Deck file to convert</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        private static ConverterDeck ConvertCockatrice(string fullPathName)
        {
            ConverterDeck converterDeck = new ConverterDeck();

            XmlTextReader reader = new XmlTextReader(fullPathName);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            XmlDocument xd = new XmlDocument();
            xd.Load(reader);

            XmlNode xnodDE = xd.DocumentElement;

            if (xnodDE.Name != "cockatrice_deck")
            {
                throw new InvalidOperationException("File is not a Cockatrice Deck");
            }

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
                            converterDeck.AddMainDeckConverterMapping
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
                            converterDeck.AddSideBoardConverterMapping
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
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        /// <remarks>This will purposely ignore the Guids, which may help importing from OCTGN2 or other unofficial sets</remarks>
        private static ConverterDeck ConvertOctgn(string fullPathName)
        {
            ConverterDeck converterDeck = new ConverterDeck();

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
                    if (child.Attributes.GetNamedItem("name").Value.Equals("Main", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (XmlNode cardXmlNode in child.ChildNodes)
                        {
                            converterDeck.AddMainDeckConverterMapping
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
                    else if (child.Attributes.GetNamedItem("name").Value.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (XmlNode cardXmlNode in child.ChildNodes)
                        {
                            converterDeck.AddSideBoardConverterMapping
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

        /// <summary>
        /// Reads the file which has Sideboard cards denoted on each line, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="lines">An array of all the lines of text from a Deck file</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        private static ConverterDeck ConvertDeckWithSideBoardCardsListedEachLine(IEnumerable<string> lines)
        {
            ConverterDeck converterDeck = new ConverterDeck();

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
                        // The line is a regular main deck Card/Quantity entry, without any Set info?  Record it
                        ConverterMapping potentialRegularMainDeckCard = RegexMatch_RegularMainDeckCard(line);
                        if (potentialRegularMainDeckCard != null)
                        {
                            converterDeck.AddMainDeckConverterMapping(potentialRegularMainDeckCard);
                        }
                        else
                        {
                            // The line is a regular sideboard Card/Quantity entry, without any Set info?  Record it
                            ConverterMapping potentialRegularSideBoardCard = RegexMatch_RegularSideBoardCard(line);
                            if (potentialRegularSideBoardCard != null)
                            {
                                converterDeck.AddSideBoardConverterMapping(potentialRegularSideBoardCard);
                            }
                            else
                            {
                                // The line is a MWS main deck Card/Quantity entry with Set info?  Record it
                                ConverterMapping potentialMWSMainDeckCard = RegexMatch_MWSMainDeckCard(line);
                                if (potentialMWSMainDeckCard != null)
                                {
                                    converterDeck.AddMainDeckConverterMapping(potentialMWSMainDeckCard);
                                }
                                else
                                {
                                    // The line is a MWS sideboard Card/Quantity entry, without any Set info?  Record it
                                    ConverterMapping potentialMWSSideBoardCard = RegexMatch_MWSSideBoardCard(line);
                                    if (potentialMWSSideBoardCard != null)
                                    {
                                        converterDeck.AddSideBoardConverterMapping(potentialMWSSideBoardCard);
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
            }

            converterDeck.ConversionSuccessful = true;
            return converterDeck;
        }

        /// <summary>
        /// Reads the file which has Sideboard cards in a section below the MainDeck cards, and returns a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="mainDecklines">An array of all the lines of text which are MainDeck cards</param>
        /// <param name="sideBoardLines">An array of all the lines of text which are SideBoard cards</param>
        /// <returns>A ConverterDeck object populated with all cards and deck name</returns>
        private static ConverterDeck ConvertDeckWithSeparateSections(IEnumerable<string> mainDecklines, IEnumerable<string> sideBoardLines)
        {
            ConverterDeck converterDeck = new ConverterDeck();
            List<ConverterMapping> mainDeckMappings = new List<ConverterMapping>();
            List<ConverterMapping> sideBoardMappings = new List<ConverterMapping>();

            List<Tuple<IEnumerable<string>, List<ConverterMapping>>> sectionsMap = new List<Tuple<IEnumerable<string>, List<ConverterMapping>>>();
            sectionsMap.Add(new Tuple<IEnumerable<string>, List<ConverterMapping>>(mainDecklines, mainDeckMappings));
            sectionsMap.Add(new Tuple<IEnumerable<string>, List<ConverterMapping>>(sideBoardLines, sideBoardMappings));

            foreach (var section in sectionsMap)
            {
                foreach (string line in section.Item1)
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
                            ConverterMapping potentialRegularMainDeckCard = RegexMatch_RegularMainDeckCard(line);
                            if (potentialRegularMainDeckCard != null)
                            {
                                section.Item2.Add(potentialRegularMainDeckCard);
                            }
                            else
                            {
                                // The line is a MWS main deck Card/Quantity entry with Set info?  Record it
                                ConverterMapping potentialMWSMainDeckCard = RegexMatch_MWSMainDeckCard(line);
                                if (potentialMWSMainDeckCard != null)
                                {
                                    section.Item2.Add(potentialMWSMainDeckCard);
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

            foreach (var mainDeckMapping in mainDeckMappings)
            {
                converterDeck.AddMainDeckConverterMapping(mainDeckMapping);
            }

            foreach (var sideBoardMapping in sideBoardMappings)
            {
                converterDeck.AddSideBoardConverterMapping(sideBoardMapping);
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
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURLUsingDeckIDInURL(string inputURL, string deckIdKey, string urlPrepend, string urlPostpend)
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
                    return ConvertEngine.ConvertDownloadURL(downloadableDeckURL);
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads a URL which is a download link to a text file or equivalent, then parses it to return a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck which is a download link to a text file or equivalent</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertDownloadURL(string url)
        {
            // Get the file and content
            Tuple<string, IEnumerable<string>> nameAndLines = ConvertEngine.ReadURLFileToLines(url);

            // Convert as SideBoardCardsListedEachLine
            ConverterDeck converterDeck = ConvertEngine.ConvertDeckWithSideBoardCardsListedEachLine(nameAndLines.Item2);

            // If no Deck Name was given, use the filename
            if (string.IsNullOrWhiteSpace(converterDeck.DeckName))
            {
                converterDeck.DeckName = nameAndLines.Item1;
            }

            return converterDeck;
        }

        /// <summary>
        /// Converts a URL from ccgdecks.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_ccgdecks_com(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "id", @"http://ccgdecks.com/to_mws.php?id=", string.Empty);
        }

        /// <summary>
        /// Converts a URL from deckcheck.de into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_deckcheck_de(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "id", @"http://www.deckcheck.de/export.php?id=", @"&format=mws");
        }

        /// <summary>
        /// Converts a URL from essentialmagic.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_essentialmagic_com(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "ID", @"http://www.essentialmagic.com/Decks/ExportToApprentice.asp?ID=", string.Empty);
        }

        /// <summary>
        /// Converts a URL from starcitygames.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_starcitygames_com(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "DeckID", @"http://sales.starcitygames.com//deckdatabase/download_deck.php?DeckID=", @"&Mode=app");
        }

        /// <summary>
        /// Converts a URL from magic.tcgplayer.com into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_tcgplayer_com(string url)
        {
            // Note, it doesn't get the Deck Name properly yet
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "deck_id", @"http://magic.tcgplayer.com/db/Deck_MWS.asp?ID=", string.Empty);
        }

        /// <summary>
        /// Regex pattern for extracting the Deck ID from a tappedout.net URL
        /// </summary>
        private static Regex Regex_tappedout_net = new Regex(@"^.*tappedout\.net/mtg-decks/(.*)/.*", RegexOptions.IgnoreCase);

        /// <summary>
        /// Converts a URL from tappedout.net into a ConverterDeck which is populated with all cards and deck name.
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_tappedout_net(string url)
        {
            Match m = ConvertEngine.Regex_tappedout_net.Match(url);
            if (m.Success)
            {
                string deckName = m.Groups[1].Value;
                return ConvertEngine.ConvertDownloadURL(@"http://tappedout.net/mtg-decks/" + deckName + @"/?format=txt");
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
        /// <returns>A ConverterDeck which is populated with all cards and deck name</returns>
        private static ConverterDeck ConvertURL_decklists_net(string url)
        {
            // Note, it doesn't get the Deck Name properly yet
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "deckId", @"http://www.decklists.net/index.php?option=com_ohmydeckdb&view=deck&controller=deck&deckId=", @"&layout=download&tmpl=component&frm=mws");
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
        /// Regex pattern for determining if the text is a regular Main-Deck-Card, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularMainDeckCard = new Regex(@"^\s*(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a regular Sideboard-Card, and extracting the Name and Quantity
        /// </summary>
        private static Regex Regex_RegularSideBoardCard = new Regex(@"^\s*[sS][bB][:]\s+(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a Magic Workstation Main-Deck-Card, and extracting the Name, Set, and Quantity
        /// </summary>
        private static Regex Regex_MWSMainDeckCard = new Regex(@"^\s*(\d)+\s+\[(.*?)\]\s+(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text is a Magic Workstation Sideboard-Card, and extracting the Name, Set, and Quantity
        /// </summary>
        private static Regex Regex_MWSSideBoardCard = new Regex(@"^\s*[sS][bB][:]\s+(\d)+\s+\[(.*?)\]\s+(.+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text begins with Sideboard.  This signifies that subsequent cards all reside in the Sideboard.
        /// </summary>
        private static Regex Regex_StartsWithSideboard = new Regex(@"^\s*[sS][iI][dD][eE][bB][oO][aA][rR][dD]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern for determining if the text contains extra garbage characters, and extracting the Card-Name and Extra-Garbage.
        /// This is used on card Names once the Quantity and Set have already been removed.
        /// </summary>
        private static Regex Regex_RemoveNameExtraGarbage = new Regex(@"\s*(.+)\s\((|\d+|Cost:|\(/\)\(/\))\)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Main Deck Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_RegularMainDeckCard(string line)
        {
            Match m = ConvertEngine.Regex_RegularMainDeckCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameExtraGarbage(m.Groups[2].Value),
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
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Sideboard Card, null otherwise
        /// </summary>
        /// <param name="line">Line of text to parse</param>
        /// <returns>ConverterMapping with info from the line if properly formatted</returns>
        private static ConverterMapping RegexMatch_RegularSideBoardCard(string line)
        {
            Match m = ConvertEngine.Regex_RegularSideBoardCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameExtraGarbage(m.Groups[2].Value),
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
                    ConvertEngine.RegexMatch_RemoveNameExtraGarbage(m.Groups[3].Value),
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
                    ConvertEngine.RegexMatch_RemoveNameExtraGarbage(m.Groups[3].Value),
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
        /// <param name="name">The name which may contain extra trailing garbage</param>
        /// <returns>The Card Name with extra trailing garbage removed</returns>
        private static string RegexMatch_RemoveNameExtraGarbage(string name)
        {
            Match m = ConvertEngine.Regex_RemoveNameExtraGarbage.Match(name);
            return m.Success ?
                m.Groups[1].Value :
                name;
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
