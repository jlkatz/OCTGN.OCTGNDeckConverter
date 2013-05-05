// -----------------------------------------------------------------------
// <copyright file="WizardPage_CompareCards.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// Represents a step in the Import Deck Wizard to be displayed by a View which
    /// shows the user all of the imported Cards, and choices of potential matches for them to change.
    /// </summary>
    public class WizardPage_CompareCards : ImportDeckWizardPageVM
    {
        /// <summary>
        /// Initializes a new instance of the WizardPage_CompareCards class.
        /// </summary>
        /// <param name="importDeckWizardVM">The parent Wizard that will use this Page</param>
        public WizardPage_CompareCards(ImportDeckWizardVM importDeckWizardVM)
            : base(importDeckWizardVM)
        { 
        }

        #region Public Properties

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string MouseOverConverterCardPropertyName = "MouseOverConverterCard";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private Model.ConverterCard _MouseOverConverterCard;

        /// <summary>
        /// Gets or sets the ConverterCard that the mouse is currently hovering over in a View
        /// </summary>
        public Model.ConverterCard MouseOverConverterCard
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
        internal const string MouseOverConverterCardImagePropertyName = "MouseOverConverterCardImage";

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

        #endregion Public Properties

        #region WizardPageVM Overrides

        /// <summary>
        /// Gets a value indicating whether a View should show the Next Step command or not
        /// </summary>
        public override bool ShowNextStepCommand
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether a View should show the Start Over command or not
        /// </summary>
        public override bool ShowStartOverCommand
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the Title for this Page that should be shown by a View
        /// </summary>
        public override string Title
        {
            get { return "Compare Cards"; }
        }

        #endregion WizardPageVM Overrides
    }
}
