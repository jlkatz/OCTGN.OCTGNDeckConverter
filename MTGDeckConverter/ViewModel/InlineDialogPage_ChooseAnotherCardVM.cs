// -----------------------------------------------------------------------
// <copyright file="InlineDialogPage_ChooseAnotherCardVM.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using MTGDeckConverter.Model;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents dialog content to be displayed by a View which prompts the user 
    /// to choose a Card from the entire catalog of available cards that suitably
    /// matches the imported Card
    /// </summary>
    public class InlineDialogPage_ChooseAnotherCardVM : InlineDialogPageVM
    {
        /// <summary>
        /// Initializes a new instance of the InlineDialogPage_ChooseAnotherCardVM class.
        /// </summary>
        /// <param name="converterMapping">The Converter Mapping which contains the parsed card name to match to</param>
        public InlineDialogPage_ChooseAnotherCardVM(ConverterMapping converterMapping)
        {
            this.ConverterMapping = converterMapping;
        }

        /// <summary>
        /// Gets the ConverterMapping which contains the parsed card name to match to
        /// </summary>
        public ConverterMapping ConverterMapping
        {
            get;
            private set;
        }
        
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string MouseOverConverterCardPropertyName = "MouseOverConverterCard";
        
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ConverterCard _MouseOverConverterCard;
        
        /// <summary>
        /// Gets or sets the ConverterCard that the mouse is currently hovering over in a View
        /// </summary>
        public ConverterCard MouseOverConverterCard
        {
            get 
            { 
                return this._MouseOverConverterCard; 
            }

            set
            {
                if (this.SetValue(ref this._MouseOverConverterCard, value, MouseOverConverterCardPropertyName))
                {
                    this.MouseOverConverterCardImage = this._MouseOverConverterCard == null ?
                        null :
                        ImportDeckWizardVM.GetCardBitmapImage(this._MouseOverConverterCard.CardID);
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SelectedConverterCardPropertyName = "SelectedConverterCard";
        
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ConverterCard _SelectedConverterCard;
        
        /// <summary>
        /// Gets or sets the ConverterCard that is currently selected
        /// </summary>
        public ConverterCard SelectedConverterCard
        {
            get 
            {
                return this._SelectedConverterCard; 
            }

            set
            {
                if (this.SetValue(ref this._SelectedConverterCard, value, SelectedConverterCardPropertyName))
                {
                    this.OnPropertyChanged(OkButtonEnabledPropertyName);

                    this.SelectedConverterCardImage = this._MouseOverConverterCard == null ?
                        null :
                        ImportDeckWizardVM.GetCardBitmapImage(this._MouseOverConverterCard.CardID);
                }
            }
        }

        /// <summary>
        /// Gets a List of all the ConverterSets available in the ConverterDatabase
        /// </summary>
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

        /// <summary>
        /// Gets the Title for this dialog that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Choose a Matching Card for '" + this.ConverterMapping.CardName + "'"; }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string OkButtonEnabledPropertyName = "OkButtonEnabled";
        
        /// <summary>
        /// Gets a value indicating whether the Ok Button should be shown by a View
        /// </summary>
        public override bool OkButtonEnabled
        {
            get { return this.SelectedConverterCard != null; }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string MouseOverConverterCardImagePropertyName = "MouseOverConverterCardImage";
        
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private BitmapImage _MouseOverConverterCardImage;
        
        /// <summary>
        /// Gets a BitmapImage that is a picture of the card the mouse is currently hovering over
        /// </summary>
        public BitmapImage MouseOverConverterCardImage
        {
            get { return this._MouseOverConverterCardImage; }
            private set { this.SetValue(ref this._MouseOverConverterCardImage, value, MouseOverConverterCardImagePropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SelectedConverterCardImagePropertyName = "SelectedConverterCardImage";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private BitmapImage _SelectedConverterCardImage;
        
        /// <summary>
        /// Gets a BitmapImage that is a picture of the card that is currently selected
        /// </summary>
        public BitmapImage SelectedConverterCardImage
        {
            get { return this._SelectedConverterCardImage; }
            private set { this.SetValue(ref this._SelectedConverterCardImage, value, SelectedConverterCardImagePropertyName); }
        }
    }
}
