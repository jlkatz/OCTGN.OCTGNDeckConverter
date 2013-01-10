using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MTGDeckConverter.Model
{
    public class Converter : INotifyPropertyChanged
    {
        public Converter()
        {
            this.DeckSourceType = null;
        }

        #region Public Properties

        public ConverterDatabase ConverterDatabase
        {
            get { return ConverterDatabase.SingletonInstance; }
        }

        private const string DeckFullPathNamePropertyName = "DeckFullPathName";
        private string _DeckFullPathName;
        public string DeckFullPathName
        {
            get { return _DeckFullPathName; }
            set { SetValue(ref _DeckFullPathName, value, DeckFullPathNamePropertyName); }
        }

        private const string DeckNamePropertyName = "DeckName";
        private string _DeckName = "Imported Deck";
        public string DeckName
        {
            get { return _DeckName; }
            set 
            {
                //http://stackoverflow.com/a/847251
                string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                string invalidReStr = string.Format(@"[{0}]+", invalidChars);
                string validFilenameValue =  System.Text.RegularExpressions.Regex.Replace(value, invalidReStr, "_");

                SetValue(ref _DeckName, validFilenameValue, DeckNamePropertyName); 
            }
        }

        private const string DeckURLPropertyName = "DeckURL";
        private string _DeckURL;
        public string DeckURL
        {
            get { return _DeckURL; }
            set { SetValue(ref _DeckURL, value, DeckURLPropertyName); }
        }

        private List<ConverterMapping> _MainDeckMappings = new List<ConverterMapping>();
        public ReadOnlyCollection<ConverterMapping> MainDeckMappings
        {
            get { return _MainDeckMappings.AsReadOnly(); }
        }

        private const string MainDeckCountPropertyName = "MainDeckCount";
        private int _MainDeckCount;
        public int MainDeckCount
        {
            get { return _MainDeckCount; }
            set { SetValue(ref _MainDeckCount, value, MainDeckCountPropertyName); }
        }

        private const string MainDeckTextPropertyName = "MainDeckText";
        private string _MainDeckText;
        public string MainDeckText
        {
            get { return _MainDeckText; }
            set { SetValue(ref _MainDeckText, value, MainDeckTextPropertyName); }
        }

        private List<ConverterMapping> _SideBoardMappings = new List<ConverterMapping>();
        public ReadOnlyCollection<ConverterMapping> SideBoardMappings
        {
            get { return _SideBoardMappings.AsReadOnly(); }
        }

        private const string SideBoardCountPropertyName = "SideBoardCount";
        private int _SideBoardCount;
        public int SideBoardCount
        {
            get { return _SideBoardCount; }
            set { SetValue(ref _SideBoardCount, value, SideBoardCountPropertyName); }
        }

        private const string SideBoardTextPropertyName = "SideBoardText";
        private string _SideBoardText;
        public string SideBoardText
        {
            get { return _SideBoardText; }
            set { SetValue(ref _SideBoardText, value, SideBoardTextPropertyName); }
        }

        public DeckSourceTypes? DeckSourceType
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        public Tuple<bool, string> Convert()
        {
            //Before attempting to convert, ensure that the card database is fully built.
            //If OCTGN takes too long building the database, give up
            if(!this.ConverterDatabase.WaitForInitializationToComplete(TimeSpan.FromSeconds(30)))
            {
                throw new TimeoutException("Timeout while building the Card Database");
            }

            if (!this.DeckSourceType.HasValue)
            { return new Tuple<bool, string>(false, "Deck Source has not been chosen"); }

            Tuple<bool, IEnumerable<ConverterMapping>, IEnumerable<ConverterMapping>, string, string> conversionResult = null;
            try
            {
                switch (this.DeckSourceType.Value)
                {
                    case DeckSourceTypes.File:
                        conversionResult = ConvertEngine.ConvertFile(this.DeckFullPathName, this.ConverterDatabase.Sets);
                        break;

                    case DeckSourceTypes.Webpage:
                        conversionResult = ConvertEngine.ConvertURL(this.DeckURL, this.ConverterDatabase.Sets);
                        break;

                    case DeckSourceTypes.Text:
                        conversionResult = ConvertEngine.ConvertText(this.MainDeckText, this.SideBoardText, this.ConverterDatabase.Sets);
                        break;
                }
            }
            catch (Exception e)
            {
                return new Tuple<bool, string>(false, e.ToString());
            }

            _MainDeckMappings = new List<ConverterMapping>(conversionResult.Item2);
            foreach (ConverterMapping cm in _MainDeckMappings)
            { cm.AutoSelectPotentialOCTGNCard(); }

            _SideBoardMappings = new List<ConverterMapping>(conversionResult.Item3);
            foreach (ConverterMapping cm in _SideBoardMappings)
            { cm.AutoSelectPotentialOCTGNCard(); }

            if (!string.IsNullOrWhiteSpace(conversionResult.Item4))
            {
                this.DeckName = conversionResult.Item4;
            }

            this.UpdateCardCounts();

            return new Tuple<bool, string>(conversionResult.Item1, conversionResult.Item5);
        }

        public void SaveDeck(string fullPathName)
        {
            Octgn.Data.Deck d = new Octgn.Data.Deck(this.ConverterDatabase.GameDefinition);

            //[0] = "Main"
            //[1] = "Sideboard"
            //[2] = "Command Zone"
            //[3] = "Planes/Schemes"

            Octgn.Data.Deck.Section mainDeckSection = d.Sections.First(s => s.Name.Equals("Main", StringComparison.InvariantCultureIgnoreCase));
            Octgn.Data.Deck.Section sideboardSection = d.Sections.First(s => s.Name.Equals("Sideboard", StringComparison.InvariantCultureIgnoreCase));

            List<Tuple<Octgn.Data.Deck.Section, IEnumerable<ConverterMapping>>> pairSectionAndMappingsList = new List<Tuple<Octgn.Data.Deck.Section, IEnumerable<ConverterMapping>>>()
            {
                new Tuple<Octgn.Data.Deck.Section, IEnumerable<ConverterMapping>>(mainDeckSection, this.MainDeckMappings),
                new Tuple<Octgn.Data.Deck.Section, IEnumerable<ConverterMapping>>(sideboardSection, this.SideBoardMappings),
            };

            foreach (var pair in pairSectionAndMappingsList)
            {
                foreach (ConverterMapping converterMapping in pair.Item2)
                {
                    if (converterMapping.SelectedOCTGNCard != null)
                    {
                        pair.Item1.Cards.Add
                        (
                            new Octgn.Data.Deck.Element
                            {
                                Card = this.ConverterDatabase.GameDefinition.GetCardById(converterMapping.SelectedOCTGNCard.CardID),
                                Quantity = (byte)converterMapping.Quantity
                            }
                        );
                    }
                }
            }

            d.Save(fullPathName);
        }

        public void UpdateCardCounts()
        {
            this.MainDeckCount = (
                from cm in this.MainDeckMappings
                where cm.SelectedOCTGNCard != null
                select cm.Quantity
            ).Sum();

            this.SideBoardCount = (
                from cm in this.SideBoardMappings
                where cm.SelectedOCTGNCard != null
                select cm.Quantity
            ).Sum();
        }

        #endregion Public Methods

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
