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
using Octgn.Library.Plugin;
using Octgn.MTGDeckConverter;

namespace DeckBuilderPluginController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private SimpleDeckBuilderPluginController _SimpleDeckBuilderPluginController = new SimpleDeckBuilderPluginController();

        /// <summary>
        /// Gets the instance of the Simple DeckBuilderPluginController
        /// </summary>
        public SimpleDeckBuilderPluginController SimpleDeckBuilderPluginController
        {
            get { return this._SimpleDeckBuilderPluginController; }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private MTGDeckConverterPlugin _MTGDeckConverterPlugin = new MTGDeckConverterPlugin();

        /// <summary>
        /// Gets the MTGDeckConverterPlugin to be used for testing
        /// </summary>
        public MTGDeckConverterPlugin MTGDeckConverterPlugin
        {
            get { return this._MTGDeckConverterPlugin; }
        }

        /// <summary>
        /// Gets the MTGDeckConverterPluginMenuItem to be used for testing
        /// </summary>
        public IPluginMenuItem MTGDeckConverterPluginMenuItem
        {
            get { return this.MTGDeckConverterPlugin.MenuItems.First(); }
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
                this.MTGDeckConverterPluginMenuItem.OnClick(this.SimpleDeckBuilderPluginController);
            }
        }
    }
}
