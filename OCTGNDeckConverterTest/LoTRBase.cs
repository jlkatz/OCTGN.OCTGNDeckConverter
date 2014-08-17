using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public abstract class LoTRBase
    {
        protected static Octgn.DataNew.Entities.Game lotrGame;

        /// <summary>
        /// This is run before the set of Tests, and is necessary because it loads the OCTGN LoTR game.
        /// (This cannot be done via [ClassInitialize] attribute because it won't call it if it is on the base class)
        /// </summary>
        static LoTRBase()
        {
            DeckBuilderPluginController.SimpleDeckBuilderPluginController sdbpc = new DeckBuilderPluginController.SimpleDeckBuilderPluginController();
            OCTGNDeckConverter.Model.ConverterDatabase.SingletonInstance.LoadGames(sdbpc.Games.Games);

            LoTRBase.lotrGame = OCTGNDeckConverter.Model.ConverterDatabase.SingletonInstance.OctgnGames.FirstOrDefault(
                g => g.Id == OCTGNDeckConverter.Model.ConvertEngine.Game.LoTR.GameGuidStatic);

            if (LoTRBase.lotrGame == null)
            {
                throw new Exception("The OCTGN Game LoTR is not installed, so unit tests cannot be run.");
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
