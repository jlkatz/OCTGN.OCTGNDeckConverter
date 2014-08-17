using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class ArcaneWonders_com : MWBase
    {
        [TestMethod]
        public void ImportFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://forum.arcanewonders.com/index.php?topic=14509.0",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Mage") { TotalCardCount = 2, UniqueCardCount = 2 },
                    new ExpectedDeckSection("Attack") { TotalCardCount = 3, UniqueCardCount = 2 },
                    new ExpectedDeckSection("Conjuration") { TotalCardCount = 6, UniqueCardCount = 5 },
                    new ExpectedDeckSection("Creature") { TotalCardCount = 1, UniqueCardCount = 1 },
                    new ExpectedDeckSection("Enchantment") { TotalCardCount = 41, UniqueCardCount = 25 },
                    new ExpectedDeckSection("Equipment") { TotalCardCount = 4, UniqueCardCount = 4 },
                    new ExpectedDeckSection("Incantation") { TotalCardCount = 8, UniqueCardCount = 4 },
                },
                MWBase.mwGame);
        }
    }
}
