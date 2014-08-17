using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public abstract class MTGBase
    {
        protected static Octgn.DataNew.Entities.Game mtgGame;

        /// <summary>
        /// This is run before the set of Tests, and is necessary because it loads the OCTGN MTG game.
        /// (This cannot be done via [ClassInitialize] attribute because it won't call it if it is on the base class)
        /// </summary>
        static MTGBase()
        {
            DeckBuilderPluginController.SimpleDeckBuilderPluginController sdbpc = new DeckBuilderPluginController.SimpleDeckBuilderPluginController();
            OCTGNDeckConverter.Model.ConverterDatabase.SingletonInstance.LoadGames(sdbpc.Games.Games);

            MTGBase.mtgGame = OCTGNDeckConverter.Model.ConverterDatabase.SingletonInstance.OctgnGames.FirstOrDefault(
                g => g.Id == OCTGNDeckConverter.Model.ConvertEngine.Game.MTG.GameGuidStatic);

            if (MTGBase.mtgGame == null)
            {
                throw new Exception("The OCTGN Game MTG is not installed, so unit tests cannot be run.");
            }
        }

        /// <summary>
        /// This is run before each Test, and is necessary for threading functions in OCTGNDeckConverter
        /// </summary>
        [TestInitialize]
        public void TestSetUp()
        {
            System.Threading.SynchronizationContext.SetSynchronizationContext(new System.Threading.SynchronizationContext());
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }
    }
}
