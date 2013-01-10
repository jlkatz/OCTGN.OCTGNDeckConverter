using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTGDeckConverter.Model;
using System.Windows.Media.Imaging;

namespace MTGDeckConverter.ViewModel
{
    public class InlineDialogPage_ChooseAnotherCardVM : InlineDialogPageVM
    {
        public InlineDialogPage_ChooseAnotherCardVM(ConverterMapping converterMapping)
        {
            this.ConverterMapping = converterMapping;
        }

        public ConverterMapping ConverterMapping
        {
            get;
            private set;
        }

        private ConverterCard _MouseOverConverterCard;
        public ConverterCard MouseOverConverterCard
        {
            get { return _MouseOverConverterCard; }
            set
            {
                if (_MouseOverConverterCard != value)
                {
                    _MouseOverConverterCard = value;
                    RaisePropertyChanged("MouseOverConverterCard");

                    this.MouseOverConverterCardImage = _MouseOverConverterCard == null ?
                        null :
                        ImportDeckWizardVM.GetCardBitmapImage(_MouseOverConverterCard.CardID);
                }
            }
        }

        private ConverterCard _SelectedConverterCard;
        public ConverterCard SelectedConverterCard
        {
            get { return _SelectedConverterCard; }
            set 
            {
                if (_SelectedConverterCard != value)
                {
                    _SelectedConverterCard = value;
                    RaisePropertyChanged("SelectedConverterCard");
                    RaisePropertyChanged("OkButtonEnabled");

                    this.SelectedConverterCardImage = _MouseOverConverterCard == null ?
                        null :
                        ImportDeckWizardVM.GetCardBitmapImage(_MouseOverConverterCard.CardID);
                }
            }
        }

        public List<ConverterSet> Sets
        {
            get
            {
                return (
                    from set in ConverterDatabase.SingletonInstance.Sets.Values
                    orderby set.MaxMultiverseID descending
                    select set
                    ).ToList();
            }
        }

        public override string Title
        {
            get { return "Choose a Matching Card for '" + this.ConverterMapping.CardName + "'"; }
        }

        public override bool OkButtonEnabled
        {
            get { return this.SelectedConverterCard != null; }
        }

        internal const string MouseOverConverterCardImagePropertyName = "MouseOverConverterCardImage";
        private BitmapImage _MouseOverConverterCardImage;
        public BitmapImage MouseOverConverterCardImage
        {
            get { return _MouseOverConverterCardImage; }
            set
            {
                if (_MouseOverConverterCardImage != value)
                {
                    _MouseOverConverterCardImage = value;
                    RaisePropertyChanged(MouseOverConverterCardImagePropertyName);
                }
            }
        }

        internal const string SelectedConverterCardImagePropertyName = "SelectedConverterCardImage";
        private BitmapImage _SelectedConverterCardImage;
        public BitmapImage SelectedConverterCardImage
        {
            get { return _SelectedConverterCardImage; }
            set
            {
                if (_SelectedConverterCardImage != value)
                {
                    _SelectedConverterCardImage = value;
                    RaisePropertyChanged(SelectedConverterCardImagePropertyName);
                }
            }
        }
    }
}
