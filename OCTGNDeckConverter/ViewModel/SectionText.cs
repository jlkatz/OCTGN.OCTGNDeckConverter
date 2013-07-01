// -----------------------------------------------------------------------
// <copyright file="SectionText.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using OCTGNDeckConverter.Model;

namespace OCTGNDeckConverter.ViewModel
{
    /// <summary>
    /// Contains the raw text entered by the user for a Section of the Deck to import
    /// </summary>
    public class SectionText : INotifyPropertyChangedBase
    {
        /// <summary>
        /// Initializes a new instance of the SectionText class.
        /// </summary>
        /// <param name="sectionName">The name of the section as it appears in the Deck.</param>
        public SectionText(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new ArgumentNullException("sectionName");
            }

            this.SectionName = sectionName;
        }

        /// <summary>
        /// Gets the name of the deck Section this object is recording user input for.
        /// </summary>
        public string SectionName
        {
            get;
            private set;
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string TextPropertyName = "Text";

        /// <summary>
        /// Private backing field
        /// </summary>
        private string _Text;

        /// <summary>
        /// Gets or sets the text that represents cards to be placed in this Section of the deck.
        /// </summary>
        public string Text
        {
            get { return this._Text; }
            set { this.SetValue(ref this._Text, value, TextPropertyName); }
        }
    }
}
