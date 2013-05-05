// -----------------------------------------------------------------------
// <copyright file="ConverterDatabase.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octgn.Core.DataExtensionMethods;

namespace MTGDeckConverter.Model
{
    /// <summary>
    /// Singleton object which contains the Octgn.Data.Game definition for MTG, a Dictionary of all the Sets
    /// available with the corresponding ConverterSet.  When first instantiated, it asynchronously fetches
    /// all the sets via the Octgn API.  This will take a few seconds, and the IsInitialized property of this
    /// object will be set to true when it is ready.
    /// </summary>
    public class ConverterDatabase : INotifyPropertyChangedBase
    {
        /// <summary>
        /// When first instantiated, an asynchronous Task is executed which accesses the OCTGN database
        /// and builds up the Dictionary of ConverterSets.  This references that Task.
        /// </summary>
        private Task _BuildCardDatabaseTask;

        /// <summary>
        /// Prevents a default instance of the <see cref="ConverterDatabase"/> class from being created.
        /// </summary>
        private ConverterDatabase()
        {
        }

        /// <summary>
        /// The database of all OCTGN card Name, Guid, Set, and MultiverseID info needs to be read in.
        /// It only needs to be done once, which is why this is a Singleton.  
        /// When Initialize is called, the database is read from the Controller on a worker thread so 
        /// the user can immediately begin entering their deck info.
        /// </summary>
        /// <param name="mtgGame">The OCTGN Game to be used to build cards from.  It must be MTG.</param>
        public void Initialize(Octgn.DataNew.Entities.Game mtgGame)
        {
            if (mtgGame == null)
            {
                throw new ArgumentNullException();
            }

            this.GameDefinition = mtgGame;

            if (this.GameDefinition != null)
            {
                this._BuildCardDatabaseTask = new Task(() =>
                {
                    this.Sets = ConverterDatabase.BuildCardDatabase(this.GameDefinition);
                    this.IsInitialized = true;
                });

                // Continue with this if building the database threw an unexpected exception
                this._BuildCardDatabaseTask.ContinueWith
                (
                    (t) =>
                    {
                        this.BuildCardDatabaseExceptions = t.Exception;
                    },
                    System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted
                );

                this._BuildCardDatabaseTask.Start();
            }
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
        /// Gets the OCTGN GameDefinition for MTG
        /// </summary>
        public Octgn.DataNew.Entities.Game GameDefinition
        {
            get;
            private set;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string BuildCardDatabaseExceptionsPropertyName = "BuildCardDatabaseExceptions";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private AggregateException _BuildCardDatabaseExceptions;
        
        /// <summary>
        /// Gets or sets the exceptions that were caught while building the OCTGN database
        /// </summary>
        public AggregateException BuildCardDatabaseExceptions
        {
            get { return this._BuildCardDatabaseExceptions; }
            set { this.SetValue(ref this._BuildCardDatabaseExceptions, value, BuildCardDatabaseExceptionsPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        private const string IsInitializedPropertyName = "IsInitialized";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
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
        
        #endregion Public Properties

        #region Public Methods

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
        /// Updates the SetsExcludedFromSearches collection to reflect the latest choices for excluded sets made by the user
        /// </summary>
        public void UpdateSetsExcludedFromSearches()
        {
            SettingsManager.SingletonInstance.SetsExcludedFromSearches.Clear();
            SettingsManager.SingletonInstance.SetsExcludedFromSearches.AddRange(this.GetSetsExcludedFromSearches());
        }

        /// <summary>
        /// Returns True if ConverterDatabase is already initialized, or it finished initializing before timing out.  
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

        #endregion Public Methods

        #region Static Helpers

        /// <summary>
        /// Returns a Dictionary who's keys are Guids which represent Octgn Sets, and who's values are corresponding
        /// ConverterSet objects.  
        /// </summary>
        /// <param name="gameDefinition">The MTG Game Definition object to use to read sets from</param>
        /// <returns>A Dictionary of Octgn Set Guids and corresponding ConverterSet objects</returns>
        private static Dictionary<Guid, ConverterSet> BuildCardDatabase(Octgn.DataNew.Entities.Game gameDefinition)
        {
            if (gameDefinition == null)
            {
                throw new ArgumentNullException();
            }

            Octgn.DataNew.Entities.PropertyDef multiverseIdPropertyDef = 
                gameDefinition.CustomProperties.First(p => p.Name.Equals("MultiVerseId", StringComparison.InvariantCultureIgnoreCase));

            Dictionary<Guid, ConverterSet> sets = new Dictionary<Guid, ConverterSet>();

            foreach (Octgn.DataNew.Entities.Set octgnSet in gameDefinition.Sets())
            {
                sets[octgnSet.Id] = new ConverterSet(octgnSet);
                foreach (Octgn.DataNew.Entities.Card card in octgnSet.Cards)
                {
                    // Try to dig the MultiverseID property out of the Octgn.DataNew.Entities.Card
                    // During testing, all properties seemed nested under the first KeyValuePair in card.Properties
                    int multiverseID = 0;
                    if (card.Properties.Count > 0)
                    {
                        KeyValuePair<string, Octgn.DataNew.Entities.CardPropertySet> firstCardPropertyKVP = card.Properties.First();
                        object multiverseIdString = null;
                        if (firstCardPropertyKVP.Value.Properties.TryGetValue(multiverseIdPropertyDef, out multiverseIdString))
                        {
                            int.TryParse(multiverseIdString.ToString(), out multiverseID);
                        }
                    }

                    sets[octgnSet.Id].AddNewConverterCard
                    (
                        card.Id,
                        card.Name,
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

        /// <summary>
        /// Performs necessary actions before this object is terminated; typically when the program exits.
        /// </summary>
        public void Cleanup()
        {
            // Update the list of excluded sets before exiting
            this.UpdateSetsExcludedFromSearches();
        }
    }
}
