// -----------------------------------------------------------------------
// <copyright file="XmlIOHelpers.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// Provides helper static functions for XML operations
    /// </summary>
    internal static class XmlIOHelpers
    {
        /// <summary>
        /// Helper method to create an XML Element which contains a single key and value
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="value">The value, which will be converted to String</param>
        /// <param name="parentXmlDoc">The XMLDocument which this XmlElement will be added to</param>
        /// <returns>A new XmlElement which contains the key and value data</returns>
        internal static XmlElement CreateKeyValueXmlElement(string key, object value, XmlDocument parentXmlDoc)
        {
            XmlElement xmlElem = parentXmlDoc.CreateElement(string.Empty, key, string.Empty);
            xmlElem.InnerText = value == null ? string.Empty : value.ToString();
            return xmlElem;
        }

        /// <summary>
        /// Gets a Dictionary of XmlNodes where each Key is the name of a child XMLNode
        /// of the xmlNode parameter, and also exists in the nodeNames List.  
        /// </summary>
        /// <param name="xmlNode">The XML Node to get child key-values of</param>
        /// <param name="nodeNames">The list of Node Names to search for and include in the returning Dictionary</param>
        /// <returns>A Dictionary of XmlNodes whose keys exist as children of xmlNode, and also in nodeNames</returns>
        internal static Dictionary<string, XmlNode> GetChildNodesWithMatchingNames(XmlNode xmlNode, List<string> nodeNames)
        {
            if (nodeNames == null) 
            { 
                throw new ArgumentNullException(); 
            }

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
