// -----------------------------------------------------------------------
// <copyright file="ConvertEngine.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OCTGNDeckConverter.Model.ConvertEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ConvertEngine
    {
        /// <summary>
        /// The logger instance for this class.
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Private backing store for the Singleton Instance
        /// </summary>
        private static ConvertEngine singletonInstance;

        private List<Game.GameConverter> gameConverters;

        /// <summary>
        /// Creates a new instance of the TransporterServer class.
        /// </summary>
        private ConvertEngine()
        {
            this.InstantiateGameConverters();
        }

        /// <summary>
        /// Gets the Singleton Instance of this class
        /// </summary>
        public static ConvertEngine SingletonInstance
        {
            get
            {
                if (singletonInstance == null)
                {
                    singletonInstance = new ConvertEngine();
                }
                return singletonInstance;
            }
        }


        /// <summary>
        /// Populates the gameConverters collection with all of the possible Games that have been defined
        /// </summary>
        private void InstantiateGameConverters()
        {
            this.gameConverters = new List<Game.GameConverter>();

            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Game.GameConverter)) && type.IsClass)
                {
                    ConstructorInfo ci = type.GetConstructor(new Type[] { });
                    this.gameConverters.Add((Game.GameConverter)ci.Invoke(new Object[] { }));
                }
            }
        }

        /// <summary>
        /// Converts a text file into a ConverterDeck which has all ConverterMappings populated with potential cards from the ConverterGame
        /// </summary>
        /// <param name="fullPathName">The full path name of the Deck file to convert</param>
        /// <param name="converterGame">The ConverterGame instance that will be used for searching for matches</param>
        /// <returns>A ConverterDeck which has all ConverterMappings populated with potential OCTGN cards from the converterSets</returns>
        public ConverterDeck ConvertFile(string fullPathName, ConverterGame converterGame)
        {
            Logger.Info("Converting the file " + fullPathName + " to the game " + converterGame.Game.Name);

            // Try to find a pre-defined GameConverter to handle ConvertFile
            Game.GameConverter gameConverter = this.FindMatchingGameConverter(converterGame);

            if (gameConverter != null)
            {
                return gameConverter.ConvertFile(fullPathName, converterGame);
            }

            throw new NotImplementedException("Converting a File for game " + converterGame.Game.Name + " has not been implemented yet.");
        }

        /// <summary>
        /// Converts a URL into a ConverterDeck which has all ConverterMappings populated with potential cards from the ConverterGame
        /// </summary>
        /// <param name="url">The URL of the Deck</param>
        /// <param name="converterGame">The ConverterGame instance that will be used for searching for matches</param>
        /// <returns>A ConverterDeck which has all ConverterMappings populated with potential OCTGN cards from the converterSets</returns>
        public ConverterDeck ConvertURL(string url, ConverterGame converterGame)
        {
            Logger.Info("Converting the url " + url + " to the game " + converterGame.Game.Name);

            // Try to find a pre-defined GameConverter to handle ConvertFile
            Game.GameConverter gameConverter = this.FindMatchingGameConverter(converterGame);

            if (gameConverter != null)
            {
                return gameConverter.ConvertURL(url, converterGame);
            }

            throw new NotImplementedException("Converting a URL for game " + converterGame.Game.Name + " has not been implemented yet.");
        }

        /// <summary>
        /// Converts user input text into a ConverterDeck which has all ConverterMappings populated with potential cards from the ConverterGame
        /// </summary>
        /// <param name="sectionsText">A collection of section names (keys), and the user input text of all cards in the section (values)</param>
        /// <param name="converterGame">The ConverterGame instance that will be used for searching for matches</param>
        /// <returns>A ConverterDeck which has all ConverterMappings populated with potential OCTGN cards from the converterSets</returns>
        public ConverterDeck ConvertText(Dictionary<string, string> sectionsText, ConverterGame converterGame)
        {
            Logger.Info("Converting text to the game " + converterGame.Game.Name);

            // Try to find a pre-defined GameConverter to handle ConvertText
            Game.GameConverter gameConverter = this.FindMatchingGameConverter(converterGame);

            if (gameConverter != null)
            {
                return gameConverter.ConvertText(sectionsText, converterGame);
            }
            else
            {
                return TextConverter.ConvertText(sectionsText, converterGame.Sets, converterGame.DeckSectionNames);
            }
        }

        public Game.GameConverter FindMatchingGameConverter(ConverterGame converterGame)
        {
            return this.gameConverters.FirstOrDefault(gc => gc.GameGuid == converterGame.Game.Id);
        }
    }
}