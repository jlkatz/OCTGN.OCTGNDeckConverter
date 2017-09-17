// -----------------------------------------------------------------------
// <copyright file="ConverterDatabase.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverter.Model
{
    /// <summary>
    /// Singleton object which contains the Octgn.Data.Game definitions for all OCTGN games.  As needed, it
    /// will instantiate ConverterGame objects to wrap each Octgn.Data.Game for the Wizard to use.
    /// </summary>
    public class ConverterDatabase : INotifyPropertyChangedBase
    {
        /// <summary>
        /// The logger instance for this class.
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Fields

        /// <summary>
        /// Private backing field that contains each OCTGN Game, and the corresponding ConverterGame instance if it exists
        /// </summary>
        private Dictionary<Octgn.DataNew.Entities.Game, ConverterGame> _ConverterGameDictionary = new Dictionary<Octgn.DataNew.Entities.Game, ConverterGame>();

        /// <summary>
        /// Private backing field that indicates whether OCTGN games have been loaded into the Singleton instance or not
        /// </summary>
        private bool _Loaded = false;

        #endregion Fields

        /// <summary>
        /// Prevents a default instance of the <see cref="ConverterDatabase"/> class from being created.
        /// </summary>
        private ConverterDatabase()
        {
        }

        #region Singleton

        /// <summary>
        /// The private backing field for SingletonInstance
        /// </summary>
        private static ConverterDatabase _SingletonInstance;

        /// <summary>
        /// Gets the Singleton instance of this class.  This is the only way to access this class.
        /// </summary>
        public static ConverterDatabase SingletonInstance
        {
            get
            {
                if (_SingletonInstance == null)
                {
                    _SingletonInstance = new ConverterDatabase();
                }

                return _SingletonInstance;
            }
        }

        #endregion Singleton

        #region Public Properties

        /// <summary>
        /// Gets a collection of all the available OCTGN Games
        /// </summary>
        public IEnumerable<Octgn.DataNew.Entities.Game> OctgnGames
        {
            get { return this._ConverterGameDictionary.Keys.ToList(); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the ConverterGame corresponding to the specified OCTGN Game.  It will be created if it doesn't exist
        /// </summary>
        /// <param name="octgnGame">The corresponding OCTGN game to get the ConverterGame for</param>
        /// <returns>The corresponding ConverterGame</returns>
        public ConverterGame GetConverterGame(Octgn.DataNew.Entities.Game octgnGame)
        {
            if (this._ConverterGameDictionary[octgnGame] == null)
            {
                this._ConverterGameDictionary[octgnGame] = new ConverterGame(octgnGame);
            }

            return this._ConverterGameDictionary[octgnGame];
        }

        /// <summary>
        /// The database of all OCTGN card Name, Guid, and Set info needs to be read in.
        /// It only needs to be done once, which is why this is a Singleton.  
        /// When Initialize is called, the database is read from the Controller on a worker thread so 
        /// the user can immediately begin entering their deck info.
        /// </summary>
        /// <param name="octgnGames">All of the possible OCTGN Games to be used to build cards from.</param>
        public void LoadGames(IEnumerable<Octgn.DataNew.Entities.Game> octgnGames)
        {
            if (this._Loaded)
            {
                return;
            }

            if (octgnGames == null)
            {
                throw new ArgumentNullException();
            }

            Logger.Info("Loading all OCTGN games...");
            this._Loaded = true;

            foreach (Octgn.DataNew.Entities.Game octgnGame in octgnGames)
            {
                this._ConverterGameDictionary.Add(octgnGame, null);
            }

            Logger.Info("Loading all OCTGN games complete.");
        }

        /// <summary>
        /// Updates the SetsExcludedFromSearches collection to reflect the latest choices for excluded sets made by the user
        /// </summary>
        public void UpdateSetsExcludedFromSearches()
        {
            foreach (ConverterGame converterGame in this._ConverterGameDictionary.Values)
            {
                if (converterGame != null)
                {
                    converterGame.UpdateSetsExcludedFromSearches();
                }
            }
        }

        #endregion Public Methods
    }
}
