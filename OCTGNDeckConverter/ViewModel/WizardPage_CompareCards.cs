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
using Octgn.Core.DataExtensionMethods;

namespace OCTGNDeckConverter.ViewModel
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
             this.ShowHiddenFeatures =
                System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) ||
                System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
        }

        private GalaSoft.MvvmLight.Command.RelayCommand _ExportCardPictures;
        public GalaSoft.MvvmLight.Command.RelayCommand ExportCardPictures
        {
            get
            {
                if (this._ExportCardPictures == null)
                {
                    this._ExportCardPictures = new GalaSoft.MvvmLight.Command.RelayCommand
                    (
                        () =>
                        {
                            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
                            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                foreach (Model.ConverterSection cs in this.ImportDeckWizardVM.Converter.ConverterDeck.ConverterSections)
                                {
                                    foreach (Model.ConverterMapping cm in cs.SectionMappings)
                                    {
                                        if (cm.SelectedOCTGNCard != null)
                                        {
                                            Octgn.DataNew.Entities.Card octgnCard = this.ImportDeckWizardVM.Converter.ConverterGame.Game.AllCards().First(c => c.Id == cm.SelectedOCTGNCard.CardID);
                                            string sourceFileName = octgnCard.GetPicture();
                                            string extension = System.IO.Path.GetExtension(sourceFileName);

                                            for (int i = 0; i < cm.Quantity; i++)
                                            {
                                                string destFileName = System.IO.Path.Combine(dlg.SelectedPath, cm.SelectedOCTGNCard.Name + extension);
                                                int d = 1;
                                                while (System.IO.File.Exists(destFileName))
                                                {
                                                    destFileName = System.IO.Path.Combine(dlg.SelectedPath, cm.SelectedOCTGNCard.Name + " (" + d + ")" + extension);
                                                    d++;
                                                }

                                                try
                                                {
                                                    System.IO.File.Copy(sourceFileName, destFileName);
                                                }
                                                catch (Exception)
                                                { }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    );
                }
                return this._ExportCardPictures;
            }
        }

        #region Public Properties
        
        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string MouseOverConverterCardPropertyName = "MouseOverConverterCard";

        /// <summary>
        /// Private backing field
        /// </summary>
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
                        ImportDeckWizardVM.GetCardBitmapImage(this._MouseOverConverterCard.CardID, this.ImportDeckWizardVM.Converter.ConverterGame.Game);
                }
            }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string MouseOverConverterCardImagePropertyName = "MouseOverConverterCardImage";

        /// <summary>
        /// Private backing field
        /// </summary>
        private BitmapImage _MouseOverConverterCardImage;

        /// <summary>
        /// Gets a BitmapImage that is a picture of the card the mouse is currently hovering over
        /// </summary>
        public BitmapImage MouseOverConverterCardImage
        {
            get { return this._MouseOverConverterCardImage; }
            private set { this.SetValue(ref this._MouseOverConverterCardImage, value, MouseOverConverterCardImagePropertyName); }
        }

        public bool ShowHiddenFeatures
        {
            get;
            set;
        }

        #endregion Public Properties

        #region WizardPageVM Overrides

        /// <summary>
        /// Gets a value indicating whether this page is the first page shown to the user or not
        /// </summary>
        public override bool IsStartPage
        {
            get { return false; }
        }

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
        /// Gets the Subtitle for this Page that should be shown by a View
        /// </summary>
        public override string Subtitle
        {
            get { return this.ImportDeckWizardVM.Converter.ConverterGame.Game.Name; }
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
