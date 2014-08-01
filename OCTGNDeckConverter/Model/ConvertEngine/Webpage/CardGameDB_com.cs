using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public class CardGameDB_com : WebpageConverter
    {
        /// <summary>
        /// Gets the base URL for the cardgamedb.com website
        /// </summary>
        protected override string BaseURL
        {
            get { return "cardgamedb.com"; }
        }

        /// <summary>
        /// Converts a LoTR URL from cardgamedb.com into a ConverterDeck which is populated with all cards and deck name.
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
            //System.Collections.Specialized.NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
            //outgoingQueryString.Add("dguid", "lotrdeck_53bf7c75f0309");
            //outgoingQueryString.Add("fPage", "deckshare");
            //outgoingQueryString.Add("fgame", "lordoftherings");
            //outgoingQueryString.Add("md5check", "880ea6a14ea49e853634fbdc5015a024");
            //outgoingQueryString.Add("pid", "4226");
            //string postData = outgoingQueryString.ToString();

            //ASCIIEncoding ascii = new ASCIIEncoding();
            //byte[] postBytes = ascii.GetBytes(postData.ToString());

            //System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.cardgamedb.com/forums/index.php?s=a466da7d1dacdadb01d87a09c62a4754&app=ccs&module=ajax&section=lotrdeckbuilder&do=share");
            //request.Method = "POST";
            //request.Accept = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = postBytes.Length;
            //request.Host = "www.cardgamedb.com";
            //request.Referer = "http://www.cardgamedb.com/index.php/thelordoftherings/the-lord-of-the-rings-deck-share?p=4226&deck=lotrdeck_53bf7c75f0309";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";

            //// add post data to request
            //System.IO.Stream postStream = request.GetRequestStream();
            //postStream.Write(postBytes, 0, postBytes.Length);
            //postStream.Flush();
            //postStream.Close();

            //System.Net.WebResponse response = request.GetResponse();
            //System.IO.Stream dataStream = response.GetResponseStream();
            //System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            //string responseFromServer = reader.ReadToEnd();
            //reader.Close();
            //response.Close();
            //Console.WriteLine(responseFromServer);

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
    }
}
