// -----------------------------------------------------------------------
// <copyright file="SimpleDeckBuilderPluginController.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Octgn.Core.DataManagers;
using Octgn.Core.Plugin;
using Octgn.DataNew.Entities;

namespace DeckBuilderPluginController
{
    /// <summary>
    /// A simple DeckBuilderPluginController intended for testing Plugins
    /// </summary>
    public class SimpleDeckBuilderPluginController : IDeckBuilderPluginController
    {
        /// <summary>
        /// Gets the GameManager which knows about installed games.
        /// </summary>
        public GameManager Games
        {
            get
            {
                return GameManager.Get();
            }
        }

        /// <summary>
        /// Backing Field for the currently loaded Game
        /// </summary>
        private Game _LoadedGame;

        /// <summary>
        /// Sets the loaded game in the Deck Editor
        /// </summary>
        /// <param name="game">The game to load</param>
        public void SetLoadedGame(Game game)
        {
            this._LoadedGame = game;
        }

        /// <summary>
        /// Gets the loaded game in the Deck Editor
        /// </summary>
        /// <returns>Returns the loaded game in the Deck Editor</returns>
        public Game GetLoadedGame()
        {
            return this._LoadedGame;
        }

        /// <summary>
        /// Backing Field for the currently loaded Deck
        /// </summary>
        private IDeck _LoadedDeck;

        /// <summary>
        /// Gets the loaded deck in the Deck Editor
        /// </summary>
        /// <returns>Returns the loaded deck in the Deck Editor</returns>
        public IDeck GetLoadedDeck()
        {
            return this._LoadedDeck;
        }

        /// <summary>
        /// Loads a deck into the Deck Editor
        /// </summary>
        /// <param name="deck">The deck to load</param>
        public void LoadDeck(IDeck deck)
        {
            this._LoadedDeck = deck;
        }
    }
}
