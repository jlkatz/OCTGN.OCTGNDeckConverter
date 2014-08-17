using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class CardGameDB_com : LoTRBase
    {
        [TestMethod]
        public void ImportOfficiallySubmittedDeckFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://www.cardgamedb.com/index.php/thelordoftherings/the-lord-of-the-rings-decks/_/lord-of-the-rings-submitted-decks/mono-spirit-r25",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Hero") { TotalCardCount = 3, UniqueCardCount = 3 },
                    new ExpectedDeckSection("Ally") { TotalCardCount = 24, UniqueCardCount = 11 },
                    new ExpectedDeckSection("Attachment") { TotalCardCount = 9, UniqueCardCount = 5 },
                    new ExpectedDeckSection("Event") { TotalCardCount = 17, UniqueCardCount = 9 },
                },
                LoTRBase.lotrGame);
        }

        [TestMethod]
        public void ImportPersonalDeckFromWebsite()
        {
            DeckFileResourceHelpers.VerifyURL(
                "http://www.cardgamedb.com/index.php/thelordoftherings/the-lord-of-the-rings-deck-share?p=4226&deck=lotrdeck_53bf7c75f0309",
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Hero") { TotalCardCount = 3, UniqueCardCount = 3 },
                    new ExpectedDeckSection("Ally") { TotalCardCount = 8, UniqueCardCount = 3 },
                    new ExpectedDeckSection("Attachment") { TotalCardCount = 21, UniqueCardCount = 8 },
                    new ExpectedDeckSection("Event") { TotalCardCount = 21, UniqueCardCount = 7 },
                },
                LoTRBase.lotrGame);
        }
    }
}
