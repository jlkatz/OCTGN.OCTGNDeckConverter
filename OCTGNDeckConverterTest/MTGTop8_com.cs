using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class MTGTop8_com : MTGBase
    {
        [TestMethod]
        public void ImportFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://mtgtop8.com/event?e=7937&d=245652&f=MO",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 27 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 15, UniqueCardCount = 10 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                },
                MTGBase.mtgGame);
        }
    }
}