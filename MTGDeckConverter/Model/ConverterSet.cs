using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MTGDeckConverter.Model
{
    public class ConverterSet
    {
        #region Constructor

        public ConverterSet(Octgn.Data.Set octgnSet)
        {
            if (octgnSet == null) { throw new ArgumentNullException(); }
            this.OctgnSet = octgnSet;
        }

        #endregion Constructor

        #region Public Properties

        public Octgn.Data.Set OctgnSet
        {
            get;
            private set;
        }

        private bool _IncludeInSearches = true;
        public bool IncludeInSearches
        {
            get { return _IncludeInSearches; }
            set { _IncludeInSearches = value; }
        }

        private ConverterCard _MaxMultiverseIDCard;
        public int MaxMultiverseID
        {
            get
            {
                return _MaxMultiverseIDCard != null ?
                    _MaxMultiverseIDCard.MultiverseID :
                    int.MaxValue;
            }
        }

        private List<ConverterCard> _ConverterCards = new List<ConverterCard>();
        public ReadOnlyCollection<ConverterCard> ConverterCards
        {
            get { return _ConverterCards.AsReadOnly(); }
        }

        #endregion Public Properties

        #region Public Methods

        internal void AddNewConverterCard(Guid cardID, string name, int multiverseID)
        {
            ConverterCard newConverterCard = new ConverterCard(cardID, name, this.OctgnSet.Name, multiverseID);
            _ConverterCards.Add(newConverterCard);

            if 
            (
                _MaxMultiverseIDCard == null ||
                newConverterCard.MultiverseID > _MaxMultiverseIDCard.MultiverseID
            )
            {
                _MaxMultiverseIDCard = newConverterCard;
            }
        }

        internal void SortConverterCards()
        {
            _ConverterCards = _ConverterCards.OrderBy(cc => cc.Name).ToList();
        }

        #endregion Public Methods
    }
}
