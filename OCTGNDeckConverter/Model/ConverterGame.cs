// -----------------------------------------------------------------------
// <copyright file="ConverterGame.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octgn.Core.DataExtensionMethods;

namespace OCTGNDeckConverter.Model
{
    /// <summary>
    /// Contains the Octgn.Data.Game definition, and a Dictionary of all the Sets
    /// available with the corresponding ConverterSet.  When first instantiated, it asynchronously fetches
    /// all the sets via the Octgn API.  This will take a few seconds, and the IsInitialized property of this
    /// object will be set to true when it is ready.
    /// </summary>
    public class ConverterGame : INotifyPropertyChangedBase
    {
        /// <summary>
        /// The logger instance for this class.
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// When first instantiated, an asynchronous Task is executed which accesses the OCTGN database
        /// and builds up the Dictionary of ConverterSets.  This references that Task.
        /// </summary>
        private Task _BuildCardDatabaseTask;

        /// <summary>
        /// Initializes a new instance of the ConverterGame class.
        /// </summary>
        /// <param name="game">The OCTGN game definition to use for conversion</param>
        public ConverterGame(Octgn.DataNew.Entities.Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            this.Game = game;
            this.Initialize();
        }

        /// <summary>
        /// Gets the OCTGN Game
        /// </summary>
        public Octgn.DataNew.Entities.Game Game
        {
            get;
            private set;
        }

        /// <summary>
        /// Property name constant for the BuildCardDatabaseExceptions property.
        /// </summary>
        private const string BuildCardDatabaseExceptionsPropertyName = "BuildCardDatabaseExceptions";

        /// <summary>
        /// Private backing field for the BuildCardDatabaseExceptions property.
        /// </summary>
        private AggregateException _BuildCardDatabaseExceptions;

        /// <summary>
        /// Gets or sets the exceptions that were caught while building the OCTGN database
        /// </summary>
        public AggregateException BuildCardDatabaseExceptions
        {
            get { return this._BuildCardDatabaseExceptions; }
            set { this.SetValue(ref this._BuildCardDatabaseExceptions, value, BuildCardDatabaseExceptionsPropertyName); }
        }

        /// <summary>
        /// Gets a collection of all the Section names for the OCTGN Game
        /// </summary>
        public IEnumerable<string> DeckSectionNames
        {
            get { return this.Game.DeckSections.Keys; }
        }

        /// <summary>
        /// Property name constant for the IsInitialized property.
        /// </summary>
        private const string IsInitializedPropertyName = "IsInitialized";

        /// <summary>
        /// Private backing field for the IsInitialized property.
        /// </summary>
        private bool _IsInitialized = false;

        /// <summary>
        /// Gets a value indicating whether the ConverterDatabase Singleton instance has finished initializing or not.
        /// </summary>
        public bool IsInitialized
        {
            get { return this._IsInitialized; }
            private set { this.SetValue(ref this._IsInitialized, value, IsInitializedPropertyName); }
        }

        /// <summary>
        /// Gets the Dictionary of all Set Guids (as defined by the OCTGN MTG team) and corresponding ConverterSet object.
        /// </summary>
        public Dictionary<Guid, ConverterSet> Sets
        {
            get;
            private set;
        }

        #region Methods

        /// <summary>
        /// Performs necessary actions before this object is terminated; typically when the program exits.
        /// </summary>
        public void Cleanup()
        {
            // Update the list of excluded sets before exiting
            this.UpdateSetsExcludedFromSearches();
        }

        /// <summary>
        /// Returns a list of Guids which are the IDs of the Octgn Sets that are to be EXCLUDED from card searches
        /// </summary>
        /// <returns>A collection of Guids which represent Octgn Sets to be excluded.</returns>
        public IEnumerable<Guid> GetSetsExcludedFromSearches()
        {
            if (this.IsInitialized)
            {
                return
                    from s in this.Sets
                    where !s.Value.IncludeInSearches
                    select s.Key;
            }
            else
            {
                return new List<Guid>();
            }
        }

        /// <summary>
        /// The database of all OCTGN card Name, Guid, and Set info needs to be read in.
        /// When Initialize is called, the database is read from the Controller on a worker thread so 
        /// the user can immediately begin entering their deck info.
        /// </summary>
        private void Initialize()
        {
            Logger.Info("Initializing the OCTGN Game " + this.Game.Name);

            this._BuildCardDatabaseTask = new Task(() =>
            {
                this.Sets = ConverterGame.BuildCardDatabase(this.Game);
                this.IsInitialized = true;
            });

            // Continue with this if building the database threw an unexpected exception
            this._BuildCardDatabaseTask.ContinueWith
            (
                (t) =>
                {
                    Logger.Error("An exception occurred while building the card database. ", t.Exception);
                    this.BuildCardDatabaseExceptions = t.Exception;
                },
                System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted
            );

            this._BuildCardDatabaseTask.Start();
        }

        /// <summary>
        /// Updates the SetsExcludedFromSearches collection to reflect the latest choices for excluded sets made by the user
        /// </summary>
        public void UpdateSetsExcludedFromSearches()
        {
            SettingsManager.SingletonInstance.SetsExcludedFromSearches.Clear();
            SettingsManager.SingletonInstance.SetsExcludedFromSearches.AddRange(this.GetSetsExcludedFromSearches());
        }

        /// <summary>
        /// Returns True if ConverterGame is already initialized, or it finished initializing before timing out.  
        /// Returns False if timed out without completing initialization.
        /// </summary>
        /// <param name="timeout">The amount of time to allow for initialization before a timeout failure</param>
        /// <returns>True if already initialized or finished initializing before timeout; false if timed out without completing initialization.</returns>
        public bool WaitForInitializationToComplete(TimeSpan timeout)
        {
            if (!this.IsInitialized && this._BuildCardDatabaseTask.Status == TaskStatus.Running)
            {
                // If still building the card database, wait up to timeout
                this._BuildCardDatabaseTask.Wait(timeout);

                // If still building the card database after timeout, give up
                return this._BuildCardDatabaseTask.Status != TaskStatus.Running;
            }
            else
            {
                return true;
            }
        }

        #endregion Methods

        #region Static Helpers

        /// <summary>
        /// Returns a Dictionary who's keys are Guids which represent Octgn Sets, and who's values are corresponding
        /// ConverterSet objects.  
        /// </summary>
        /// <param name="game">The Octgn.DataNew.Entities.Game object to use to read sets from</param>
        /// <returns>A Dictionary of Octgn Set Guids and corresponding ConverterSet objects</returns>
        private static Dictionary<Guid, ConverterSet> BuildCardDatabase(Octgn.DataNew.Entities.Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException();
            }

            Logger.Info("Building the card database for game " + game.Name);

            // MTG has a property "MultiVerseId" which should be grabbed if it exists
            Octgn.DataNew.Entities.PropertyDef multiverseIdPropertyDef =
                game.CustomProperties.FirstOrDefault(p => p.Name.Equals("MultiVerseId", StringComparison.InvariantCultureIgnoreCase));

            Dictionary<Guid, ConverterSet> sets = new Dictionary<Guid, ConverterSet>();

            foreach (Octgn.DataNew.Entities.Set octgnSet in game.Sets())
            {
                Logger.Info("Adding cards from set " + octgnSet.Name);

                sets[octgnSet.Id] = new ConverterSet(octgnSet);
                foreach (Octgn.DataNew.Entities.Card card in octgnSet.Cards)
                {
                    // Try to dig the MultiverseID property out of the Octgn.DataNew.Entities.Card
                    // During testing, all properties seemed nested under the first KeyValuePair in card.Properties
                    int multiverseID = 0;
                    if (multiverseIdPropertyDef != null)
                    {
                        if (card.Properties.Count > 0)
                        {
                            KeyValuePair<string, Octgn.DataNew.Entities.CardPropertySet> firstCardPropertyKVP = card.Properties.First();
                            object multiverseIdString = null;
                            if (firstCardPropertyKVP.Value.Properties.TryGetValue(multiverseIdPropertyDef, out multiverseIdString))
                            {
                                int.TryParse(multiverseIdString.ToString(), out multiverseID);
                            }
                        }
                    }

                    string name = card.Name;

                    // CoC uses special characters at the beginning of the card name to represent properties such as Unique and Steadfast.
                    // These characters should not be included as part of the name for comparison.
                    if (game.Id == ConvertEngine.Game.CoC.GameGuidStatic)
                    {
                        name = name.Trim(new Char[] { '{', '}', '[', ']', '<', '>', '_', '^', '*', ' ' });
                    }

                    sets[octgnSet.Id].AddNewConverterCard
                    (
                        card.Id,
                        name,
                        multiverseID
                    );
                }
            }

            foreach (KeyValuePair<Guid, ConverterSet> kvp in sets)
            {
                kvp.Value.SortConverterCards();

                if (SettingsManager.SingletonInstance.SetsExcludedFromSearches.Contains(kvp.Key))
                {
                    kvp.Value.IncludeInSearches = false;
                }
            }

            return sets;
        }

        #endregion Static Helpers
    }
}
