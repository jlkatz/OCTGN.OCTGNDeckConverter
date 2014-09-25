using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class TappedOut_net : MTGBase
    {
        [TestMethod]
        public void ImportFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://tappedout.net/mtg-decks/dimir-killing-mill/",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 18 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 15, UniqueCardCount = 6 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                },
                MTGBase.mtgGame);

            DeckFileResourceHelpers.VerifyURL(
                "http://tappedout.net/mtg-decks/rise-of-the-undead-shrubbery/",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 100, UniqueCardCount = 87 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                },
                MTGBase.mtgGame);
        }
    }
}
