// -----------------------------------------------------------------------
// <copyright file="ImportDeckWizardVM.cs" company="jlkatz">
// Copyright (c) 2013 Justin L Katz. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Octgn.Core.DataExtensionMethods;
using OCTGNDeckConverter.Model;

namespace OCTGNDeckConverter.ViewModel
{
    /// <summary>
    /// The ViewModel which drives the Import Deck Wizard.  This is responsible for determining
    /// what Wizard page is shown based on user input.
    /// </summary>
    public class ImportDeckWizardVM : Model.INotifyPropertyChangedBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ImportDeckWizardVM class.
        /// </summary>
        public ImportDeckWizardVM()
        {
            // Call Start Over, since it will reset everything and set the first page
            this.StartOver();
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Raised when the Import Deck Wizard is commanded to Close.
        /// </summary>
        public event EventHandler Close;

        #endregion Events

        #region Public Properties

        /// <summary>
        /// Private backing field
        /// </summary>
        private bool _Completed = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user has clicked a command to close the Wizard or not.  (Such as Complete or Cancel)
        /// </summary>
        public bool Completed
        {
            get { return this._Completed; }
            protected set { this.SetValue(ref this._Completed, value, CompletedPropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string CurrentWizardPageVMPropertyName = "CurrentWizardPageVM";

        /// <summary>
        /// Private backing field
        /// </summary>
        private ImportDeckWizardPageVM _CurrentWizardPageVM;
        
        /// <summary>
        /// Gets the View Model which represents the current Wizard Page that should be displayed
        /// </summary>
        public ImportDeckWizardPageVM CurrentWizardPageVM
        {
            get { return this._CurrentWizardPageVM; }
            private set { this.SetValue(ref this._CurrentWizardPageVM, value, CurrentWizardPageVMPropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string InlineDialogPropertyName = "InlineDialog";

        /// <summary>
        /// Private backing field
        /// </summary>
        private InlineDialogVM _InlineDialog;
        
        /// <summary>
        /// Gets or sets the View Model which represents an Inline Dialog that should be shown over the current Wizard Page
        /// </summary>
        public InlineDialogVM InlineDialog
        {
            get { return this._InlineDialog; }
            set { this.SetValue(ref this._InlineDialog, value, InlineDialogPropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string ConverterPropertyName = "Converter";

        /// <summary>
        /// Private backing field
        /// </summary>
        private Converter _Converter;
        
        /// <summary>
        /// Gets the Converter which contains all of the parameters and data about the conversion
        /// </summary>
        public Converter Converter
        {
            get { return this._Converter; }
            private set { this.SetValue(ref this._Converter, value, ConverterPropertyName); }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string CompletedPropertyName = "Completed";

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string WasNotCancelledPropertyName = "WasNotCancelled";

        /// <summary>
        /// Private backing field
        /// </summary>
        private bool _WasNotCancelled = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user cancelled the Import Deck Wizard when closing it or not.
        /// </summary>
        public bool WasNotCancelled
        {
            get { return this._WasNotCancelled; }
            protected set { this.SetValue(ref this._WasNotCancelled, value, WasNotCancelledPropertyName); }
        }

        /// <summary>
        /// Gets the welcome message to be shown by whatever wizard page is the first seen by the user.
        /// </summary>
        public string WelcomeMessage
        {
            get { return "OCTGN Deck Converter will help you to quickly and easily convert your decks into the OCTGN format.  You can convert from a file on your computer, a webpage, or even paste the card list directly."; }
        }

        #endregion Public Properties

        #region Commands

        #region Choose Another Card Command

        /// <summary>
        /// Private backing field
        /// </summary>
        private RelayCommand<ConverterMapping> _ChooseAnotherCardCommand;
        
        /// <summary>
        /// Gets the Command which will instruct the Wizard to show a popup which allows the user to pick another
        /// card from across all the installed MTG sets to represent the converted card text.
        /// </summary>
        public RelayCommand<ConverterMapping> ChooseAnotherCardCommand
        {
            get
            {
                if (this._ChooseAnotherCardCommand == null)
                {
                    this._ChooseAnotherCardCommand = new RelayCommand<ConverterMapping>
                    (
                        (cm) => 
                        {
                            this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ChooseAnotherCardVM(cm, this.Converter.ConverterGame));

                            this.InlineDialog.CallWhenCompletedMethod = delegate(InlineDialogVM inlineDialogVM)
                            {
                                if (inlineDialogVM.WasNotCancelled)
                                {
                                    InlineDialogPage_ChooseAnotherCardVM page = inlineDialogVM.InlineDialogPage as InlineDialogPage_ChooseAnotherCardVM;
                                    if (page.SelectedConverterCard != null)
                                    {
                                        cm.AddPotentialOCTGNCard(page.SelectedConverterCard);
                                        cm.SelectedOCTGNCard = page.SelectedConverterCard;
                                        this.Converter.ConverterDeck.UpdateCardCounts();
                                    }
                                }
                            };
                        }

                    );
                }

                return this._ChooseAnotherCardCommand;
            }
        }
        #endregion Choose Another Card Command

        #region Choose Included Sets Command

        /// <summary>
        /// Private backing field
        /// </summary>
        private RelayCommand _ChooseIncludedSetsCommand;
        
        /// <summary>
        /// Gets the Command which will instruct the Wizard to show a popup that allows the user to choose
        /// which Sets will be included when searching for matching cards.
        /// </summary>
        public RelayCommand ChooseIncludedSetsCommand
        {
            get
            {
                if (this._ChooseIncludedSetsCommand == null)
                {
                    this._ChooseIncludedSetsCommand = new RelayCommand
                    (
                        () =>
                        {
                            this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ChooseIncludedSetsVM(this.Converter.ConverterGame));

                            this.InlineDialog.CallWhenCompletedMethod = delegate(InlineDialogVM inlineDialogVM)
                            {
                                // Save settings immediately, so that the chosen sets to exclude is remembered upon quitting.
                                this.Converter.ConverterGame.UpdateSetsExcludedFromSearches();
                                Model.SettingsManager.SingletonInstance.SaveSettingsManager();
                            };
                        }

                    );
                }

                return this._ChooseIncludedSetsCommand;
            }
        }
        #endregion Choose Included Sets Command

        #region Command Buttons

        #region Next Step Command

        /// <summary>
        /// Private backing field
        /// </summary>
        private RelayCommand _NextStepCommand;
        
        /// <summary>
        /// Gets the Command which instructs the Wizard to move to the next step.
        /// </summary>
        public RelayCommand NextStepCommand
        {
            get
            {
                if (this._NextStepCommand == null)
                {
                    this._NextStepCommand = new RelayCommand
                    (
                        () => this.MoveToNextStep(),
                        () =>
                        {
                            return this.CurrentWizardPageVM != null ?
                                this.CurrentWizardPageVM.CanMoveToNextStep :
                                false;
                        }

                    );
                }

                return this._NextStepCommand;
            }
        }

        /// <summary>
        /// Property name constant
        /// </summary>
        internal const string NextStepCommandDisplayNamePropertyName = "NextStepCommandDisplayName";

        /// <summary>
        /// Private backing field
        /// </summary>
        private string _NextStepCommandDisplayName = "Next >";

        /// <summary>
        /// Gets the text to show on the 'Next Step' button
        /// </summary>
        public string NextStepCommandDisplayName
        {
            get { return this._NextStepCommandDisplayName; }
            private set { this.SetValue(ref this._NextStepCommandDisplayName, value, NextStepCommandDisplayNamePropertyName); }
        }

        #endregion Next Step Command

        #region StartOver Command

        /// <summary>
        /// Private backing field
        /// </summary>
        private RelayCommand _StartOverCommand;
        
        /// <summary>
        /// Gets the Command which instructs the Wizard to start over.
        /// </summary>
        public RelayCommand StartOverCommand
        {
            get
            {
                if (this._StartOverCommand == null)
                {
                    this._StartOverCommand = new RelayCommand
                    (
                        () => this.StartOver()
                    );
                }

                return this._StartOverCommand;
            }
        }
        #endregion StartOver Command

        #region Cancel Command

        /// <summary>
        /// Private backing field
        /// </summary>
        private RelayCommand _CancelCommand;
        
        /// <summary>
        /// Gets the Command which instructs the Wizard to be cancelled.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                if (this._CancelCommand == null)
                {
                    this._CancelCommand = new RelayCommand
                    (
                        () => this.CloseWizard(false)
                    );
                }

                return this._CancelCommand;
            }
        }
        #endregion Cancel Command

        #endregion Command Buttons

        #endregion Commands

        #region Public Methods

        /// <summary>
        /// Returns true if the current page is the last step, false otherwise.
        /// </summary>
        /// <returns>True if the current page is the last step, false otherwise.</returns>
        public bool IsCurrentWizardPageTheLastStep()
        {
            return this.CurrentWizardPageVM is WizardPage_CompareCards;
        }

        /// <summary>
        /// Instructs the Wizard to move to the next step.  This method will determine what the next step is
        /// based on the current step and the input parameters.
        /// </summary>
        public void MoveToNextStep()
        {
            if (this.IsCurrentWizardPageTheLastStep())
            {
                this.CloseWizard(true);
            }
            else
            {
                ImportDeckWizardPageVM nextPage = this.DetermineNextPage();

                if (nextPage is WizardPage_CompareCards)
                {
                    this.HandleConversionAndShowCompareCardsPage();
                }
                else
                {
                    this.SetCurrentPage(nextPage);
                }
            }
        }

        #endregion Public Methods
        
        #region Private Methods

        /// <summary>
        /// Closes the Import Deck Wizard
        /// </summary>
        /// <param name="wasNotCancelled">A value indicating whether the Inline Dialog was _not_ cancelled when closed or not</param>
        private void CloseWizard(bool wasNotCancelled)
        {
            this.WasNotCancelled = wasNotCancelled;
            this.Completed = true;

            var handler = this.Close;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Converts the cards.  If the conversion is successful, advance to CompareCards
        /// If the conversion fails, show a (hopefully) helpful message and do not advance
        /// While converting, show an in-progress dialog
        /// </summary>
        private void HandleConversionAndShowCompareCardsPage()
        {
            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
            var cancellationToken = new System.Threading.CancellationToken();

            // Show the dialog that conversion is in progress
            this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ConvertingCardsVM());

            System.Threading.Tasks.Task<Tuple<bool, string>> conversionTask =
                System.Threading.Tasks.Task<Tuple<bool, string>>.Factory.StartNew(() =>
                {
                    DateTime startConversionTime = DateTime.Now;

                    Tuple<bool, string> result = this.Converter.Convert();
                    
                    TimeSpan timeSinceStart = DateTime.Now - startConversionTime;

                    // Wait at least this long so the conversion process is convincing
                    double minSeconds = 0.5;  
                    if (timeSinceStart.TotalSeconds < minSeconds)
                    {
                        TimeSpan leftover = TimeSpan.FromSeconds(minSeconds - timeSinceStart.TotalSeconds);
                        Console.WriteLine("Finished! Waiting to be convincing for " + leftover);
                        System.Threading.Thread.Sleep(leftover);
                    }

                    return result;
                });

            // Continue with this if conversion completes successfully
            conversionTask.ContinueWith
            (
                (t) => 
                {
                    if (t.Result.Item1)
                    {
                        this.InlineDialog = null;
                        this.SetCurrentPage(new WizardPage_CompareCards(this));
                    }
                    else
                    {
                        StringBuilder message = new StringBuilder();
                        message.AppendLine("An error occurred while trying to convert the deck.  Please try again.");
                        message.AppendLine();
                        message.AppendLine("Details:");
                        message.AppendLine(t.Result.Item2);
                        this.InlineDialog = new InlineDialogVM(new InlineDialogPage_MessageVM(message.ToString(), "Error While Converting Deck"));
                    }
                },
                cancellationToken,
                System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion, 
                uiScheduler
            );

            // Or continue with this if importing threw an unexpected exception
            conversionTask.ContinueWith
            (
                (t) => 
                {
                    AggregateException aggEx = t.Exception;
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("An unexpected Exception occurred while trying to convert the deck.  Please try again.");
                    message.AppendLine();
                    message.AppendLine("Details:");
                    foreach (Exception e in aggEx.InnerExceptions)
                    {
                        message.AppendLine(e.ToString());
                        message.AppendLine();
                    }
                    
                    this.InlineDialog = new InlineDialogVM(new InlineDialogPage_MessageVM(message.ToString(), "Unexpected Exception While Converting Deck"));
                },
                cancellationToken,
                System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted,
                uiScheduler
            );
        }

        /// <summary>
        /// Determines what the next Wizard page should be based on the current page and input parameters.
        /// </summary>
        /// <returns>The next Wizard page that should be shown</returns>
        private ImportDeckWizardPageVM DetermineNextPage()
        {
            if (this.Converter.ConverterGame == null)
            {
                // The wizard just started
                if (Model.ConverterDatabase.SingletonInstance.OctgnGames.Count() == 1)
                {
                    // Since there is only 1 game, automatically select it
                    this.Converter.ConverterGame = Model.ConverterDatabase.SingletonInstance.GetConverterGame(Model.ConverterDatabase.SingletonInstance.OctgnGames.First());
                }
                else
                {
                    return new WizardPage_ChooseGame(this);
                }
            }

            bool skippedChooseGame = Model.ConverterDatabase.SingletonInstance.OctgnGames.Count() == 1;

            if (!this.Converter.DeckSourceType.HasValue)
            {
                if (this.Converter.ConverterGame.Game.Id == Model.ConvertEngine.Game.MTG.GameGuidStatic)
                {
                    // The chosen game was MTG, so allow the user to choose the deck source type by URL or File on disk or text
                    return new WizardPage_ChooseDeckSourceType(this, true, true, skippedChooseGame);
                }
                else if
                (
                    this.Converter.ConverterGame.Game.Id == Model.ConvertEngine.Game.LoTR.GameGuidStatic ||
                    this.Converter.ConverterGame.Game.Id == Model.ConvertEngine.Game.LoTR.GameGuidStatic
                )
                {
                    // The chosen game is available on some website, so allow URL or text
                    return new WizardPage_ChooseDeckSourceType(this, false, true, skippedChooseGame);
                }
                else
                {
                    this.Converter.DeckSourceType = DeckSourceTypes.Text;
                }
            }

            bool skippedChooseGameAndChooseDeckSource = skippedChooseGame && this.Converter.ConverterGame.Game.Id != Model.ConvertEngine.Game.MTG.GameGuidStatic;

            if
            (
                this.Converter.DeckSourceType.Value == DeckSourceTypes.File &&
                string.IsNullOrWhiteSpace(this.Converter.DeckFullPathName) &&
                string.IsNullOrWhiteSpace(this.Converter.DeckFileNameWithoutExtension)
            )
            {
                return new WizardPage_SelectFile(this, skippedChooseGameAndChooseDeckSource);
            }
            else if
            (
                this.Converter.DeckSourceType.Value == DeckSourceTypes.Webpage &&
                string.IsNullOrWhiteSpace(this.Converter.DeckURL)
            )
            {
                return new WizardPage_EnterWebpage(this, skippedChooseGameAndChooseDeckSource);
            }
            else if
            (
                this.Converter.DeckSourceType.Value == DeckSourceTypes.Text &&
                this.Converter.SectionsText.All(kvp => string.IsNullOrWhiteSpace(kvp.Value))
            )
            {
                return new WizardPage_EnterText(this, skippedChooseGameAndChooseDeckSource);
            }

            return new WizardPage_CompareCards(this);
        }
                
        /// <summary>
        /// Starts this Wizard over by re-instantiating the critical objects which keep track of state and data.
        /// </summary>
        private void StartOver()
        {
            this.Converter = new Converter();
            this.CurrentWizardPageVM = null;
            this.SetCurrentPage(this.DetermineNextPage());
        }

        /// <summary>
        /// Sets the current page of the Wizard to page.
        /// </summary>
        /// <param name="page">The page object to set the Wizard to</param>
        private void SetCurrentPage(ImportDeckWizardPageVM page)
        {
            this.CurrentWizardPageVM = page;
            this.NextStepCommandDisplayName = this.CurrentWizardPageVM is WizardPage_CompareCards ? 
                "Load Deck in OCTGN" : 
                "Next >";
        }

        #endregion Private Methods

        #region Static Helpers

        /// <summary>
        /// Returns an Image of the Octgn Card with corresponding Guid.  If there is an error (including if the Card Guid is not found)
        /// then the default back of the card is returned.
        /// </summary>
        /// <param name="cardID">The Guid of the Card to get a Bitmap picture of</param>
        /// <param name="game">The OCTGN game to find the Card from</param>
        /// <returns>The BitmapImage of the Octgn Card with corresponding Guid</returns>
        public static BitmapImage GetCardBitmapImage(Guid cardID, Octgn.DataNew.Entities.Game game)
        {
            var bim = new BitmapImage();
            bim.BeginInit();
            bim.CacheOption = BitmapCacheOption.OnLoad;

            try
            {
                Octgn.DataNew.Entities.Card octgnCard = game.AllCards().First(c => c.Id == cardID);
                bim.UriSource = new Uri(octgnCard.GetPicture());
                bim.EndInit();
            }
            catch (Exception)
            {
                bim = new BitmapImage();
                bim.CacheOption = BitmapCacheOption.OnLoad;
                bim.BeginInit();
                bim.UriSource = new Uri(game.CardFront);
                bim.EndInit();
            }

            return bim;
        }

        #endregion Static Helpers
    }
}
