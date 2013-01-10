using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MTGDeckConverter.Model
{
    public class ConverterMapping : INotifyPropertyChanged
    {
        public ConverterMapping(string cardName, string cardSet, int quantity)
        {
            this.CardName = cardName;
            this.CardSet = cardSet;
            this.Quantity = quantity;
        }

        #region Public Properties

        public string CardName
        {
            get;
            private set;
        }

        public string CardSet
        {
            get;
            private set;
        }

        public int Quantity
        {
            get;
            private set;
        }

        private ObservableCollection<ConverterCard> _PotentialOCTGNCards = new ObservableCollection<ConverterCard>();
        private ReadOnlyObservableCollection<ConverterCard> _PotentialOCTGNCardsReadOnly;
        public ReadOnlyObservableCollection<ConverterCard> PotentialOCTGNCards
        {
            get 
            {
                if (_PotentialOCTGNCardsReadOnly == null)
                { _PotentialOCTGNCardsReadOnly = new ReadOnlyObservableCollection<ConverterCard>(_PotentialOCTGNCards); }
                return _PotentialOCTGNCardsReadOnly;
            }
        }
        
        internal const string SelectedOCTGNCardPropertyName = "SelectedOCTGNCard";
        private ConverterCard _SelectedOCTGNCard;
        public ConverterCard SelectedOCTGNCard
        {
            get { return _SelectedOCTGNCard; }
            set { this.SetValue(ref _SelectedOCTGNCard, value, SelectedOCTGNCardPropertyName); }
        }

        #endregion Public Properties

        #region Public Methods

        //Automatically chooses the OCTGN card with the largest MultiverseID
        public void AutoSelectPotentialOCTGNCard()
        {
            if (this.PotentialOCTGNCards.Count == 0)
            {
                this.SelectedOCTGNCard = null;
            }
            else
            {
                ConverterCard maxMultiverseIDCard = this.PotentialOCTGNCards.First();
                foreach (ConverterCard cc in this.PotentialOCTGNCards)
                {
                    if (cc.MultiverseID > maxMultiverseIDCard.MultiverseID)
                    {
                        maxMultiverseIDCard = cc;
                    }
                }
                this.SelectedOCTGNCard = maxMultiverseIDCard;
            }
        }

        public bool AddPotentialOCTGNCard(ConverterCard potentialCard)
        {
            if (_PotentialOCTGNCards.Contains(potentialCard))
            { return false; }

            if(_PotentialOCTGNCards.Count == 0)
            {
                //Add it since it is the only item
                _PotentialOCTGNCards.Add(potentialCard);
            }
            else if(_PotentialOCTGNCards.Last().MultiverseID > potentialCard.MultiverseID)
            {
                //Add it at the end
                _PotentialOCTGNCards.Add(potentialCard);
            }
            else
            {
                int index = 0;
                for(int i = 0; i < _PotentialOCTGNCards.Count; i++)
                {
                    if(_PotentialOCTGNCards[i].MultiverseID > potentialCard.MultiverseID)
                    { break; }
                    else
                    { index = i; }
                }
                _PotentialOCTGNCards.Insert(index, potentialCard);
            }
            return true;
        }

        public bool RemovePotentialOCTGNCard(ConverterCard potentialCard)
        {
            return _PotentialOCTGNCards.Remove(potentialCard);
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
