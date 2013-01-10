using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MTGDeckConverter.ViewModel
{
    public class WizardPage_CompareCards : ImportDeckWizardPageVM
    {
        public WizardPage_CompareCards(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        { }

        public override bool ShowNextStepCommand
        {
            get { return true; }
        }

        public override bool ShowStartOverCommand
        {
            get { return true; }
        }

        public override string Title
        {
            get { return "Compare Cards"; }
        }

        internal const string MouseOverConverterCardPropertyName = "MouseOverConverterCard";
        private Model.ConverterCard _MouseOverConverterCard;
        public Model.ConverterCard MouseOverConverterCard
        {
            get { return _MouseOverConverterCard; }
            set 
            {
                if (this.SetValue(ref _MouseOverConverterCard, value, MouseOverConverterCardPropertyName))
                {
                    this.MouseOverConverterCardImage = _MouseOverConverterCard == null ?
                        null :
                        ImportDeckWizardVM.GetCardBitmapImage(_MouseOverConverterCard.CardID);
                }
            }
        }
        
        internal const string MouseOverConverterCardImagePropertyName = "MouseOverConverterCardImage";
        private BitmapImage _MouseOverConverterCardImage;
        public BitmapImage MouseOverConverterCardImage
        {
            get { return _MouseOverConverterCardImage; }
            set { this.SetValue(ref _MouseOverConverterCardImage, value, MouseOverConverterCardImagePropertyName); }
        }
    }
}
