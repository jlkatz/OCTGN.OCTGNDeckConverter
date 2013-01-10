namespace MTGDeckConverter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Provides helper static functions for XML operations
    /// </summary>
    public static class XmlIOHelpers
    {
        public static XmlElement CreateKeyValueXmlElement(string key, object value, XmlDocument parentXmlDoc)
        {
            XmlElement xmlElem = parentXmlDoc.CreateElement("", key, "");
            xmlElem.InnerText = value == null ? "" : value.ToString();
            return xmlElem;
        }

        public static Dictionary<string, XmlNode> GetChildNodesWithMatchingNames(XmlNode xmlNode, List<string> nodeNames)
        {
            if (nodeNames == null) { throw new ArgumentNullException(); }

            Dictionary<string, XmlNode> childNodeDict = new Dictionary<string, XmlNode>();

            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (nodeNames.Contains(child.Name))
                {
                    childNodeDict[child.Name] = child;
                }
            }
            return childNodeDict;
        }
    }
}
