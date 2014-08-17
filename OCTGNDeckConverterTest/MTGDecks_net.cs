using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class MTGDecks_net : MTGBase
    {
        [TestMethod]
        public void ImportFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://www.mtgdecks.net/decks/view/38600",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 19 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 15, UniqueCardCount = 7 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                },
                MTGBase.mtgGame);
        }
    }
}