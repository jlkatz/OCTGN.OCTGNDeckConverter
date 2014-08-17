using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCTGNDeckConverter.Model;
using OCTGNDeckConverter.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OCTGNDeckConverterTest
{
    public static class DeckFileResourceHelpers
    {
        /// <summary>
        /// Creates a copy of the Resource File with matchin deckName (found in DeckFiles directory).
        /// Returns a value indicating whether the file was created successfully or not.
        /// </summary>
        /// <param name="directory">The directory to create the copy of the file in</param>
        /// <param name="deckName">The filename of the deck, including extension.  This must match one of the files in 'DeckFiles'</param>
        /// <returns>A value indicating whether the file was created successfully or not</returns>
        public static bool CopyDeckFileResourceToDirectory(string directory, string deckName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resource = string.Format("OCTGNDeckConverterTest.DeckFiles.{0}", deckName);
                using (var stream = asm.GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (var fileStream = File.Create(Path.Combine(directory, deckName)))
                        {
                            stream.CopyTo(fileStream);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Parses and verifies the card count of the deck filename.  The filename must match
        /// those found in /DeckFiles/
        /// </summary>
        /// <param name="deckFileName">The filename of the deck to verify</param>
        /// <param name="scratchDirectory">The directory to create the deck file in</param>
        /// <param name="game">The Game that should be chosen in the Wizard</param>
        public static void VerifyDeckFile(
            string deckFileName, 
            IEnumerable<ExpectedDeckSection> expectedSectionsStats,
            string scratchDirectory, 
            Octgn.DataNew.Entities.Game game)
        {
            Assert.IsTrue(DeckFileResourceHelpers.CopyDeckFileResourceToDirectory(
                scratchDirectory,
                deckFileName));

            ConverterDeck converterDeck = ConvertDeckFileUsingWizard(Path.Combine(scratchDirectory, deckFileName), game);

            foreach (ConverterSection converterSection in converterDeck.ConverterSections)
            {
                ExpectedDeckSection expectedSectionStats = 
                    expectedSectionsStats.First(eds => eds.SectionName == converterSection.SectionName);

                Assert.AreEqual(expectedSectionStats.TotalCardCount, converterSection.SectionCount);
                Assert.AreEqual(expectedSectionStats.UniqueCardCount, converterSection.SectionMappings.Count(sm => sm.PotentialOCTGNCards.Count > 0));

                foreach (ConverterMapping converterMapping in converterSection.SectionMappings)
                {
                    // At least one potential match should have been found
                    Assert.IsTrue(converterMapping.PotentialOCTGNCards.Count() > 0);
                }
            }
        }

        /// <summary>
        /// Parses and verifies the card count of the deck URL.
        /// </summary>
        /// <param name="deckURL">The URL of the deck to verify</param>
        /// <param name="scratchDirectory">The directory to create the deck file in</param>
        /// <param name="game">The Game that should be chosen in the Wizard</param>
        public static void VerifyURL(
            string deckURL,
            IEnumerable<ExpectedDeckSection> expectedSectionsStats,
            Octgn.DataNew.Entities.Game game)
        {
            ConverterDeck converterDeck = ConvertURLUsingWizard(deckURL, game);

            foreach (ConverterSection converterSection in converterDeck.ConverterSections)
            {
                ExpectedDeckSection expectedSectionStats =
                    expectedSectionsStats.First(eds => eds.SectionName == converterSection.SectionName);

                Assert.AreEqual(expectedSectionStats.TotalCardCount, converterSection.SectionCount);
                Assert.AreEqual(expectedSectionStats.UniqueCardCount, converterSection.SectionMappings.Count(sm => sm.PotentialOCTGNCards.Count > 0));

                foreach (ConverterMapping converterMapping in converterSection.SectionMappings)
                {
                    // At least one potential match should have been found
                    Assert.IsTrue(converterMapping.PotentialOCTGNCards.Count() > 0);
                }
            }
        }

        /// <summary>
        /// Converts a file into a ConverterDeck using the given game, then returns it
        /// </summary>
        /// <param name="deckFileFullPathName">The full path name of the deck to get</param>
        /// <param name="game">The Game that should be chosen in the Wizard</param>
        /// <returns>a ConverterDeck containing cards parsed from the deck file</returns>
        public static OCTGNDeckConverter.Model.ConverterDeck ConvertDeckFileUsingWizard(
            string deckFileFullPathName,
            Octgn.DataNew.Entities.Game game)
        {
            ImportDeckWizardVM idwvm = new ImportDeckWizardVM();

            // If more than one Game is installed, the ChooseGame page will be active.  Select the game
            if (idwvm.CurrentWizardPageVM is WizardPage_ChooseGame)
            {
                (idwvm.CurrentWizardPageVM as WizardPage_ChooseGame).ChooseGameCommand.Execute(game);
            }

            Assert.IsTrue(idwvm.CurrentWizardPageVM is WizardPage_ChooseDeckSourceType);
            (idwvm.CurrentWizardPageVM as WizardPage_ChooseDeckSourceType).ChooseDeckSourceTypeCommand.Execute(DeckSourceTypes.File);

            Assert.IsTrue(idwvm.CurrentWizardPageVM is WizardPage_SelectFile);
            idwvm.Converter.DeckFullPathName = deckFileFullPathName;
            idwvm.MoveToNextStep();

            // It might take some time for the conversion, (or for the database to load) so wait for it
            DateTime waitForConversionUntil = DateTime.Now.Add(TimeSpan.FromSeconds(300));
            while (!(idwvm.CurrentWizardPageVM is WizardPage_CompareCards) && DateTime.Now < waitForConversionUntil)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            Assert.IsTrue(idwvm.CurrentWizardPageVM is WizardPage_CompareCards);

            return idwvm.Converter.ConverterDeck;
        }

        /// <summary>
        /// Converts a URL into a ConverterDeck, then returns it
        /// </summary>
        /// <param name="deckURL">The URL of the deck to get</param>
        /// <param name="game">The Game that should be chosen in the Wizard</param>
        /// <returns>a ConverterDeck containing cards parsed from the deck file</returns>
        public static OCTGNDeckConverter.Model.ConverterDeck ConvertURLUsingWizard(
            string deckURL,
            Octgn.DataNew.Entities.Game game)
        {
            ImportDeckWizardVM idwvm = new ImportDeckWizardVM();

            // If more than one Game is installed, the ChooseGame page will be active.  Select the game
            if (idwvm.CurrentWizardPageVM is WizardPage_ChooseGame)
            {
                (idwvm.CurrentWizardPageVM as WizardPage_ChooseGame).ChooseGameCommand.Execute(game);
            }

            Assert.IsTrue(idwvm.CurrentWizardPageVM is WizardPage_ChooseDeckSourceType);
            (idwvm.CurrentWizardPageVM as WizardPage_ChooseDeckSourceType).ChooseDeckSourceTypeCommand.Execute(DeckSourceTypes.Webpage);

            Assert.IsTrue(idwvm.CurrentWizardPageVM is WizardPage_EnterWebpage);
            idwvm.Converter.DeckURL = deckURL;
            idwvm.MoveToNextStep();

            // It might take some time for the conversion, (or for the database to load) so wait for it
            DateTime waitForConversionUntil = DateTime.Now.Add(TimeSpan.FromSeconds(300));
            while (!(idwvm.CurrentWizardPageVM is WizardPage_CompareCards) && DateTime.Now < waitForConversionUntil)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            Assert.IsTrue(idwvm.CurrentWizardPageVM is WizardPage_CompareCards);

            return idwvm.Converter.ConverterDeck;
        }

        public static void VerifyAllConverterDecksAreIdentical(IEnumerable<ConverterDeck> converterDecks)
        {
            if (converterDecks.Count() < 2)
            {
                throw new ArgumentException("Cannot verify all converter decks are identical if there aren't at least 2.");
            }
            
            for (int cd = 1; cd < converterDecks.Count(); cd++)
            {
                VerifyConverterDecksAreEqual(converterDecks.ElementAt(0), converterDecks.ElementAt(cd));
            }
        }

        private static void VerifyConverterDecksAreEqual(ConverterDeck converterDeck, ConverterDeck otherConverterDeck)
        {
            for (int cs = 0; cs < converterDeck.ConverterSections.Count; cs++)
            {
                VerifyConverterSectionsAreIdentical(converterDeck.ConverterSections[cs], otherConverterDeck.ConverterSections[cs]);
            }
        }

        private static void VerifyConverterSectionsAreIdentical(ConverterSection converterSection, ConverterSection otherConverterSection)
        {
            Assert.AreEqual(converterSection.SectionCount, otherConverterSection.SectionCount);
            Assert.AreEqual(converterSection.SectionMappings.Count, otherConverterSection.SectionMappings.Count);

            foreach (ConverterMapping converterMapping in converterSection.SectionMappings)
            {
                ConverterMapping otherConverterMapping = otherConverterSection.SectionMappings.First(sm => sm.CardName == converterMapping.CardName);
                VerifyConverterMappingsAreIdentical(converterMapping, otherConverterMapping);
            }
        }

        private static void VerifyConverterMappingsAreIdentical(ConverterMapping converterMapping, ConverterMapping otherConverterMapping)
        {
            Assert.AreEqual(converterMapping.CardName, otherConverterMapping.CardName);
            if (!string.IsNullOrWhiteSpace(converterMapping.CardSet) && !string.IsNullOrWhiteSpace(otherConverterMapping.CardSet))
            {
                Assert.AreEqual(converterMapping.CardSet, otherConverterMapping.CardSet);
            }
            Assert.AreEqual(converterMapping.Quantity, otherConverterMapping.Quantity);
            if (converterMapping.SelectedOCTGNCard != null)
            {
                VerifyConverterCardsAreIdentical(converterMapping.SelectedOCTGNCard, otherConverterMapping.SelectedOCTGNCard);
            }
            Assert.AreEqual(converterMapping.PotentialOCTGNCards.Count, otherConverterMapping.PotentialOCTGNCards.Count);

            foreach(ConverterCard potentialConverterCard in converterMapping.PotentialOCTGNCards)
            {
                ConverterCard otherPotentialConverterCard = otherConverterMapping.PotentialOCTGNCards.First(poc => poc.CardID == potentialConverterCard.CardID);
                VerifyConverterCardsAreIdentical(potentialConverterCard, otherPotentialConverterCard);
            }

            Assert.AreEqual(converterMapping.CardName, otherConverterMapping.CardName);
        }

        private static void VerifyConverterCardsAreIdentical(ConverterCard converterCard, ConverterCard otherConverterCard)
        {
            Assert.AreEqual(converterCard.CardID, otherConverterCard.CardID);
            Assert.AreEqual(converterCard.MultiverseID, otherConverterCard.MultiverseID);
            Assert.AreEqual(converterCard.Name, otherConverterCard.Name);
            Assert.AreEqual(converterCard.Set, otherConverterCard.Set);
        }
    }
}
