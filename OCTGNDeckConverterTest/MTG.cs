using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCTGNDeckConverter.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    [TestClass]
    public class MTG : MTGBase
    {
        public const string Apprentice_MTG_CardWithAEName_dec = "[Apprentice][MTG]CardWithAEName.dec";
        public const string Apprentice_MTG_DeckInMultipleFormats1_dec = "[Apprentice][MTG]DeckInMultipleFormats1.dec";
        public const string Apprentice_MTG_DeckInMultipleFormats2_dec = "[Apprentice][MTG]DeckInMultipleFormats2.dec";
        public const string Apprentice_MTG_NormalDeck_dec = "[Apprentice][MTG]NormalDeck.dec";
        public const string Cockatrice_MTG_NormalDeck_cod = "[Cockatrice][MTG]NormalDeck.cod";
        public const string Generic_MTG_DeckInMultipleFormats1_txt = "[Generic][MTG]DeckInMultipleFormats1.txt";
        public const string Generic_MTG_DeckInMultipleFormats2_txt = "[Generic][MTG]DeckInMultipleFormats2.txt";
        public const string Generic_MTG_DeckInMultipleFormats3_txt = "[Generic][MTG]DeckInMultipleFormats3.txt";
        public const string MTGO_MTG_NormalDeck_txt = "[MTGO][MTG]NormalDeck.txt";
        public const string MWS_MTG_DeckInMultipleFormats3_mwDeck = "[MWS][MTG]DeckInMultipleFormats3.mwDeck";
        public const string OCTGN2_MTG_DeckInMultipleFormats1_txt = "[OCTGN2][MTG]DeckInMultipleFormats1.o8d";
        public const string OCTGN2_MTG_DeckInMultipleFormats2_txt = "[OCTGN2][MTG]DeckInMultipleFormats2.o8d";
        public const string OCTGN2_MTG_NormalDeck_txt = "[OCTGN2][MTG]NormalDeck.o8d";

        static MTG()
        {
            ExpectedCardCounts = new Dictionary<string, IEnumerable<ExpectedDeckSection>>();

            ExpectedCardCounts.Add(Apprentice_MTG_CardWithAEName_dec,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 12 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 8, UniqueCardCount = 2 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(Apprentice_MTG_DeckInMultipleFormats1_dec,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 12 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(Apprentice_MTG_DeckInMultipleFormats2_dec,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 100, UniqueCardCount = 90 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });
            
            ExpectedCardCounts.Add(Apprentice_MTG_NormalDeck_dec,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 42, UniqueCardCount = 32 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 3, UniqueCardCount = 3 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(Cockatrice_MTG_NormalDeck_cod,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 40, UniqueCardCount = 18 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 4, UniqueCardCount = 4 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(Generic_MTG_DeckInMultipleFormats1_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 12 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(Generic_MTG_DeckInMultipleFormats2_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 100, UniqueCardCount = 90 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });
            
            ExpectedCardCounts.Add(Generic_MTG_DeckInMultipleFormats3_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 16 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 15, UniqueCardCount = 7 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(MTGO_MTG_NormalDeck_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 42, UniqueCardCount = 32 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 3, UniqueCardCount = 3 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });


            ExpectedCardCounts.Add(MWS_MTG_DeckInMultipleFormats3_mwDeck,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 16 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 15, UniqueCardCount = 7 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(OCTGN2_MTG_DeckInMultipleFormats1_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 60, UniqueCardCount = 12 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });
            
            ExpectedCardCounts.Add(OCTGN2_MTG_DeckInMultipleFormats2_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 100, UniqueCardCount = 90 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
                });

            ExpectedCardCounts.Add(OCTGN2_MTG_NormalDeck_txt,
                new List<ExpectedDeckSection>()
                {
                    new ExpectedDeckSection("Main") { TotalCardCount = 57, UniqueCardCount = 20 },
                    new ExpectedDeckSection("Sideboard") { TotalCardCount = 3, UniqueCardCount = 2 },
                    new ExpectedDeckSection("Command Zone") { TotalCardCount = 0, UniqueCardCount = 0 },
                    new ExpectedDeckSection("Planes/Schemes") { TotalCardCount = 0, UniqueCardCount = 0 },
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
        public void ImportApprenticeFiles()
        {
            List<string> apprenticeDeckFileNames = new List<string>()
            {
                MTG.Apprentice_MTG_NormalDeck_dec,
                MTG.Apprentice_MTG_CardWithAEName_dec,
                MTG.Apprentice_MTG_DeckInMultipleFormats1_dec,
                MTG.Apprentice_MTG_DeckInMultipleFormats2_dec,
            };

            this.VerifyMTGDeckFiles(apprenticeDeckFileNames);
        }

        [TestMethod]
        public void ImportCockatriceFiles()
        {
            List<string> cockatriceDeckFileNames = new List<string>()
            {
                MTG.Cockatrice_MTG_NormalDeck_cod,
            };

            this.VerifyMTGDeckFiles(cockatriceDeckFileNames);
        }

        [TestMethod]
        public void ImportGenericFiles()
        {
            List<string> genericDeckFileNames = new List<string>()
            {
                MTG.Generic_MTG_DeckInMultipleFormats1_txt,
                MTG.Generic_MTG_DeckInMultipleFormats2_txt,
                MTG.Generic_MTG_DeckInMultipleFormats3_txt,
            };

            this.VerifyMTGDeckFiles(genericDeckFileNames);
        }

        [TestMethod]
        public void ImportMTGOFiles()
        {
            List<string> mtgoDeckFileNames = new List<string>()
            {
                MTG.MTGO_MTG_NormalDeck_txt,
            };

            this.VerifyMTGDeckFiles(mtgoDeckFileNames);
        }

        [TestMethod]
        public void ImportMWSFiles()
        {
            List<string> mwsDeckFileNames = new List<string>()
            {
                MTG.MWS_MTG_DeckInMultipleFormats3_mwDeck,
            };

            this.VerifyMTGDeckFiles(mwsDeckFileNames);
        }

        [TestMethod]
        public void ImportOCTGN2Files()
        {
            List<string> octgn2DeckFileNames = new List<string>()
            {
                MTG.OCTGN2_MTG_NormalDeck_txt,
                MTG.OCTGN2_MTG_DeckInMultipleFormats1_txt,
                MTG.OCTGN2_MTG_DeckInMultipleFormats2_txt,
            };

            this.VerifyMTGDeckFiles(octgn2DeckFileNames);
        }

        [TestMethod]
        public void DecksSavedInDifferentFormatsAreIdentical1()
        {
            this.CompareIdenticalDecksInDifferentFormats(new List<string>()
            {
                MTG.Apprentice_MTG_DeckInMultipleFormats1_dec,
                MTG.Generic_MTG_DeckInMultipleFormats1_txt,
                MTG.OCTGN2_MTG_DeckInMultipleFormats1_txt,
            });
        }

        [TestMethod]
        public void DecksSavedInDifferentFormatsAreIdentical2()
        {
            this.CompareIdenticalDecksInDifferentFormats(new List<string>()
            {
                MTG.Apprentice_MTG_DeckInMultipleFormats2_dec,
                MTG.Generic_MTG_DeckInMultipleFormats2_txt,
                MTG.OCTGN2_MTG_DeckInMultipleFormats2_txt,
            });
        }

        [TestMethod]
        public void DecksSavedInDifferentFormatsAreIdentical3()
        {
            this.CompareIdenticalDecksInDifferentFormats(new List<string>()
            {
                MTG.Generic_MTG_DeckInMultipleFormats3_txt,
                MTG.MWS_MTG_DeckInMultipleFormats3_mwDeck,
            });
        }

        private void CompareIdenticalDecksInDifferentFormats(IEnumerable<string> deckNameGroup)
        {
            List<OCTGNDeckConverter.Model.ConverterDeck> converterDeckGroup = new List<OCTGNDeckConverter.Model.ConverterDeck>();

            foreach (string deckName in deckNameGroup)
            {
                DeckFileResourceHelpers.CopyDeckFileResourceToDirectory(this.TestContext.DeploymentDirectory, deckName);
                converterDeckGroup.Add(DeckFileResourceHelpers.ConvertDeckFileUsingWizard(
                    Path.Combine(this.TestContext.DeploymentDirectory, deckName), MTG.mtgGame));
            }

            DeckFileResourceHelpers.VerifyAllConverterDecksAreIdentical(converterDeckGroup);
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
                    MTG.ExpectedCardCounts[deckFileName],
                    this.TestContext.DeploymentDirectory,
                    MTG.mtgGame);
            }
        }
    }
}
