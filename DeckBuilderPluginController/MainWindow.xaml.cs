// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Octgn.Core.Plugin;
using OCTGNDeckConverter;

namespace DeckBuilderPluginController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Text to show when the test GUI is first loaded
        /// </summary>
        private const string WELCOME_TEXT = "This is a shell to test OCTGNDeckConverter.  Choose it from the Plugins menu above.";
        
        /// <summary>
        /// Text to show when no deck was loaded (which might happen due to an error)
        /// </summary>
        private const string NO_DECK = "The Wizard completed, but no Deck was loaded.";

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.infoTextBlock.Text = WELCOME_TEXT;
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private SimpleDeckBuilderPluginController _SimpleDeckBuilderPluginController = new SimpleDeckBuilderPluginController();

        /// <summary>
        /// Gets the instance of the Simple DeckBuilderPluginController
        /// </summary>
        public SimpleDeckBuilderPluginController SimpleDeckBuilderPluginController
        {
            get { return this._SimpleDeckBuilderPluginController; }
        }

        /// <summary>
        /// Private backing field
        /// </summary>
        private OCTGNDeckConverterPlugin _OCTGNDeckConverterPlugin = new OCTGNDeckConverterPlugin();

        /// <summary>
        /// Gets the OCTGNDeckConverterPlugin to be used for testing
        /// </summary>
        public OCTGNDeckConverterPlugin OCTGNDeckConverterPlugin
        {
            get { return this._OCTGNDeckConverterPlugin; }
        }

        /// <summary>
        /// Gets the OCTGNDeckConverterPluginMenuItem to be used for testing
        /// </summary>
        public IPluginMenuItem OCTGNDeckConverterPluginMenuItem
        {
            get { return this.OCTGNDeckConverterPlugin.MenuItems.First(); }
        }

        /// <summary>
        /// Triggered when the user selects the Plugin menu item
        /// </summary>
        /// <param name="sender">Should be pluginMenuItem</param>
        /// <param name="e">Click event args</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.pluginMenuItem)
            {
                this.infoTextBlock.Text = WELCOME_TEXT;
                this.OCTGNDeckConverterPluginMenuItem.OnClick(this.SimpleDeckBuilderPluginController);

                Octgn.DataNew.Entities.IDeck deck = this.SimpleDeckBuilderPluginController.GetLoadedDeck();
                if (deck != null)
                {
                    this.infoTextBlock.Text = string.Empty;
                    this.deckDisplayer.Content = deck;
                }
                else
                {
                    this.infoTextBlock.Text = NO_DECK;
                }
            }
        }
    }
}
