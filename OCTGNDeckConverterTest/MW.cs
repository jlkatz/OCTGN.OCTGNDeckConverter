using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class MW
    {
        public const string SBB_MW_NormalDeck_txt = "[SBB][MW]NormalDeck.txt";

        private static Octgn.DataNew.Entities.Game mwGame;

        static MW()
        {
            ExpectedCardCounts = new Dictionary<string, IEnumerable<ExpectedDeckSection>>();

            ExpectedCardCounts.Add(SBB_MW_NormalDeck_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Mage") { TotalCardCount = 2, UniqueCardCount = 2 },
                    new ExpectedDeckSection("Attack") { TotalCardCount = 9, UniqueCardCount = 6 },
                    new ExpectedDeckSection("Conjuration") { TotalCardCount = 7, UniqueCardCount = 5 },
                    new ExpectedDeckSection("Creature") { TotalCardCount = 1, UniqueCardCount = 1 },
                    new ExpectedDeckSection("Enchantment") { TotalCardCount = 19, UniqueCardCount = 12 },
                    new ExpectedDeckSection("Equipment") { TotalCardCount = 17, UniqueCardCount = 12 },
                    new ExpectedDeckSection("Incantation") { TotalCardCount = 2, UniqueCardCount = 1 },
                });
        }

        /// <summary>
        /// Gets a Dictionary of the number of cards expected in each section of the deck
        /// </summary>
        public static Dictionary<string, IEnumerable<ExpectedDeckSection>> ExpectedCardCounts
        {
            get;
            private set;
        }

        [TestMethod]
        public void ImportSBBFiles()
        {
            List<string> sbbDeckFileNames = new List<string>()
            {
                MW.SBB_MW_NormalDeck_txt,
            };

            this.VerifyMTGDeckFiles(sbbDeckFileNames);
        }

        /// <summary>
        /// Parses and verifies the card count of each of the deck filenames.  These filenames must match
        /// those found in /DeckFiles/
        /// </summary>
        /// <param name="deckFileNames">Collection of deck filenames to verify</param>
        private void VerifyMTGDeckFiles(IEnumerable<string> deckFileNames)
        {
            foreach (string deckFileName in deckFileNames)
            {
                DeckFileResourceHelpers.VerifyDeckFile(
                    deckFileName, 
                    MW.ExpectedCardCounts[deckFileName],
                    this.TestContext.DeploymentDirectory, 
                    MW.mwGame);
            }
        }

        /// <summary>
        /// This is run before each Test, and is necessary for threading functions in OCTGNDeckConverter
        /// </summary>
        [TestInitialize()]
        public void TestSetUp()
        {
            System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Threading.SynchronizationContext());
        }

        /// <summary>
        /// This is run before the set of Tests, and is necessary because it load the OCTGN MW game
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            DeckBuilderPluginController.SimpleDeckBuilderPluginController sdbpc = new DeckBuilderPluginController.SimpleDeckBuilderPluginController();
            OCTGNDeckConverter.Model.ConverterDatabase.SingletonInstance.LoadGames(sdbpc.Games.Games);

            MW.mwGame = OCTGNDeckConverter.Model.ConverterDatabase.SingletonInstance.OctgnGames.FirstOrDefault(
                g => g.Id == OCTGNDeckConverter.Model.ConvertEngine.Game.MW.GameGuidStatic);

            if (MW.mwGame == null)
            {
                throw new Exception("The OCTGN Game MW is not installed, so unit tests cannot be run.");
            }
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }
    }
}
