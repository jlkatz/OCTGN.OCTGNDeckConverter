// -----------------------------------------------------------------------
// <copyright file="ConverterSection.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model
{
    /// <summary>
    /// Contains all data about a Section of a Deck being converted.
    /// </summary>
    public class ConverterSection : INotifyPropertyChangedBase
    {
        /// <summary>
        /// Initializes a new instance of the ConverterSection class.
        /// </summary>
        /// <param name="sectionName">The name of this section as it appears in the Deck.</param>
        public ConverterSection(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new ArgumentNullException("sectionName");
            }

            this.SectionName = sectionName;
        }

        /// <summary>
        /// Gets a UI-friendly version of the Section Name with count
        /// </summary>
        public string SectionNameAndCount
        {
            get { return this.SectionName + " (" + this.SectionCount + ")"; }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        private const string SectionCountPropertyName = "SectionCount";

        /// <summary>
        /// Private backing field
        /// </summary>
        private int _SectionCount;

        /// <summary>
        /// Gets the count of the number of Cards in the Section.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        public int SectionCount
        {
            get { return this._SectionCount; }
            private set { this.SetValue(ref this._SectionCount, value, SectionCountPropertyName); }
        }

        /// <summary>
        /// Gets the name of this deck Section this ConverterSection represents
        /// </summary>
        public string SectionName
        {
            get;
            private set;
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private ObservableCollection<ConverterMapping> _SectionMappings = new ObservableCollection<ConverterMapping>();

        /// <summary>
        /// Private backing field
        /// </summary>
        private ReadOnlyObservableCollection<ConverterMapping> _SectionMappingsReadOnly;

        /// <summary>
        /// Gets a Collection of all the ConverterMappings which belong in the SectionMappings section
        /// </summary>
        public ReadOnlyObservableCollection<ConverterMapping> SectionMappings
        {
            get
            {
                if (this._SectionMappingsReadOnly == null)
                {
                    this._SectionMappingsReadOnly = new ReadOnlyObservableCollection<ConverterMapping>(this._SectionMappings);
                }

                return this._SectionMappingsReadOnly;
            }
        }

        /// <summary>
        /// Adds the converterMapping to the list of SectionMappings
        /// </summary>
        /// <param name="converterMapping">The ConverterMapping to add</param>
        /// <returns>True if mapping was successfully added, False if not added</returns>
        public bool AddConverterMapping(ConverterMapping converterMapping)
        {
            if (!this._SectionMappings.Contains(converterMapping))
            {
                this._SectionMappings.Add(converterMapping);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the converterMapping from the SectionMappings list of mappings.
        /// </summary>
        /// <param name="converterMapping">The ConverterMapping to remove</param>
        /// <returns>True if mapping was successfully removed, False if not removed</returns>
        public bool RemoveConverterMapping(ConverterMapping converterMapping)
        {
            return this._SectionMappings.Remove(converterMapping);
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private ObservableCollection<string> incorrectlyFormattedLines = new ObservableCollection<string>();

        /// <summary>
        /// Private backing field
        /// </summary>
        private ReadOnlyObservableCollection<string> incorrectlyFormattedLinesReadOnly;

        public ReadOnlyObservableCollection<string> IncorrectlyFormattedLines
        {
            get
            {
                if (this.incorrectlyFormattedLinesReadOnly == null)
                {
                    this.incorrectlyFormattedLinesReadOnly = new ReadOnlyObservableCollection<string>(this.incorrectlyFormattedLines);
                }

                return this.incorrectlyFormattedLinesReadOnly;
            }
        }

        /// <summary>
        /// Adds an incorrectly Formatted line of text to the list of IncorrectlyFormattedLines
        /// </summary>
        /// <param name="incorrectlyFormattedLine">The incorrectlyFormatted line of text to add</param>
        public void AddIncorrectlyFormattedLine(string incorrectlyFormattedLine)
        {
            this.incorrectlyFormattedLines.Add(incorrectlyFormattedLine);
        }

        /// <summary>
        /// Returns a value indicating whether removing the specified incorrectly formatted line was successful or not
        /// </summary>
        /// <param name="incorrectlyFormattedLine">The incorrectlyFormatted line of text to remove</param>
        /// <returns>a value indicating whether removing the specified incorrectly formatted line was successful or not</returns>
        public bool RemoveIncorrectlyFormattedLine(string incorrectlyFormattedLine)
        {
            return this.incorrectlyFormattedLines.Remove(incorrectlyFormattedLine);
        }

        /// <summary>
        /// Updates the SectionCount property with an up-to-date count of the 
        /// number of Cards in the section.  This only includes the Cards which
        /// have a corresponding OCTGN Card selected; those without are not counted.
        /// </summary>
        public void UpdateSectionCount()
        {
            this.SectionCount = (
                from cm in this.SectionMappings
                where cm.SelectedOCTGNCard != null
                select cm.Quantity
            ).Sum();
        }
    }
}
