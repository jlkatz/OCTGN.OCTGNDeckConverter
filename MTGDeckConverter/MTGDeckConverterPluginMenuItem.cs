// -----------------------------------------------------------------------
// <copyright file="MTGDeckConverterPluginMenuItem.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octgn.Core.Plugin;
using Octgn.DataNew.Entities;

namespace MTGDeckConverter
{
    /// <summary>
    /// Implements IPluginMenuItem so it can be used as a Plugin for OCTGN.
    /// </summary>
    public class MTGDeckConverterPluginMenuItem : IPluginMenuItem
    {
        /// <summary>
        /// Gets the name to display on the Plugins menu
        /// </summary>
        public string Name
        {
            get 
            {
                return "Magic: the Gathering Deck Converter";
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
            // Attempt to get the MTG Game from the IDeckBuilderPluginController
            Game mtgGame = MTGDeckConverterPluginMenuItem.GetGameDefinition(controller);

            if (mtgGame != null)
            {
                // Initialize the ConverterDatabase with the MTG Game
                Model.ConverterDatabase.SingletonInstance.Initialize(mtgGame);

                MainWindow mainWindow = new MainWindow();
            
                MTGDeckConverter.ViewModel.ImportDeckWizardVM importDeckWizardVM = new MTGDeckConverter.ViewModel.ImportDeckWizardVM();

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
                        controller.SetLoadedGame(mtgGame);
                        controller.LoadDeck(importDeckWizardVM.Converter.CreateDeck());
                    }
                };

                mainWindow.DataContext = importDeckWizardVM;

                // This will block until the Main Window is closed
                mainWindow.ShowDialog();

                // Clean up the Singleton items, regardless of whether the Wizard did anything or not
                Model.ConverterDatabase.SingletonInstance.UpdateSetsExcludedFromSearches();
                Model.SettingsManager.SingletonInstance.SaveSettingsManager();
                Model.ConverterDatabase.SingletonInstance.Cleanup();
            }
            else
            {
                System.Windows.MessageBox.Show
                (
                    "The Game Definition for Magic: the Gathering could not be found, check that it is installed in OCTGN correctly.",
                    "Game Definition Not Found"
                );
            }
        }

        /// <summary>
        /// Attempts to retrieve and return a reference to the MTG Game Definition from the IDeckBuilderPluginController.
        /// </summary>
        /// <param name="controller">The controller which is using this plugin.  Usually it is OCTGN</param>
        /// <returns>Octgn Game object for MTG if installed, null otherwise.</returns>
        private static Game GetGameDefinition(IDeckBuilderPluginController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException();
            }

            return controller.Games.Games.FirstOrDefault(g => g.Id == Guid.Parse("A6C8D2E8-7CD8-11DD-8F94-E62B56D89593"));
        }
    }
}
