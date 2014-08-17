using OCTGNDeckConverter.Model.ConvertEngine.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model.ConvertEngine.Webpage
{
    public class MTGVault_com : WebpageConverter
    {
        /// <summary>
        /// Gets the base URL for the mtgvault.com website
        /// </summary>
        protected override string BaseURL
        {
            get { return "mtgvault.com"; }
        }

        /// <summary>
        /// Converts a URL from mtgvault.com into a ConverterDeck which is populated with all cards and deck name.
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
            
            // Extract the '__VIEWSTATE' and '__EVENTVALIDATION' input values
            string viewstateValue = null;
            string eventValidationValue = null;

            // Get a collection of all the input nodes
            IEnumerable<object> aspnetFormInputNodes = HtmlAgilityPackWrapper.HtmlNode_InvokeMethod_SelectNodes(htmlDocument_DocumentNode, "//input");
            foreach (object inputNode in aspnetFormInputNodes)
            {
                // get the name of the input
                IEnumerable<object> attributes = HtmlAgilityPackWrapper.HtmlNode_GetProperty_Attributes(inputNode);
                string name = string.Empty;
                foreach (object attribute in attributes)
                {
                    if (HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Name(attribute).Equals("name", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name = HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Value(attribute);
                        break;
                    }
                }

                if (name.Equals(@"__VIEWSTATE", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (object attribute in attributes)
                    {
                        if (HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Name(attribute).Equals("value", StringComparison.InvariantCultureIgnoreCase))
                        {
                            viewstateValue = HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Value(attribute);
                            break;
                        }
                    }
                }
                else if (name.Equals(@"__EVENTVALIDATION", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (object attribute in attributes)
                    {
                        if (HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Name(attribute).Equals("value", StringComparison.InvariantCultureIgnoreCase))
                        {
                            eventValidationValue = HtmlAgilityPackWrapper.HtmlAttribute_GetProperty_Value(attribute);
                            break;
                        }
                    }
                }
            }

            System.Collections.Specialized.NameValueCollection outgoingQueryString = System.Web.HttpUtility.ParseQueryString(String.Empty);
            outgoingQueryString.Add(@"__EVENTTARGET", @"ctl00$ContentPlaceHolder1$LinkButton1");
            outgoingQueryString.Add(@"__EVENTARGUMENT", string.Empty);
            outgoingQueryString.Add(@"__VIEWSTATE", viewstateValue);
            outgoingQueryString.Add(@"__EVENTVALIDATION", eventValidationValue);            
            outgoingQueryString.Add(@"ctl00$ContentPlaceHolder1$TextBox_AutoCompleteSideBoard", string.Empty);
            outgoingQueryString.Add(@"ctl00$ContentPlaceHolder1$TextBox_QuantitySideBoard", string.Empty);
            outgoingQueryString.Add(@"ctl00$ContentPlaceHolder1$TextBox_BulkImportSideBoard", string.Empty);
            outgoingQueryString.Add(@"ctl00$Login1$TextBox_Username", string.Empty);
            outgoingQueryString.Add(@"ctl00$Login1$TextBox_Password", string.Empty);

            string postData = outgoingQueryString.ToString();

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postData.ToString());

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "text/html,application/xhtml+xml,application/xml";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            request.Host = "www.mtgvault.com";
            request.Referer = url;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";

            // add post data to request
            System.IO.Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Flush();
            postStream.Close();

            System.Net.WebResponse response = request.GetResponse();
            string filename = response.Headers["content-disposition"];
            System.IO.Stream dataStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            string responseFromServer = System.Web.HttpUtility.HtmlDecode(reader.ReadToEnd());
            reader.Close();
            response.Close();

            filename = filename.Replace(@"attachment;", string.Empty);
            filename = filename.Replace(@".txt", string.Empty);

            ConverterDeck converterDeck = convertGenericFileFunc(TextConverter.SplitLines(responseFromServer), deckSectionNames);
            converterDeck.DeckName = filename;
            return converterDeck;
        }
    }
}
