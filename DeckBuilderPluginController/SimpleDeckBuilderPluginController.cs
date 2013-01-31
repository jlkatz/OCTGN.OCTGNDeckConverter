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
using Octgn.Data;
using Octgn.Library.Plugin;

namespace DeckBuilderPluginController
{
    /// <summary>
    /// A simple DeckBuilderPluginController intended for testing Plugins
    /// </summary>
    public class SimpleDeckBuilderPluginController : IDeckBuilderPluginController
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private readonly GamesRepository _Games = new GamesRepository();

        /// <summary>
        /// Gets the games repository of installed games.
        /// </summary>
        public GamesRepository Games
        {
            get
            {
                return this._Games;
            }
        }

        /// <summary>
        /// Backing Field for the currently loaded Deck
        /// </summary>
        private Deck _LoadedDeck;

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
        /// Loads a deck into the Deck Editor
        /// </summary>
        /// <param name="deck">The deck to load</param>
        public void LoadDeck(Deck deck)
        {
            this._LoadedDeck = deck;
        }

        /// <summary>
        /// Gets the loaded deck in the Deck Editor
        /// </summary>
        /// <returns>Returns the loaded deck in the Deck Editor</returns>
        public Deck GetLoadedDeck()
        {
            return this._LoadedDeck;
        }
    }
}
