using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace MTGDeckConverter.Model
{
    public class SettingsManager : INotifyPropertyChanged
    {
        public const string CONST_SettingsFilenameString = "Settings.xml";

        private string _FullPathName;

        #region Constructor

        private SettingsManager()
        {
            _FullPathName = System.IO.Path.Combine
            (
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                SettingsManager.CONST_SettingsFilenameString
            );

            this.ImportSettings();
        }

        #endregion Constructor

        #region Singleton

        private static SettingsManager _SingletonInstance;
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

        internal const string OpenFileDirectoryPropertyName = "OpenFileDirectory";
        public string OpenFileDirectory
        {
            get;
            set;
        }

        internal const string SaveFileDirectoryPropertyName = "SaveFileDirectory";
        public string SaveFileDirectory
        {
            get;
            set;
        }

        internal const string SetsExcludedFromSearchesPropertyName = "SetsExcludedFromSearches";
        internal const string SetString = "Set";
        private List<Guid> _SetsExcludedFromSearches = new List<Guid>();
        public List<Guid> SetsExcludedFromSearches
        {
            get { return _SetsExcludedFromSearches; }
            set { _SetsExcludedFromSearches = value; }
        }

        #endregion Public Properties

        #region IO

        private void ImportSettings()
        {
            //If the settings file doesn't exist, then don't try to import it, just leave and use the defaults.
            if (!System.IO.File.Exists(_FullPathName))
            {
                return;
            }

            XmlTextReader reader = new XmlTextReader(_FullPathName);
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

            if (childNodes.ContainsKey(OpenFileDirectoryPropertyName)) { this.OpenFileDirectory = childNodes[OpenFileDirectoryPropertyName].InnerText; }
            if (childNodes.ContainsKey(SaveFileDirectoryPropertyName)) { this.SaveFileDirectory = childNodes[SaveFileDirectoryPropertyName].InnerText; }
            if (childNodes.ContainsKey(SetsExcludedFromSearchesPropertyName)) 
            {
                this.SetsExcludedFromSearches.AddRange
                (
                    SettingsManager.CreateSetsExcludedFromSearchesListFromXmlElement(childNodes[SetsExcludedFromSearchesPropertyName])
                );
            }
        }

        private static IEnumerable<Guid> CreateSetsExcludedFromSearchesListFromXmlElement(XmlNode xmlNode)
        {
            var setsExcludedFromSearches = new List<Guid>();
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                setsExcludedFromSearches.Add(Guid.Parse(child.InnerText));
            }
            return setsExcludedFromSearches;
        }

        internal const string SettingsManagerString = "SettingsManager";
        public bool SaveSettingsManager()
        {
            XmlDocument parentXmlDoc = new XmlDocument();

            //XML Declaration
            parentXmlDoc.AppendChild(parentXmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", ""));

            XmlElement xmlElem = parentXmlDoc.CreateElement("", SettingsManagerString, "");

            xmlElem.AppendChild(XmlIOHelpers.CreateKeyValueXmlElement(OpenFileDirectoryPropertyName, this.OpenFileDirectory, parentXmlDoc));
            xmlElem.AppendChild(XmlIOHelpers.CreateKeyValueXmlElement(SaveFileDirectoryPropertyName, this.SaveFileDirectory, parentXmlDoc));

            //SetsExcludedFromSearches
            XmlElement setsExcludedFromSearches = parentXmlDoc.CreateElement("", SettingsManager.SetsExcludedFromSearchesPropertyName, "");
            foreach (Guid setGuid in this.SetsExcludedFromSearches)
            {
                setsExcludedFromSearches.AppendChild(XmlIOHelpers.CreateKeyValueXmlElement(SetString, setGuid, parentXmlDoc));
            }
            xmlElem.AppendChild(setsExcludedFromSearches);

            parentXmlDoc.AppendChild(xmlElem);
            try
            {
                parentXmlDoc.Save(_FullPathName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        #endregion IO

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //http://www.pochet.net/blog/2010/06/25/inotifypropertychanged-implementations-an-overview/
        protected bool SetValue<T>(ref T property, T value, string propertyName)
        {
            if (Object.Equals(property, value))
            {
                return false;
            }
            property = value;

            this.OnPropertyChanged(propertyName);

            return true;
        }

        #endregion
    }
}
