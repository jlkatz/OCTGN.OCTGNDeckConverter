// -----------------------------------------------------------------------
// <copyright file="HtmlAgilityPackWrapper.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OCTGNDeckConverter.Model
{
    /// <summary>
    /// Provides a wrapper to access the HtmlAgilityPack.dll 
    /// assembly when it is dynamically loaded at runtime
    /// </summary>
    public static class HtmlAgilityPackWrapper
    {
        #region Fields

        /// <summary>
        /// Private backing field
        /// </summary>
        private static Assembly _HTMLAgilityPackAssembly;

        /// <summary>
        /// Private backing field
        /// </summary>
        private static Type[] _HTMLAgilityPackTypes;

        /// <summary>
        /// Private backing field
        /// </summary>
        private static bool _IsInitialized;

        #endregion Fields

        /// <summary>
        /// Initializes everything the wrapper needs if it hasn't already been initialized.
        /// </summary>
        /// <remarks>Extracts the DLL to disk if it doesn't exist, then loads the 
        /// assembly and plucks types, methods and properties that are needed.</remarks>
        private static void InitializeIfNeeded()
        {
            if (!HtmlAgilityPackWrapper._IsInitialized)
            {
                string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string htmlAgilityPackDLLFilename = @"HtmlAgilityPack.dll";
                string htmlAgilityPackFullPathName = System.IO.Path.Combine(exeDir, htmlAgilityPackDLLFilename);

                // Extract the DLL if missing or outdated
                if (!System.IO.File.Exists(htmlAgilityPackFullPathName))
                {
                    System.IO.File.WriteAllBytes(htmlAgilityPackFullPathName, OCTGNDeckConverter.Properties.Resources.HtmlAgilityPack);
                }
                else
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(htmlAgilityPackFullPathName);
                    Version ver = assembly.GetName().Version;

                    // Check the version of HtmlAgilityPack
                    if (ver.Major != 1 || ver.Minor != 4 || ver.Build != 6)
                    {
                        System.IO.File.WriteAllBytes(htmlAgilityPackFullPathName, OCTGNDeckConverter.Properties.Resources.HtmlAgilityPack);
                    }
                }

                if (System.IO.File.Exists(htmlAgilityPackFullPathName))
                {
                    HtmlAgilityPackWrapper._HTMLAgilityPackAssembly = System.Reflection.Assembly.LoadFile(htmlAgilityPackFullPathName);
                    HtmlAgilityPackWrapper._HTMLAgilityPackTypes = HtmlAgilityPackWrapper._HTMLAgilityPackAssembly.GetTypes();

                    HtmlAgilityPackWrapper.InitializeHtmlWeb();
                    HtmlAgilityPackWrapper.InitializeHtmlDocument();
                    HtmlAgilityPackWrapper.InitializeHtmlNode();
                    HtmlAgilityPackWrapper.InitializeHtmlAttribute();

                    HtmlAgilityPackWrapper._IsInitialized = true;
                }
                else
                {
                    throw new InvalidOperationException("HTMLAgilityPack.dll cannot be found.  Ensure it is in the same directory, or reinstall the plugin.");
                }
            }
        }

        #region HtmlWeb

        /// <summary>
        /// Private backing field for the HtmlWeb type
        /// </summary>
        private static Type _HtmlWebType;

        /// <summary>
        /// Private backing field for the collection of HtmlWeb methods
        /// </summary>
        private static MethodInfo[] _HtmlWebMethods;

        /// <summary>
        /// Private backing field for the HtmlWeb.Load(...) method
        /// </summary>
        private static MethodInfo _HtmlWeb_Method_Load;

        /// <summary>
        /// Initializes everything the wrapper needs for HtmlWeb type
        /// </summary>
        private static void InitializeHtmlWeb()
        {
            HtmlAgilityPackWrapper._HtmlWebType = HtmlAgilityPackWrapper._HTMLAgilityPackTypes.First(t => t.Name.Equals("HtmlWeb"));

            HtmlAgilityPackWrapper._HtmlWebMethods = HtmlAgilityPackWrapper._HtmlWebType.GetMethods();
            HtmlAgilityPackWrapper._HtmlWeb_Method_Load = HtmlAgilityPackWrapper._HtmlWebMethods.First(t => t.Name.Equals("Load"));
        }

        /// <summary>
        /// Returns a new instance of HtmlWeb.  Although the return type is object, it is actually an HtmlWeb.
        /// </summary>
        /// <returns>A new instance of HtmlWeb</returns>
        public static object HtmlWeb_CreateInstance()
        {
            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return Activator.CreateInstance(HtmlAgilityPackWrapper._HtmlWebType, new object[] { });
        }

        /// <summary>
        /// Returns the result of calling the HtmlWeb.Load(url) method on the HtmlWeb instance.  
        /// Although the return type is object, it is actually an HtmlDocument.
        /// </summary>
        /// <param name="htmlWebInstance">The instance of HtmlWeb to invoke Load(url) on</param>
        /// <param name="url">The requested URL</param>
        /// <returns>The result of calling the HtmlWeb.Load(url) method on the HtmlWeb instance</returns>
        public static object HtmlWeb_InvokeMethod_Load(object htmlWebInstance, string url)
        {
            if (!HtmlAgilityPackWrapper._HtmlWebType.IsAssignableFrom(htmlWebInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlWebType + " is not assignable from htmlWebInstance (" + htmlWebInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return HtmlAgilityPackWrapper._HtmlWeb_Method_Load.Invoke(htmlWebInstance, new object[] { url });
        }

        #endregion HtmlWeb

        #region HtmlDocument

        /// <summary>
        /// Private backing field for the HtmlDocument type
        /// </summary>
        private static Type _HtmlDocumentType;

        /// <summary>
        /// Private backing field for the collection of HtmlDocument properties
        /// </summary>
        private static PropertyInfo[] _HtmlDocumentProperties;

        /// <summary>
        /// Private backing field for the HtmlDocument.DocumentNode property
        /// </summary>
        private static PropertyInfo _HtmlDocument_Property_DocumentNode;

        /// <summary>
        /// Initializes everything the wrapper needs for HtmlDocument type
        /// </summary>
        private static void InitializeHtmlDocument()
        {
            HtmlAgilityPackWrapper._HtmlDocumentType = HtmlAgilityPackWrapper._HTMLAgilityPackTypes.First(t => t.Name.Equals("HtmlDocument"));

            HtmlAgilityPackWrapper._HtmlDocumentProperties = HtmlAgilityPackWrapper._HtmlDocumentType.GetProperties();
            HtmlAgilityPackWrapper._HtmlDocument_Property_DocumentNode = HtmlAgilityPackWrapper._HtmlDocumentProperties.First(t => t.Name.Equals("DocumentNode"));
        }

        /// <summary>
        /// Returns the HtmlDocument.DocumentNode property of the HtmlDocument instance.
        /// Although the return type is object, it is actually an HtmlNode.
        /// </summary>
        /// <param name="htmlDocumentInstance">The instance of HtmlDocument to get the property from</param>
        /// <returns>The HtmlDocument.DocumentNode property of the HtmlDocument instance</returns>
        public static object HtmlDocument_GetProperty_DocumentNode(object htmlDocumentInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlDocumentType.IsAssignableFrom(htmlDocumentInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlDocumentType + " is not assignable from htmlDocumentInstance (" + htmlDocumentInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return HtmlAgilityPackWrapper._HtmlDocument_Property_DocumentNode.GetValue(htmlDocumentInstance, null);
        }

        #endregion HtmlDocument

        #region HtmlNode

        /// <summary>
        /// Private backing field for the HtmlNode type
        /// </summary>
        private static Type _HtmlNodeType;

        /// <summary>
        /// Private backing field for the collection of HtmlNode methods
        /// </summary>
        private static MethodInfo[] _HtmlNodeMethods;

        /// <summary>
        /// Private backing field for the HtmlNode.SelectNodes(...) method
        /// </summary>
        private static MethodInfo _HtmlNode_Method_SelectNodes;

        /// <summary>
        /// Private backing field for the HtmlNode.SelectSingleNode(...) method
        /// </summary>
        private static MethodInfo _HtmlNode_Method_SelectSingleNode;

        /// <summary>
        /// Private backing field for the collection of HtmlNode properties
        /// </summary>
        private static PropertyInfo[] _HtmlNodeProperties;

        /// <summary>
        /// Private backing field for the HtmlNode.Attributes property
        /// </summary>
        private static PropertyInfo _HtmlNode_Property_Attributes;

        /// <summary>
        /// Private backing field for the HtmlNode.ChildNodes property
        /// </summary>
        private static PropertyInfo _HtmlNode_Property_ChildNodes;

        /// <summary>
        /// Private backing field for the HtmlNode.InnerText property
        /// </summary>
        private static PropertyInfo _HtmlNode_Property_InnerText;

        /// <summary>
        /// Private backing field for the HtmlNode.Name property
        /// </summary>
        private static PropertyInfo _HtmlNode_Property_Name;

        /// <summary>
        /// Initializes everything the wrapper needs for HtmlNode type
        /// </summary>
        private static void InitializeHtmlNode()
        {
            HtmlAgilityPackWrapper._HtmlNodeType = HtmlAgilityPackWrapper._HTMLAgilityPackTypes.First(t => t.Name.Equals("HtmlNode"));

            HtmlAgilityPackWrapper._HtmlNodeMethods = HtmlAgilityPackWrapper._HtmlNodeType.GetMethods();
            HtmlAgilityPackWrapper._HtmlNode_Method_SelectNodes = HtmlAgilityPackWrapper._HtmlNodeMethods.First(t => t.Name.Equals("SelectNodes"));
            HtmlAgilityPackWrapper._HtmlNode_Method_SelectSingleNode = HtmlAgilityPackWrapper._HtmlNodeMethods.First(t => t.Name.Equals("SelectSingleNode"));

            HtmlAgilityPackWrapper._HtmlNodeProperties = HtmlAgilityPackWrapper._HtmlNodeType.GetProperties();
            HtmlAgilityPackWrapper._HtmlNode_Property_Attributes = HtmlAgilityPackWrapper._HtmlNodeProperties.First(t => t.Name.Equals("Attributes"));
            HtmlAgilityPackWrapper._HtmlNode_Property_ChildNodes = HtmlAgilityPackWrapper._HtmlNodeProperties.First(t => t.Name.Equals("ChildNodes"));
            HtmlAgilityPackWrapper._HtmlNode_Property_InnerText = HtmlAgilityPackWrapper._HtmlNodeProperties.First(t => t.Name.Equals("InnerText"));
            HtmlAgilityPackWrapper._HtmlNode_Property_Name = HtmlAgilityPackWrapper._HtmlNodeProperties.First(t => t.Name.Equals("Name"));
        }

        /// <summary>
        /// Returns the result of calling the HtmlNode.SelectNodes(xpath) method on the HtmlNode instance as an IEnumerable.
        /// Although the items in the returned collection are type object, they are actually HtmlNode.
        /// </summary>
        /// <param name="htmlNodeInstance">The instance of HtmlNode to invoke SelectNodes(xpath) on</param>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>The result of calling the HtmlNode.SelectNodes(xpath) method on the HtmlNode instance as an IEnumerable</returns>
        public static System.Collections.IEnumerable HtmlNode_InvokeMethod_SelectNodes(object htmlNodeInstance, string xpath)
        {
            if (!HtmlAgilityPackWrapper._HtmlNodeType.IsAssignableFrom(htmlNodeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlNodeType + " is not assignable from htmlNodeInstance (" + htmlNodeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return (System.Collections.IEnumerable)HtmlAgilityPackWrapper._HtmlNode_Method_SelectNodes.Invoke(htmlNodeInstance, new object[] { xpath });
        }

        /// <summary>
        /// Returns the result of calling the HtmlNode.SelectSingleNode(xpath) method on the HtmlNode instance.
        /// Although the return type is object, it is actually an HtmlNode.
        /// </summary>
        /// <param name="htmlNodeInstance">The instance of HtmlNode to invoke SelectNodes(xpath) on</param>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>The result of calling the HtmlNode.SelectSingleNode(xpath) method on the HtmlNode instance</returns>
        public static object HtmlNode_InvokeMethod_SelectSingleNode(object htmlNodeInstance, string xpath)
        {
            if (!HtmlAgilityPackWrapper._HtmlNodeType.IsAssignableFrom(htmlNodeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlNodeType + " is not assignable from htmlNodeInstance (" + htmlNodeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return HtmlAgilityPackWrapper._HtmlNode_Method_SelectSingleNode.Invoke(htmlNodeInstance, new object[] { xpath });
        }

        /// <summary>
        /// Returns the HtmlNode.Attributes property of the HtmlNode instance as an IEnumerable.
        /// Although the items in the returned collection are type object, they are actually HtmlAttribute.
        /// </summary>
        /// <param name="htmlNodeInstance">The instance of HtmlNode to get the property from</param>
        /// <returns>The HtmlNode.Attributes property of the HtmlNode instance as an IEnumerable</returns>
        internal static System.Collections.IEnumerable HtmlNode_GetProperty_Attributes(object htmlNodeInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlNodeType.IsAssignableFrom(htmlNodeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlNodeType + " is not assignable from htmlNodeInstance (" + htmlNodeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return (System.Collections.IEnumerable)HtmlAgilityPackWrapper._HtmlNode_Property_Attributes.GetValue(htmlNodeInstance, null);
        }

        /// <summary>
        /// Returns the HtmlNode.ChildNodes property of the HtmlNode instance as an IEnumerable.
        /// Although the items in the returned collection are type object, they are actually HtmlNode.
        /// </summary>
        /// <param name="htmlNodeInstance">The instance of HtmlNode to get the property from</param>
        /// <returns>The HtmlNode.ChildNodes property of the HtmlNode instance as an IEnumerable</returns>
        public static System.Collections.IEnumerable HtmlNode_GetProperty_ChildNodes(object htmlNodeInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlNodeType.IsAssignableFrom(htmlNodeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlNodeType + " is not assignable from htmlNodeInstance (" + htmlNodeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return (System.Collections.IEnumerable)HtmlAgilityPackWrapper._HtmlNode_Property_ChildNodes.GetValue(htmlNodeInstance, null);
        }

        /// <summary>
        /// Returns the HtmlNode.InnerText property of the HtmlNode instance as a string.
        /// Any HTML characters are already decoded to ascii.
        /// </summary>
        /// <param name="htmlNodeInstance">The instance of HtmlNode to get the property from</param>
        /// <returns>The HtmlNode.InnerText property of the HtmlNode instance as a string</returns>
        public static string HtmlNode_GetProperty_InnerText(object htmlNodeInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlNodeType.IsAssignableFrom(htmlNodeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlNodeType + " is not assignable from htmlNodeInstance (" + htmlNodeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();

            string innerTextString = (string)HtmlAgilityPackWrapper._HtmlNode_Property_InnerText.GetValue(htmlNodeInstance, null);

            // Before returning, convert HTML characters to ASCII.  For example, &#39; ==> '
            return System.Net.WebUtility.HtmlDecode(innerTextString);
        }

        /// <summary>
        /// Returns the HtmlNode.Name property of the HtmlNode instance as a string.
        /// </summary>
        /// <param name="htmlNodeInstance">The instance of HtmlNode to get the property from</param>
        /// <returns>The HtmlNode.Name property of the HtmlNode instance as a string</returns>
        public static string HtmlNode_GetProperty_Name(object htmlNodeInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlNodeType.IsAssignableFrom(htmlNodeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlNodeType + " is not assignable from htmlNodeInstance (" + htmlNodeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return (string)HtmlAgilityPackWrapper._HtmlNode_Property_Name.GetValue(htmlNodeInstance, null);
        }

        #endregion HtmlNode

        #region HtmlAttribute

        /// <summary>
        /// Private backing field for the HtmlAttribute type
        /// </summary>
        private static Type _HtmlAttributeType;

        /// <summary>
        /// Private backing field for the collection of HtmlAttribute properties
        /// </summary>
        private static PropertyInfo[] _HtmlAttributeProperties;

        /// <summary>
        /// Private backing field for the HtmlAttribute.Name property
        /// </summary>
        private static PropertyInfo _HtmlAttribute_Property_Name;

        /// <summary>
        /// Private backing field for the HtmlAttribute.Value property
        /// </summary>
        private static PropertyInfo _HtmlAttribute_Property_Value;

        /// <summary>
        /// Initializes everything the wrapper needs for HtmlNode type
        /// </summary>
        private static void InitializeHtmlAttribute()
        {
            HtmlAgilityPackWrapper._HtmlAttributeType = HtmlAgilityPackWrapper._HTMLAgilityPackTypes.First(t => t.Name.Equals("HtmlAttribute"));

            HtmlAgilityPackWrapper._HtmlAttributeProperties = HtmlAgilityPackWrapper._HtmlAttributeType.GetProperties();
            HtmlAgilityPackWrapper._HtmlAttribute_Property_Name = HtmlAgilityPackWrapper._HtmlAttributeProperties.First(t => t.Name.Equals("Name"));
            HtmlAgilityPackWrapper._HtmlAttribute_Property_Value = HtmlAgilityPackWrapper._HtmlAttributeProperties.First(t => t.Name.Equals("Value"));
        }

        /// <summary>
        /// Returns the HtmlAttribute.Name property of the HtmlAttribute instance as a string.
        /// </summary>
        /// <param name="htmlAttributeInstance">The instance of HtmlAttribute to get the property from</param>
        /// <returns>The HtmlAttribute.Name property of the HtmlAttribute instance as a string</returns>
        public static string HtmlAttribute_GetProperty_Name(object htmlAttributeInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlAttributeType.IsAssignableFrom(htmlAttributeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlAttributeType + " is not assignable from htmlAttributeInstance (" + htmlAttributeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return (string)HtmlAgilityPackWrapper._HtmlAttribute_Property_Name.GetValue(htmlAttributeInstance, null);
        }

        /// <summary>
        /// Returns the HtmlAttribute.Value property of the HtmlAttribute instance as a string.
        /// </summary>
        /// <param name="htmlAttributeInstance">The instance of HtmlAttribute to get the property from</param>
        /// <returns>The HtmlAttribute.Value property of the HtmlAttribute instance as a string</returns>
        public static string HtmlAttribute_GetProperty_Value(object htmlAttributeInstance)
        {
            if (!HtmlAgilityPackWrapper._HtmlAttributeType.IsAssignableFrom(htmlAttributeInstance.GetType()))
            {
                throw new InvalidOperationException(HtmlAgilityPackWrapper._HtmlAttributeType + " is not assignable from htmlAttributeInstance (" + htmlAttributeInstance.GetType() + ")");
            }

            HtmlAgilityPackWrapper.InitializeIfNeeded();
            return (string)HtmlAgilityPackWrapper._HtmlAttribute_Property_Value.GetValue(htmlAttributeInstance, null);
        }

        #endregion HtmlAttribute
    }
}
