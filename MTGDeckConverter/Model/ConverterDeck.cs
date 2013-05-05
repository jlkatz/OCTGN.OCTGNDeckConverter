// -----------------------------------------------------------------------
// <copyright file="ConverterDeck.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MTGDeckConverter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains all data about the Deck being converted.  A fancy Tuple
    /// </summary>
    public class ConverterDeck : INotifyPropertyChangedBase
    {
        #region Public Properties

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ObservableCollection<ConverterMapping> _MainDeck = new ObservableCollection<ConverterMapping>();

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ReadOnlyObservableCollection<ConverterMapping> _MainDeckReadOnly;

        /// <summary>
        /// Gets a Collection of all the ConverterMappings which belong in the MainDeck section
        /// </summary>
        public ReadOnlyObservableCollection<ConverterMapping> MainDeck
        {
            get
            {
                if (this._MainDeckReadOnly == null)
                {
                    this._MainDeckReadOnly = new ReadOnlyObservableCollection<ConverterMapping>(this._MainDeck);
                }

                return this._MainDeckReadOnly;
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string MainDeckCountPropertyName = "MainDeckCount";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private int _MainDeckCount;

        /// <summary>
        /// Gets the count of the number of Cards in the MainDeck.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        public int MainDeckCount
        {
            get { return this._MainDeckCount; }
            private set { this.SetValue(ref this._MainDeckCount, value, MainDeckCountPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ObservableCollection<ConverterMapping> _SideBoard = new ObservableCollection<ConverterMapping>();

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ReadOnlyObservableCollection<ConverterMapping> _SideBoardReadOnly;

        /// <summary>
        /// Gets a Collection of all the ConverterMappings which belong in the SideBoard section
        /// </summary>
        public ReadOnlyObservableCollection<ConverterMapping> SideBoard
        {
            get
            {
                if (this._SideBoardReadOnly == null)
                {
                    this._SideBoardReadOnly = new ReadOnlyObservableCollection<ConverterMapping>(this._SideBoard);
                }

                return this._SideBoardReadOnly;
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string SideBoardCountPropertyName = "SideBoardCount";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private int _SideBoardCount;

        /// <summary>
        /// Gets the count of the number of Cards in the SideBoard.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        public int SideBoardCount
        {
            get { return this._SideBoardCount; }
            private set { this.SetValue(ref this._SideBoardCount, value, SideBoardCountPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string DeckNamePropertyName = "DeckName";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private string _DeckName = string.Empty;

        /// <summary>
        /// Gets or sets the name of this Deck.  When set, invalid filename characters are automatically replaced with '_'.
        /// </summary>
        public string DeckName
        {
            get
            {
                return this._DeckName;
            }

            set
            {
                if (value != null)
                {
                    // http://stackoverflow.com/a/847251
                    string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                    string invalidReStr = string.Format(@"[{0}]+", invalidChars);
                    string validFilenameValue = System.Text.RegularExpressions.Regex.Replace(value, invalidReStr, "_");

                    this.SetValue(ref this._DeckName, validFilenameValue, DeckNamePropertyName);
                }
                else
                {
                    this.SetValue(ref this._DeckName, string.Empty, DeckNamePropertyName);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the conversion of the Deck to Octgn was successful or not.
        /// </summary>
        public bool ConversionSuccessful
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the mainDeckConverterMapping from the MainDeck list of mappings.
        /// </summary>
        /// <param name="mainDeckConverterMapping">The ConverterMapping to add</param>
        /// <returns>True if mapping was successfully added, False if not added</returns>
        public bool AddMainDeckConverterMapping(ConverterMapping mainDeckConverterMapping)
        {
            if (!this._MainDeck.Contains(mainDeckConverterMapping))
            {
                this._MainDeck.Add(mainDeckConverterMapping);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the sideBoardConverterMapping from the MainDeck list of mappings.
        /// </summary>
        /// <param name="sideBoardConverterMapping">The ConverterMapping to add</param>
        /// <returns>True if mapping was successfully added, False if not added</returns>
        public bool AddSideBoardConverterMapping(ConverterMapping sideBoardConverterMapping)
        {
            if (!this._SideBoard.Contains(sideBoardConverterMapping))
            {
                this._SideBoard.Add(sideBoardConverterMapping);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tell each ConverterMapping in both MainDeck and SideBoard to auto-select a potential OCTGN Card
        /// </summary>
        public void AutoSelectPotentialCards()
        {
            foreach (ConverterMapping converterMapping in this.MainDeck.Concat(this.SideBoard))
            {
                converterMapping.AutoSelectPotentialOCTGNCard();
            }
        }

        /// <summary>
        /// Populates every ConverterMapping in both MainDeck and SideBoard with potential cards from the converterSets
        /// </summary>
        /// <param name="converterSets">List of all ConverterSets. Only those with flag IncludeInSearches will be used.</param>
        public void PopulateConverterMappings(Dictionary<Guid, ConverterSet> converterSets)
        {
            foreach (ConverterMapping converterMapping in this.MainDeck.Concat(this.SideBoard))
            {
                converterMapping.PopulateWithPotentialCards(converterSets);
            }
        }

        /// <summary>
        /// Removes the mainDeckConverterMapping from the MainDeck list of mappings.
        /// </summary>
        /// <param name="mainDeckConverterMapping">The ConverterMapping to remove</param>
        /// <returns>True if mapping was successfully removed, False if not removed</returns>
        public bool RemoveMainDeckConverterMapping(ConverterMapping mainDeckConverterMapping)
        {
            return this._MainDeck.Remove(mainDeckConverterMapping);
        }

        /// <summary>
        /// Removes the sideBoardConverterMapping from the SideBoard list of mappings.
        /// </summary>
        /// <param name="sideBoardConverterMapping">The ConverterMapping to remove</param>
        /// <returns>True if mapping was successfully removed, False if not removed</returns>
        public bool RemoveSideBoardConverterMapping(ConverterMapping sideBoardConverterMapping)
        {
            return this._SideBoard.Remove(sideBoardConverterMapping);
        }

        /// <summary>
        /// Updates the MainDeckCount and SideBoardCount properties with an up-to-date 
        /// count of the number of Cards.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        public void UpdateCardCounts()
        {
            this.UpdateMainDeckCount();
            this.UpdateSideBoardCount();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Updates the MainDeckCount property with an up-to-date count of the 
        /// number of Cards in the MainDeck.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        private void UpdateMainDeckCount()
        {
            this.MainDeckCount = (
                from cm in this.MainDeck
                where cm.SelectedOCTGNCard != null
                select cm.Quantity
            ).Sum();
        }

        /// <summary>
        /// Updates the SideBoardCount property with an up-to-date count of the 
        /// number of Cards in the SideBoard.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        private void UpdateSideBoardCount()
        {
            this.SideBoardCount = (
                from cm in this.SideBoard
                where cm.SelectedOCTGNCard != null
                select cm.Quantity
            ).Sum();
        }

        #endregion Private Methods
    }
}
