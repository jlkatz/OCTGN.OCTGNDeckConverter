using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web;

namespace MTGDeckConverter.Model
{
    public static class ConvertEngine
    {
        public const string FileExtensionApprentice = ".dec";
        public const string FileExtensionMWS = ".mwDeck";
        public const string FileExtensionCockatrice = ".cod";
        public const string FileExtensionOctgn = ".o8d";

        /// <summary>
        /// Returns 1: Success? 2. MainDeck 3. SideBoard 4. Deck Name (blank if unknown) 5. Message about the conversion (errors)
        /// </summary>
        public static Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string> ConvertFile(string fullPathName, Dictionary<Guid, ConverterSet> converterSets)
        {
            string extension = System.IO.Path.GetExtension(fullPathName);
            string text = System.IO.File.ReadAllText(fullPathName);
            string[] lines = ConvertEngine.SplitLines(text);
            Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> sections;

            if 
            (
                extension.Equals(FileExtensionMWS, StringComparison.InvariantCultureIgnoreCase) ||
                extension.Equals(FileExtensionApprentice, StringComparison.InvariantCultureIgnoreCase)
            )
            { sections = ConvertDeckWithSideBoardCardsListedEachLine(lines); }

            else if (extension.Equals(FileExtensionCockatrice, StringComparison.InvariantCultureIgnoreCase))
            { sections = ConvertCockatrice(fullPathName); }

            else if (extension.Equals(FileExtensionOctgn, StringComparison.InvariantCultureIgnoreCase))
            { sections = ConvertOctgn(fullPathName); }

            //The file is in another format, probably '.txt'.  
            else
            {
                //Sideboard cards appear to be specified on each line?  Process it that way
                if (lines.Any(l => RegexMatch_RegularSideBoardCard(l) != null || RegexMatch_MWSSideBoardCard(l) != null))
                {
                    sections = ConvertDeckWithSideBoardCardsListedEachLine(lines);
                }

                //Look for the 'Sideboard' section
                else
                {
                    List<string> mainDeckLines = new List<string>();
                    List<string> sideBoardLines = new List<string>();
                    Regex regex_StartsWithSideboard = new Regex(ConvertEngine.RegexPattern_StartsWithSideboard, RegexOptions.IgnoreCase);

                    bool sideboardStringPassed = false;
                    foreach (string line in lines)
                    {
                        Match match_StartsWithSideboard = regex_StartsWithSideboard.Match(line);
                        if (match_StartsWithSideboard.Success)
                        { sideboardStringPassed = true; }
                        else
                        {
                            if (!sideboardStringPassed)
                            { mainDeckLines.Add(line); }
                            else
                            { sideBoardLines.Add(line); }
                        }
                    }

                    sections = ConvertEngine.ConvertDeckWithSeparateSections(mainDeckLines, sideBoardLines);
                }
            }

            foreach (ConverterMapping cm in sections.Item1)
            { ConvertEngine.PopulateMappingWithPotentialCards(cm, converterSets); }
            foreach (ConverterMapping cm in sections.Item2)
            { ConvertEngine.PopulateMappingWithPotentialCards(cm, converterSets); }

            return new Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string>(true, sections.Item1, sections.Item2, sections.Item3, "");
        }

        /// <summary>
        /// Returns 1: Success? 2. MainDeck 3. SideBoard 4. Deck Name (blank if unknown) 5. Message about the conversion (errors)
        /// </summary>
        public static Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string> ConvertURL(string url, Dictionary<Guid, ConverterSet> converterSets)
        {
            Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> sections;
            if (url.ToLower().Contains("ccgdecks.com"))
            { sections = ConvertEngine.ConvertURL_ccgdecks_com(url); }
                
            else if (url.ToLower().Contains("deckcheck.de"))
            { sections = ConvertEngine.ConvertURL_deckcheck_de(url); }
                
            else if (url.ToLower().Contains("essentialmagic.com"))
            { sections = ConvertEngine.ConvertURL_essentialmagic_com(url); }
                
            else if (url.ToLower().Contains("starcitygames.com"))
            { sections = ConvertEngine.ConvertURL_starcitygames_com(url); }
                
            else if (url.ToLower().Contains("tcgplayer.com"))
            { sections = ConvertEngine.ConvertURL_tcgplayer_com(url); }
                
            else if (url.ToLower().Contains("tappedout.net"))
            { sections = ConvertEngine.ConvertURL_tappedout_net(url); }

            else if (url.ToLower().Contains("decklists.net"))
            { sections = ConvertEngine.ConvertURL_decklists_net(url); }
                
            else
            { throw new NotImplementedException("The website has not been implemented yet"); }

            if (sections == null)
            { throw new InvalidOperationException("There was a problem importing the deck from the given url"); }

            foreach (ConverterMapping cm in sections.Item1)
            { ConvertEngine.PopulateMappingWithPotentialCards(cm, converterSets); }
            foreach (ConverterMapping cm in sections.Item2)
            { ConvertEngine.PopulateMappingWithPotentialCards(cm, converterSets); }

            return new Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string>(true, sections.Item1, sections.Item2, sections.Item3, "");
        }

        /// <summary>
        /// Returns 1: Success? 2. MainDeck 3. SideBoard 4. Deck Name (blank if unknown) 5. Message about the conversion (errors)
        /// </summary>
        public static Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string> ConvertText(string mainDeckText, string sideBoardText, Dictionary<Guid, ConverterSet> converterSets)
        {
            Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> sections;
            string[] mainDeckLines = ConvertEngine.SplitLines(mainDeckText);
            string[] sideBoardLines = ConvertEngine.SplitLines(sideBoardText);

            sections = ConvertEngine.ConvertDeckWithSeparateSections(mainDeckLines, sideBoardLines);

            foreach (ConverterMapping cm in sections.Item1)
            { ConvertEngine.PopulateMappingWithPotentialCards(cm, converterSets); }
            foreach (ConverterMapping cm in sections.Item2)
            { ConvertEngine.PopulateMappingWithPotentialCards(cm, converterSets); }

            return new Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string>(true, sections.Item1, sections.Item2, sections.Item3, "");
        }

        #region Converters

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertCockatrice(string fullPathName)
        {
            List<ConverterMapping> mainDeckMappings = new List<ConverterMapping>();
            List<ConverterMapping> sideBoardMappings = new List<ConverterMapping>();
            string deckName = "";

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
                    deckName = child.InnerText;
                }
                else if (child.Name.Equals("zone", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (child.Attributes.GetNamedItem("name").Value.Equals("main", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (XmlNode cardXmlNode in child.ChildNodes)
                        {
                            mainDeckMappings.Add
                            (
                                new ConverterMapping
                                (
                                    cardXmlNode.Attributes.GetNamedItem("name").Value,
                                    "",
                                    int.Parse(cardXmlNode.Attributes.GetNamedItem("number").Value)
                                )
                            );
                        }
                    }

                    else if (child.Attributes.GetNamedItem("name").Value.Equals("side", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (XmlNode cardXmlNode in child.ChildNodes)
                        {
                            sideBoardMappings.Add
                            (
                                new ConverterMapping
                                (
                                    cardXmlNode.Attributes.GetNamedItem("name").Value,
                                    "",
                                    int.Parse(cardXmlNode.Attributes.GetNamedItem("number").Value)
                                )
                            );
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string>(mainDeckMappings, sideBoardMappings, deckName);
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. blank (OCTGN does not store deck names)
        /// This will purposely ignore the Guids, which may help importing from OCTGN2 or other unofficial sets
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertOctgn(string fullPathName)
        {
            List<ConverterMapping> mainDeckMappings = new List<ConverterMapping>();
            List<ConverterMapping> sideBoardMappings = new List<ConverterMapping>();
            string deckName = "";

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
                            mainDeckMappings.Add
                            (
                                new ConverterMapping
                                (
                                    cardXmlNode.InnerText,
                                    "",
                                    int.Parse(cardXmlNode.Attributes.GetNamedItem("qty").Value)
                                )
                            );
                        }
                    }

                    else if (child.Attributes.GetNamedItem("name").Value.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (XmlNode cardXmlNode in child.ChildNodes)
                        {
                            sideBoardMappings.Add
                            (
                                new ConverterMapping
                                (
                                    cardXmlNode.InnerText,
                                    "",
                                    int.Parse(cardXmlNode.Attributes.GetNamedItem("qty").Value)
                                )
                            );
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string>(mainDeckMappings, sideBoardMappings, deckName);
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertDeckWithSideBoardCardsListedEachLine(IEnumerable<string> lines)
        {
            List<ConverterMapping> mainDeckMappings = new List<ConverterMapping>();
            List<ConverterMapping> sideBoardMappings = new List<ConverterMapping>();
            string deckName = "";

            foreach (string line in lines)
            {
                //The line is the Deck Name?  Record it.
                string potentialDeckName = ConvertEngine.RegexMatch_DeckName(line);
                if (potentialDeckName != null)
                { deckName = potentialDeckName; }
                else
                {
                    //The line is not a Comment?
                    if (ConvertEngine.RegexMatch_Comment(line) == null)
                    {
                        //The line is a regular main deck Card/Quantity entry, without any Set info?  Record it
                        ConverterMapping potentialRegularMainDeckCard = RegexMatch_RegularMainDeckCard(line);
                        if (potentialRegularMainDeckCard != null)
                        {
                            mainDeckMappings.Add(potentialRegularMainDeckCard);
                        }
                        else
                        {
                            //The line is a regular sideboard Card/Quantity entry, without any Set info?  Record it
                            ConverterMapping potentialRegularSideBoardCard = RegexMatch_RegularSideBoardCard(line);
                            if (potentialRegularSideBoardCard != null)
                            {
                                sideBoardMappings.Add(potentialRegularSideBoardCard);
                            }
                            else
                            {
                                //The line is a MWS main deck Card/Quantity entry with Set info?  Record it
                                ConverterMapping potentialMWSMainDeckCard = RegexMatch_MWSMainDeckCard(line);
                                if (potentialMWSMainDeckCard != null)
                                {
                                    mainDeckMappings.Add(potentialMWSMainDeckCard);
                                }
                                else
                                {
                                    //The line is a MWS sideboard Card/Quantity entry, without any Set info?  Record it
                                    ConverterMapping potentialMWSSideBoardCard = RegexMatch_MWSSideBoardCard(line);
                                    if (potentialMWSSideBoardCard != null)
                                    {
                                        sideBoardMappings.Add(potentialMWSSideBoardCard);
                                    }
                                    else
                                    {
                                        //The line is not a valid card entry
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string>(mainDeckMappings, sideBoardMappings, deckName);
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertDeckWithSeparateSections(IEnumerable<string> mainDecklines, IEnumerable<string> sideBoardLines)
        {
            List<ConverterMapping> mainDeckMappings = new List<ConverterMapping>();
            List<ConverterMapping> sideBoardMappings = new List<ConverterMapping>();
            string deckName = "";

            Regex regex_Comment = new Regex(ConvertEngine.RegexPattern_Comment, RegexOptions.IgnoreCase);
            Regex regex_DeckName = new Regex(ConvertEngine.RegexPattern_DeckName, RegexOptions.IgnoreCase);
            Regex regex_RegularMainDeckCard = new Regex(ConvertEngine.RegexPattern_RegularMainDeckCard, RegexOptions.IgnoreCase);
            Regex regex_MWSMainDeckCard = new Regex(ConvertEngine.RegexPattern_MWSMainDeckCard, RegexOptions.IgnoreCase);
            Regex regex_RemoveNameExtraGarbage = new Regex(ConvertEngine.RegexPattern_RemoveNameExtraGarbage, RegexOptions.IgnoreCase);

            List<Tuple<IEnumerable<string>, List<ConverterMapping>>> sectionsMap = new List<Tuple<IEnumerable<string>, List<ConverterMapping>>>();
            sectionsMap.Add(new Tuple<IEnumerable<string>, List<ConverterMapping>>(mainDecklines, mainDeckMappings));
            sectionsMap.Add(new Tuple<IEnumerable<string>, List<ConverterMapping>>(sideBoardLines, sideBoardMappings));

            foreach (var section in sectionsMap)
            {
                foreach (string line in section.Item1)
                {
                    //The line is the Deck Name?  Record it.
                    string potentialDeckName = ConvertEngine.RegexMatch_DeckName(line);
                    if (potentialDeckName != null)
                    { deckName = potentialDeckName; }
                    else
                    {
                        //The line is not a Comment?
                        if (ConvertEngine.RegexMatch_Comment(line) == null)
                        {
                            //The line is a regular main deck Card/Quantity entry, without any Set info?  Record it
                            ConverterMapping potentialRegularMainDeckCard = RegexMatch_RegularMainDeckCard(line);
                            if (potentialRegularMainDeckCard != null)
                            {
                                section.Item2.Add(potentialRegularMainDeckCard);
                            }
                            else
                            {
                                //The line is a MWS main deck Card/Quantity entry with Set info?  Record it
                                ConverterMapping potentialMWSMainDeckCard = RegexMatch_MWSMainDeckCard(line);
                                if (potentialMWSMainDeckCard != null)
                                {
                                    section.Item2.Add(potentialMWSMainDeckCard);
                                }
                                else
                                {
                                    //The line is not a valid card entry
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string>(mainDeckMappings, sideBoardMappings, deckName);
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURLUsingDeckIDInURL(string inputURL, string deckIdKey, string urlPrepend, string urlPostpend)
        {  
            int deckID;

            //Requires .Net Full Profile
            //Uri uri = new Uri(inputURL);
            //if (int.TryParse(HttpUtility.ParseQueryString(uri.Query).Get(deckIdKey), out deckID))

            string deckIDString;
            if (ConvertEngine.GetParams(inputURL).TryGetValue(deckIdKey, out deckIDString))
            {
                if (int.TryParse(deckIDString, out deckID))
                {
                    return ConvertEngine.ConvertDownloadURL(urlPrepend + deckID + urlPostpend);
                }
            }
            return null;
        }

        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertDownloadURL(string url)
        {
            //Get the file and content
            Tuple<string, IEnumerable<string>> nameAndLines = ConvertEngine.ReadURLFileToLines(url);

            //Convert as SideBoardCardsListedEachLine
            var result = ConvertEngine.ConvertDeckWithSideBoardCardsListedEachLine(nameAndLines.Item2);

            //If no Deck Name was given, use the filename
            return string.IsNullOrWhiteSpace(result.Item3) ?
                new Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string>(result.Item1, result.Item2, nameAndLines.Item1) :
                result;
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_ccgdecks_com(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "id", @"http://ccgdecks.com/to_mws.php?id=", @"");
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_deckcheck_de(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "id", @"http://www.deckcheck.de/export.php?id=", @"&format=mws");
        }
        
        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_essentialmagic_com(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "ID", @"http://www.essentialmagic.com/Decks/ExportToApprentice.asp?ID=", @"");
        }
        
        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_starcitygames_com(string url)
        {
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "DeckID", @"http://sales.starcitygames.com//deckdatabase/download_deck.php?DeckID=", @"&Mode=app");
        }
        
        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_tcgplayer_com(string url)
        {
            //Note, it doesn't get the Deck Name properly yet
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "deck_id", @"http://magic.tcgplayer.com/db/Deck_MWS.asp?ID=", @"");
        }

        /// <summary>
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private const string RegexPattern_tappedout_net = @"^.*tappedout\.net/mtg-decks/(.*)/.*";
        private static Regex Regex_tappedout_net = new Regex(ConvertEngine.RegexPattern_tappedout_net, RegexOptions.IgnoreCase);
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_tappedout_net(string url)
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
        /// Returns 1. MainDeck 2. SideBoard 3. Deck Name (blank if unknown)
        /// </summary>
        private static Tuple<IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string> ConvertURL_decklists_net(string url)
        {
            //Note, it doesn't get the Deck Name properly yet
            return ConvertEngine.ConvertURLUsingDeckIDInURL(url, "deckId", @"http://www.decklists.net/index.php?option=com_ohmydeckdb&view=deck&controller=deck&deckId=", @"&layout=download&tmpl=component&frm=mws");
        }

        #region Regex Helpers

        private const string RegexPattern_Comment = @"^\s*[/][/](.+)";
        private const string RegexPattern_DeckName = @"^\s*[/][/]\s*[nN][aA][mM][eE][:]\s*(.+)";
        private const string RegexPattern_RegularMainDeckCard = @"^\s*(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)";
        private const string RegexPattern_RegularSideboardCard = @"^\s*[sS][bB][:]\s+(\d+)\s*[xX]?(?!\s+\[)\s+([a-zA-Z].+)";
        private const string RegexPattern_MWSMainDeckCard = @"^\s*(\d)+\s+\[(.*?)\]\s+(.+)";
        private const string RegexPattern_MWSSideboardCard = @"^\s*[sS][bB][:]\s+(\d)+\s+\[(.*?)\]\s+(.+)";
        private const string RegexPattern_StartsWithSideboard = @"^\s*[sS][iI][dD][eE][bB][oO][aA][rR][dD]";
        private const string RegexPattern_RemoveNameExtraGarbage = @"\s*(.+)\s\((|\d+|Cost:|\(/\)\(/\))\)";

        private static Regex Regex_Comment = new Regex(ConvertEngine.RegexPattern_Comment, RegexOptions.IgnoreCase);
        private static Regex Regex_DeckName = new Regex(ConvertEngine.RegexPattern_DeckName, RegexOptions.IgnoreCase);
        private static Regex Regex_RegularMainDeckCard = new Regex(ConvertEngine.RegexPattern_RegularMainDeckCard, RegexOptions.IgnoreCase);
        private static Regex Regex_RegularSideBoardCard = new Regex(ConvertEngine.RegexPattern_RegularSideboardCard, RegexOptions.IgnoreCase);
        private static Regex Regex_MWSMainDeckCard = new Regex(ConvertEngine.RegexPattern_MWSMainDeckCard, RegexOptions.IgnoreCase);
        private static Regex Regex_MWSSideBoardCard = new Regex(ConvertEngine.RegexPattern_MWSSideboardCard, RegexOptions.IgnoreCase);
        private static Regex Regex_RemoveNameExtraGarbage = new Regex(ConvertEngine.RegexPattern_RemoveNameExtraGarbage, RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a properly populated ConverterMapping if the line is properly formatted as a Regular Main Deck Card, null otherwise
        /// </summary>
        private static ConverterMapping RegexMatch_RegularMainDeckCard(string line)
        {
            Match m = ConvertEngine.Regex_RegularMainDeckCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameExtraGarbage(m.Groups[2].Value),
                    "",
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
        private static ConverterMapping RegexMatch_RegularSideBoardCard(string line)
        {
            Match m = ConvertEngine.Regex_RegularSideBoardCard.Match(line);
            if (m.Success)
            {
                return new ConverterMapping
                (
                    ConvertEngine.RegexMatch_RemoveNameExtraGarbage(m.Groups[2].Value),
                    "",
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
        private static string RegexMatch_RemoveNameExtraGarbage(string name)
        {
            var bob = ConvertEngine.Regex_RemoveNameExtraGarbage.Match(name);
            return bob.Success ?
                bob.Groups[1].Value :
                name;
        }

        #endregion Regex Helpers

        /// <summary>
        /// Returns 1: filename, 2: all text as array of lines
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static Tuple<string, IEnumerable<string>> ReadURLFileToLines(string url)
        {
            List<string> lines = new List<string>();
            string filename;

            System.Net.WebRequest wreq = System.Net.WebRequest.Create(url);

            using (System.Net.WebResponse wresp = wreq.GetResponse())
            {
                filename = wresp.Headers["Content-Disposition"];  //"attachment; filename=\"Draft #1396152 deck.mwDeck\""
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

        private static void PopulateMappingWithPotentialCards(ConverterMapping cm, Dictionary<Guid, ConverterSet> converterSets)
        {
            if (cm == null) { throw new ArgumentNullException(); }

            //Some cards have 2+ names, so create an array of all the names in order
            string[] cmNames = cm.CardName.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (ConverterSet converterSet in converterSets.Values)
            {
                if (converterSet.IncludeInSearches)
                {
                    foreach (ConverterCard converterCard in converterSet.ConverterCards)
                    {
                        string[] ccNames = converterCard.Name.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        int numIndexToCompare = Math.Min(cmNames.Length, ccNames.Length);

                        //Compare all names.  If one card has less names than the other, then just compare indexes where 
                        //both names are present because sometimes another format only includes the first name
                        bool isMatch = true;
                        for (int i = 0; i < numIndexToCompare; i++)
                        {
                            //Remove extra whitespace
                            string ccName = ccNames[i].Trim();
                            string cmName = cmNames[i].Trim();

                            //Remove funny apostrophes (Happens sometimes when copied from a webpage)
                            ccName = ccName.Replace('’', '\'');
                            cmName = cmName.Replace('’', '\'');

                            //Change 'Æ' to "Ae"
                            ccName = ccName.Replace("Æ", "Ae");
                            cmName = cmName.Replace("Æ", "Ae");

                            if (!ccName.Equals(cmName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                isMatch = false;
                                break;
                            }
                        }

                        if (isMatch)
                        {
                            cm.AddPotentialOCTGNCard(converterCard);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Splits a string into an array, split on newlines.  Empty lines are not included.
        /// (http://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string[] SplitLines(string text)
        {
            return text == null ?
                new string[] {} :
                text.Split
                (
                    new string[] { "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                );
        }

        //Courtesy of: http://codereview.stackexchange.com/a/1592
        private static Dictionary<string, string> GetParams(string uri)
        {
            var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            return matches.Cast<Match>().ToDictionary(
                m => Uri.UnescapeDataString(m.Groups[2].Value),
                m => Uri.UnescapeDataString(m.Groups[3].Value)
            );
        }

        #endregion Helpers
    }
}
