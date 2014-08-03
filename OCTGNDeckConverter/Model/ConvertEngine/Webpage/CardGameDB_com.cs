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
            object htmlWebInstance = HtmlAgilityPackWrapper.HtmlWeb_CreateInstance();
            object htmlDocumentInstance = HtmlAgilityPackWrapper.HtmlWeb_InvokeMethod_Load(htmlWebInstance, url);
            object htmlDocument_DocumentNode = HtmlAgilityPackWrapper.HtmlDocument_GetProperty_DocumentNode(htmlDocumentInstance);

            // Find the block of javascript that contains the variable 'viewType'
            object rawDeckJavascriptNode = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectSingleNode(htmlDocument_DocumentNode, "//script[contains(text(), 'var viewType =')]");
            string rawDeckJavascriptText = HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(rawDeckJavascriptNode);
            string[] rawDeckJavascriptLines = rawDeckJavascriptText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            dynamic rawDeckJSON = null;

            string viewTypeLine = rawDeckJavascriptLines.FirstOrDefault(l => l.Contains("var viewType ="));
            if (viewTypeLine == null)
            {
                throw new InvalidOperationException("Could not find the javascript variable 'viewType'");
            }
            else if (viewTypeLine.Contains("submitted"))
            {
                // Since viewType is 'submitted', the deck is published.  In this case, the JSON data the deck is embedded on this page in the javascript variable 'rawdeck' 
                
                string rawDeckLine = rawDeckJavascriptLines.First(l => l.Contains("var rawdeck ="));
                
                // Trim everything except the JSON
                int openingCurlyBraceIndex = rawDeckLine.IndexOf('{');
                int closingCurlyBraceIndex = rawDeckLine.LastIndexOf('}');
                string rawDeckJSONString = rawDeckLine.Substring(openingCurlyBraceIndex, closingCurlyBraceIndex - openingCurlyBraceIndex + 1);
                rawDeckJSON = JsonConvert.DeserializeObject(rawDeckJSONString);
            }
            else if (viewTypeLine.Contains("share"))
            {
                // Since viewType is 'share', the deck is personal.  In this case, the JSON is not embedded on this page and must be fetched via a form POST

                Dictionary<string, string> urlParams = WebpageConverter.GetParams(url);

                // Find the block of javascript that contains the 'ipb.vars' dictionary variable
                object rawIPBVarsJavascriptNode = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectSingleNode(htmlDocument_DocumentNode, "//script[contains(text(), 'ipb.vars[')]");
                string rawIPBVarsJavascriptText = HtmlAgilityPackWrapper.HtmlNode_GetProperty_InnerText(rawIPBVarsJavascriptNode);
                string[] rawIPBVarsJavascriptLines = rawIPBVarsJavascriptText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                // Extract the 'secure_hash' value from IPBVar
                string secureHashLine = rawIPBVarsJavascriptLines.First(l => l.Contains(@"ipb.vars['secure_hash']"));
                string secure_hash = secureHashLine.Substring(0, secureHashLine.LastIndexOf('\''));
                secure_hash = secure_hash.Substring(secure_hash.LastIndexOf('\'') + 1);

                // Extract the 'secure_hash' value from IPBVar
                string baseURLLine = rawIPBVarsJavascriptLines.First(l => l.Contains(@"ipb.vars['base_url']"));
                string base_url = baseURLLine.Substring(0, baseURLLine.LastIndexOf('\''));
                base_url = base_url.Substring(base_url.LastIndexOf('\'') + 1);

                // These variables are all submitted with the form.  They are added via javascript in lotrdecksection.js
                System.Collections.Specialized.NameValueCollection outgoingQueryString = System.Web.HttpUtility.ParseQueryString(String.Empty);
                outgoingQueryString.Add("dguid", urlParams["deck"]);
                outgoingQueryString.Add("fPage", "deckshare");
                outgoingQueryString.Add("fgame", "lordoftherings");
                outgoingQueryString.Add("md5check", secure_hash);
                outgoingQueryString.Add("pid", urlParams["p"]);
                string postData = outgoingQueryString.ToString();

                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] postBytes = ascii.GetBytes(postData.ToString());

                string requestString = base_url + "app=ccs&module=ajax&section=lotrdeckbuilder&do=share";
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestString);
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postBytes.Length;
                request.Host = "www.cardgamedb.com";
                request.Referer = url;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";

                // add post data to request
                System.IO.Stream postStream = request.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
                postStream.Close();

                System.Net.WebResponse response = request.GetResponse();
                System.IO.Stream dataStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                string responseFromServer = System.Web.HttpUtility.HtmlDecode(reader.ReadToEnd());
                reader.Close();
                response.Close();
                rawDeckJSON = JsonConvert.DeserializeObject(responseFromServer);
                
                // When JSON is retrieved this way, the deck contents is stored in a sub-variable
                rawDeckJSON = rawDeckJSON.deckContents;
            }
            else
            {
                throw new InvalidOperationException("The javascript variable 'viewType' was not an expected value (" + viewTypeLine + ")");
            }

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
