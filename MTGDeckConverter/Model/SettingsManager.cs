// -----------------------------------------------------------------------
// <copyright file="SettingsManager.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// Contains the settings for the MTGDeckConverter program in a Singleton instance.  
    /// Can also read and write settings from/to a file.
    /// </summary>
    public class SettingsManager : INotifyPropertyChangedBase
    {
        /// <summary>
        /// The default name for the settings file which contains all configuration data
        /// </summary>
        public const string CONST_SettingsFilenameString = "Settings.xml";

        /// <summary>
        /// The full path name of the settings file to be used
        /// </summary>
        private string _FullPathName;

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingsManager"/> class from being created.
        /// Creates a new instance of the SettingsManager class.  This should not be called directly, use SingletonInstance instead
        /// </summary>
        private SettingsManager()
        {
            // Set _FullPathName to the full path name of "Settings.xml" in the same directory as the converter program
            this._FullPathName = System.IO.Path.Combine
            (
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                SettingsManager.CONST_SettingsFilenameString
            );

            this.ImportSettings();
        }

        #endregion Constructor

        #region Singleton

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private static SettingsManager _SingletonInstance;

        /// <summary>
        /// Gets the Singleton Instance of the SettingsManager
        /// </summary>
        public static SettingsManager SingletonInstance
        {
            get
            {
                if (_SingletonInstance == null)
                {
                    _SingletonInstance = new SettingsManager();
                }

                return _SingletonInstance;
            }
        }

        #endregion Singleton

        #region Public Properties

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string OpenFileDirectoryPropertyName = "OpenFileDirectory";
        
        /// <summary>
        /// Gets or sets the last directory used to open a file from
        /// </summary>
        public string OpenFileDirectory
        {
            get;
            set;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SaveFileDirectoryPropertyName = "SaveFileDirectory";
        
        /// <summary>
        /// Gets or sets the last directory used to save a file to
        /// </summary>
        public string SaveFileDirectory
        {
            get;
            set;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SetsExcludedFromSearchesPropertyName = "SetsExcludedFromSearches";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SetString = "Set";
        
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private List<Guid> _SetsExcludedFromSearches = new List<Guid>();
        
        /// <summary>
        /// Gets or sets a list of Sets (by their Guid) that should be excluded when searching
        /// </summary>
        public List<Guid> SetsExcludedFromSearches
        {
            get { return this._SetsExcludedFromSearches; }
            set { this._SetsExcludedFromSearches = value; }
        }

        #endregion Public Properties

        #region IO

        /// <summary>
        /// Reads the Settings file and sets all the values of the singleton-instance of SettingsManager accordingly.  If the
        /// settings file doesn't exist, it simply leaves everything at default.
        /// </summary>
        private void ImportSettings()
        {
            // If the settings file doesn't exist, then don't try to import it, just leave and use the defaults.
            if (!System.IO.File.Exists(this._FullPathName))
            {
                return;
            }

            XmlTextReader reader = new XmlTextReader(this._FullPathName);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            XmlDocument xd = new XmlDocument();
            xd.Load(reader);

            XmlNode xnodDE = xd.DocumentElement;

            if (xnodDE.Name != SettingsManagerString)
            {
                throw new InvalidOperationException("Root node is not " + SettingsManagerString);
            }

            var childNodes = XmlIOHelpers.GetChildNodesWithMatchingNames
            (
                xnodDE,
                new List<string>()
                {
                    OpenFileDirectoryPropertyName,
                    SaveFileDirectoryPropertyName,
                    SetsExcludedFromSearchesPropertyName,
                }

            );

            if (childNodes.ContainsKey(OpenFileDirectoryPropertyName)) 
            { 
                this.OpenFileDirectory = childNodes[OpenFileDirectoryPropertyName].InnerText; 
            }

            if (childNodes.ContainsKey(SaveFileDirectoryPropertyName)) 
            { 
                this.SaveFileDirectory = childNodes[SaveFileDirectoryPropertyName].InnerText; 
            }
            
            if (childNodes.ContainsKey(SetsExcludedFromSearchesPropertyName)) 
            {
                this.SetsExcludedFromSearches.AddRange
                (
                    SettingsManager.CreateSetsExcludedFromSearchesListFromXmlElement(childNodes[SetsExcludedFromSearchesPropertyName])
                );
            }
        }

        /// <summary>
        /// Returns a list of Set Guids found within the xmlNode which should be excluded from searches.
        /// </summary>
        /// <param name="xmlNode">XmlNode which contains the sets to be excluded</param>
        /// <returns>A list of Set Guids found within the xmlNode which should be excluded from searches.</returns>
        private static IEnumerable<Guid> CreateSetsExcludedFromSearchesListFromXmlElement(XmlNode xmlNode)
        {
            var setsExcludedFromSearches = new List<Guid>();
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                setsExcludedFromSearches.Add(Guid.Parse(child.InnerText));
            }

            return setsExcludedFromSearches;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SettingsManagerString = "SettingsManager";

        /// <summary>
        /// Saves all settings found in the singleton instance of SettingsManager to the default filename in the execution directory.
        /// </summary>
        /// <returns>True if successfully saved, false if not</returns>
        public bool SaveSettingsManager()
        {
            XmlDocument parentXmlDoc = new XmlDocument();

            // XML Declaration
            parentXmlDoc.AppendChild(parentXmlDoc.CreateNode(XmlNodeType.XmlDeclaration, string.Empty, string.Empty));

            XmlElement xmlElem = parentXmlDoc.CreateElement(string.Empty, SettingsManagerString, string.Empty);

            xmlElem.AppendChild(XmlIOHelpers.CreateKeyValueXmlElement(OpenFileDirectoryPropertyName, this.OpenFileDirectory, parentXmlDoc));
            xmlElem.AppendChild(XmlIOHelpers.CreateKeyValueXmlElement(SaveFileDirectoryPropertyName, this.SaveFileDirectory, parentXmlDoc));

            // SetsExcludedFromSearches
            XmlElement setsExcludedFromSearches = parentXmlDoc.CreateElement(string.Empty, SettingsManager.SetsExcludedFromSearchesPropertyName, string.Empty);
            foreach (Guid setGuid in this.SetsExcludedFromSearches)
            {
                setsExcludedFromSearches.AppendChild(XmlIOHelpers.CreateKeyValueXmlElement(SetString, setGuid, parentXmlDoc));
            }

            xmlElem.AppendChild(setsExcludedFromSearches);

            parentXmlDoc.AppendChild(xmlElem);
            try
            {
                parentXmlDoc.Save(this._FullPathName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        #endregion IO
    }
}
