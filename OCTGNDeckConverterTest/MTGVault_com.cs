using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class MTGVault_com : MTGBase
    {
        [TestMethod]
        public void ImportFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://www.mtgvault.com/az88/decks/standard-azorius-control-m15/",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 20 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 15, UniqueCardCount = 9 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                },
                MTGBase.mtgGame);
        }
    }
}
