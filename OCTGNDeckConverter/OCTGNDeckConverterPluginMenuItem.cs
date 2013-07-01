// -----------------------------------------------------------------------
// <copyright file="OCTGNDeckConverterPluginMenuItem.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octgn.Core.Plugin;
using Octgn.DataNew.Entities;

namespace OCTGNDeckConverter
{
    /// <summary>
    /// Implements IPluginMenuItem so it can be used as a Plugin for OCTGN.
    /// </summary>
    public class OCTGNDeckConverterPluginMenuItem : IPluginMenuItem
    {
        /// <summary>
        /// Gets the name to display on the Plugins menu
        /// </summary>
        public string Name
        {
            get 
            {
                return "OCTGN Deck Converter";
            }
        }

        /// <summary>
        /// Should be called when the user clicks on the menu item for this Plugin.  It will launch the
        /// Deck Converter Wizard Window.  If the Wizard is completed successfully, the Deck will be 
        /// loaded into OCTGN's Deck Editor.
        /// </summary>
        /// <param name="controller">The controller which is using this plugin.  Usually it is OCTGN</param>
        public void OnClick(IDeckBuilderPluginController controller)
        {
            if (controller.Games.Games.Count() > 0)
            {
                // Initialize the ConverterDatabase2 with all available Games
                Model.ConverterDatabase.SingletonInstance.LoadGames(controller.Games.Games);

                MainWindow mainWindow = new MainWindow();
            
                OCTGNDeckConverter.ViewModel.ImportDeckWizardVM importDeckWizardVM = new OCTGNDeckConverter.ViewModel.ImportDeckWizardVM();

                // This will execute when the Close event is fired.  
                // If the Main Window is closed via 'X' button, this won't be executed.
                importDeckWizardVM.Close += (s, e) =>
                {
                    if 
                    (
                        importDeckWizardVM.Completed && 
                        importDeckWizardVM.WasNotCancelled
                    )
                    {
                        controller.SetLoadedGame(importDeckWizardVM.Converter.ConverterGame.Game);
                        controller.LoadDeck(importDeckWizardVM.Converter.CreateDeck());
                    }
                };

                mainWindow.DataContext = importDeckWizardVM;

                // This will block until the Main Window is closed
                mainWindow.ShowDialog();

                // Clean up the Singleton items, regardless of whether the Wizard did anything or not
                Model.ConverterDatabase.SingletonInstance.UpdateSetsExcludedFromSearches();
                Model.SettingsManager.SingletonInstance.SaveSettingsManager();
            }
            else
            {
                System.Windows.MessageBox.Show
                (
                    "No Game Definitions could be found.  Check that you have installed them in OCTGN.",
                    "Game Definitions Not Found"
                );
            }
        }
    }
}
