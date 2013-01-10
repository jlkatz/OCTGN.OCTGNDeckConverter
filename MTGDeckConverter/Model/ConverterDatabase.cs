namespace MTGDeckConverter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.ComponentModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ConverterDatabase : INotifyPropertyChanged
    {
        private static AggregateException _BuildCardDatabaseExceptions;
        private static Task _BuildCardDatabaseTask;

        /// <summary>
        /// The database of all OCTGN card Name, Guid, Set, and MultiverseID info needs to be read in.
        /// It only needs to be done once, which is why this is a Singleton.  
        /// When this is instantiated, the database is read on a worker thread so the user can
        /// immediately begin entering their deck info.
        /// </summary>
        private ConverterDatabase()
        {
            this.GameDefinition = ConverterDatabase.GetGameDefinition();

            if (this.GameDefinition != null)
            {
                _BuildCardDatabaseTask = Task.Factory.StartNew(() =>
                {
                    this.Sets = ConverterDatabase.BuildCardDatabase(this.GameDefinition);
//System.Threading.Thread.Sleep(999999);  //Pretend OCTGN takes forever getting all cards
                    this.IsInitialized = true;
                });

                //Continue with this if building the database threw an unexpected exception
                _BuildCardDatabaseTask.ContinueWith((t) =>
                {
                    _BuildCardDatabaseExceptions = t.Exception;
                }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        #region Singleton

        private static ConverterDatabase _SingletonInstance;
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

        public Octgn.Data.Game GameDefinition
        {
            get;
            private set;
        }

        private const string IsInitializedPropertyName = "IsInitialized";
        private bool _IsInitialized = false;
        public bool IsInitialized
        {
            get { return _IsInitialized; }
            set { SetValue(ref _IsInitialized, value, IsInitializedPropertyName); }
        }

        public Dictionary<Guid, ConverterSet> Sets
        {
            get;
            private set;
        }
        
        #endregion Public Properties

        #region Public Methods

        public IEnumerable<Guid> GetSetsExcludedFromSearches()
        {
            return
                from s in this.Sets
                where !s.Value.IncludeInSearches
                select s.Key;
        }

        /// <summary>
        /// Returns True if ConverterDatabase is already initialized, or it finished initializing before timing out.  
        /// Returns False if timed out without completing initialization.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForInitializationToComplete(TimeSpan timeout)
        {
            if (!this.IsInitialized && _BuildCardDatabaseTask.Status == TaskStatus.Running)
            {
                //If still building the card database, wait up to timeout
                _BuildCardDatabaseTask.Wait(timeout);

                //If still building the card database after timeout, give up
                return _BuildCardDatabaseTask.Status != TaskStatus.Running;
            }
            else
            {
                return true;
            }
        }

        #endregion Public Methods

        #region Static Helpers

        private static Octgn.Data.Game GetGameDefinition()
        {
            Octgn.Data.GamesRepository repo = new Octgn.Data.GamesRepository();
            return repo.Games.FirstOrDefault(g => g.Id == Guid.Parse("A6C8D2E8-7CD8-11DD-8F94-E62B56D89593"));
        }

        private static Dictionary<Guid, ConverterSet> BuildCardDatabase(Octgn.Data.Game gameDefinition)
        {
            if (gameDefinition == null)
            { throw new ArgumentNullException(); }

            Dictionary<Guid, ConverterSet> sets = new Dictionary<Guid, ConverterSet>();

            var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (Octgn.Data.Set octgnSet in gameDefinition.Sets)
            {
                sets[octgnSet.Id] = new ConverterSet(octgnSet);
            }

            System.Data.DataTable allcards = gameDefinition.SelectCards(new string[] { "[Name] LIKE '%%'" });
            System.Data.DataColumn cardIDColumn = allcards.Columns["id"];
            System.Data.DataColumn setIDColumn = allcards.Columns["set_id"];
            System.Data.DataColumn nameColumn = allcards.Columns["name"];
            System.Data.DataColumn multiverseIDColumn = allcards.Columns["MultiverseId"];
            foreach (System.Data.DataRow row in allcards.Rows)
            {
                Guid setGuid = Guid.Parse(row[setIDColumn].ToString());
                Guid cardGuid = Guid.Parse(row[cardIDColumn].ToString());
                string name = row[nameColumn].ToString().Trim();
                int multiverseID = 0;
                int.TryParse(row[multiverseIDColumn].ToString(), out multiverseID);

                sets[setGuid].AddNewConverterCard
                (
                    cardGuid,
                    name,
                    multiverseID
                );
            }

            foreach(KeyValuePair<Guid, ConverterSet> kvp in sets)
            {
                kvp.Value.SortConverterCards();

                if (SettingsManager.SingletonInstance.SetsExcludedFromSearches.Contains(kvp.Key))
                { kvp.Value.IncludeInSearches = false; }
            }

            sw.Stop();
            Console.WriteLine("Building Card Database took " + sw.Elapsed);

            return sets;
        }

        #endregion Static Helpers

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //http://www.pochet.net/blog/2010/06/25/inotifypropertychanged-implementations-an-overview/
        protected bool SetValue<T>(ref T property, T value, string propertyName)
        {
            if (Object.Equals(property, value))
            {
                return false;
            }
            property = value;

            this.OnPropertyChanged(propertyName);

            return true;
        }

        #endregion

        public void Cleanup()
        {
            //Update the list of excluded sets before exiting
            SettingsManager.SingletonInstance.SetsExcludedFromSearches.Clear();
            SettingsManager.SingletonInstance.SetsExcludedFromSearches.AddRange(this.GetSetsExcludedFromSearches());
        }
    }
}
