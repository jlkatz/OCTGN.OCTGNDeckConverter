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
using MTGDeckConverter.Model;
using Octgn.Core.DataExtensionMethods;

namespace MTGDeckConverter.ViewModel
{
    /// <summary>
    /// The ViewModel which drives the Import Deck Wizard.  This is responsible for determining
    /// what Wizard page is shown based on user input.
    /// </summary>
    public class ImportDeckWizardVM : Model.INotifyPropertyChangedBase
    {
        #region Fields

        /// <summary>
        /// ChooseDeckSourceType Wizard Page VM backing field
        /// </summary>
        private WizardPage_ChooseDeckSourceType _WizardPage_ChooseDeckSourceType;

        /// <summary>
        /// SelectFile Wizard Page VM backing field
        /// </summary>
        private WizardPage_SelectFile _WizardPage_SelectFile;

        /// <summary>
        /// EnterWebpage Wizard Page VM backing field
        /// </summary>
        private WizardPage_EnterWebpage _WizardPage_EnterWebpage;

        /// <summary>
        /// EnterText Wizard Page VM backing field
        /// </summary>
        private WizardPage_EnterText _WizardPage_EnterText;

        /// <summary>
        /// CompareCards Wizard Page VM backing field
        /// </summary>
        private WizardPage_CompareCards _WizardPage_CompareCards;

        #endregion Fields

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

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private bool _Completed = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user has clicked a command to close the Wizard or not.  (Such as Complete or Cancel)
        /// </summary>
        public bool Completed
        {
            get { return this._Completed; }
            protected set { this.SetValue(ref this._Completed, value, CompletedPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string CurrentWizardPageVMPropertyName = "CurrentWizardPageVM";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private ImportDeckWizardPageVM _CurrentWizardPageVM;
        
        /// <summary>
        /// Gets the View Model which represents the current Wizard Page that should be displayed
        /// </summary>
        public ImportDeckWizardPageVM CurrentWizardPageVM
        {
            get { return this._CurrentWizardPageVM; }
            private set { this.SetValue(ref this._CurrentWizardPageVM, value, CurrentWizardPageVMPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string InlineDialogPropertyName = "InlineDialog";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private InlineDialogVM _InlineDialog;
        
        /// <summary>
        /// Gets or sets the View Model which represents an Inline Dialog that should be shown over the current Wizard Page
        /// </summary>
        public InlineDialogVM InlineDialog
        {
            get { return this._InlineDialog; }
            set { this.SetValue(ref this._InlineDialog, value, InlineDialogPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string ConverterPropertyName = "Converter";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private Converter _Converter;
        
        /// <summary>
        /// Gets the Converter which contains all of the parameters and data about the conversion
        /// </summary>
        public Converter Converter
        {
            get { return this._Converter; }
            private set { this.SetValue(ref this._Converter, value, ConverterPropertyName); }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string CompletedPropertyName = "Completed";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Property name constant")]
        internal const string WasNotCancelledPropertyName = "WasNotCancelled";

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private bool _WasNotCancelled = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user cancelled the Import Deck Wizard when closing it or not.
        /// </summary>
        public bool WasNotCancelled
        {
            get { return this._WasNotCancelled; }
            protected set { this.SetValue(ref this._WasNotCancelled, value, WasNotCancelledPropertyName); }
        }

        #endregion Public Properties

        #region Commands

        #region Choose Another Card Command

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel<ConverterMapping> _ChooseAnotherCardCommand;
        
        /// <summary>
        /// Gets the Command which will instruct the Wizard to show a popup which allows the user to pick another
        /// card from across all the installed MTG sets to represent the converted card text.
        /// </summary>
        public CommandViewModel<ConverterMapping> ChooseAnotherCardCommand
        {
            get
            {
                if (this._ChooseAnotherCardCommand == null)
                {
                    this._ChooseAnotherCardCommand = new CommandViewModel<ConverterMapping>
                    (
                        "...",
                        new RelayCommand<ConverterMapping>
                        (
                            (cm) => 
                            {
                                this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ChooseAnotherCardVM(cm));
                                
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

                        )
                    );
                }

                return this._ChooseAnotherCardCommand;
            }
        }
        #endregion Choose Another Card Command

        #region Choose Included Sets Command

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _ChooseIncludedSetsCommand;
        
        /// <summary>
        /// Gets the Command which will instruct the Wizard to show a popup that allows the user to choose
        /// which Sets will be included when searching for matching cards.
        /// </summary>
        public CommandViewModel ChooseIncludedSetsCommand
        {
            get
            {
                if (this._ChooseIncludedSetsCommand == null)
                {
                    this._ChooseIncludedSetsCommand = new CommandViewModel
                    (
                        "Choose Included Sets...",
                        new RelayCommand
                        (
                            () =>
                            {
                                this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ChooseIncludedSetsVM());

                                this.InlineDialog.CallWhenCompletedMethod = delegate(InlineDialogVM inlineDialogVM)
                                {
                                    // Save settings immediately, so that the chosen sets to exclude is remembered upon quitting.
                                    ConverterDatabase.SingletonInstance.UpdateSetsExcludedFromSearches();
                                    Model.SettingsManager.SingletonInstance.SaveSettingsManager();
                                };
                            }

                        )
                    );
                }

                return this._ChooseIncludedSetsCommand;
            }
        }
        #endregion Choose Included Sets Command

        #region Command Buttons

        #region Next Step Command

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _NextStepCommand;
        
        /// <summary>
        /// Gets the Command which instructs the Wizard to move to the next step.
        /// </summary>
        public CommandViewModel NextStepCommand
        {
            get
            {
                if (this._NextStepCommand == null)
                {
                    this._NextStepCommand = new CommandViewModel
                    (
                        "Next >",
                        new RelayCommand
                        (
                            () => this.MoveToNextStep(),
                            () =>
                            {
                                return this.CurrentWizardPageVM != null ?
                                    this.CurrentWizardPageVM.CanMoveToNextStep :
                                    false;
                            }

                        )
                    );
                }

                return this._NextStepCommand;
            }
        }
        #endregion Next Step Command

        #region StartOver Command

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _StartOverCommand;
        
        /// <summary>
        /// Gets the Command which instructs the Wizard to start over.
        /// </summary>
        public CommandViewModel StartOverCommand
        {
            get
            {
                if (this._StartOverCommand == null)
                {
                    this._StartOverCommand = new CommandViewModel
                    (
                        "Start Over",
                        new RelayCommand
                        (
                            () => this.StartOver()
                        )
                    );
                }

                return this._StartOverCommand;
            }
        }
        #endregion StartOver Command

        #region Cancel Command

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private backing field")]
        private CommandViewModel _CancelCommand;
        
        /// <summary>
        /// Gets the Command which instructs the Wizard to be cancelled.
        /// </summary>
        public CommandViewModel CancelCommand
        {
            get
            {
                if (this._CancelCommand == null)
                {
                    this._CancelCommand = new CommandViewModel
                    (
                        "Cancel",
                        new RelayCommand
                        (
                            () => this.CloseWizard(false)
                        )
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
            return this.CurrentWizardPageVM == this._WizardPage_CompareCards;
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

                if (nextPage == this._WizardPage_CompareCards)
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
                        this.SetCurrentPage(_WizardPage_CompareCards);
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
            if (this.CurrentWizardPageVM == this._WizardPage_ChooseDeckSourceType)
            {
                switch (this.Converter.DeckSourceType)
                {
                    case DeckSourceTypes.File:
                        return this._WizardPage_SelectFile;

                    case DeckSourceTypes.Webpage:
                        return this._WizardPage_EnterWebpage;

                    case DeckSourceTypes.Text:
                        return this._WizardPage_EnterText;

                    default:
                        throw new NotImplementedException();
                }
            }
            else if 
            (
                this.CurrentWizardPageVM == this._WizardPage_SelectFile ||
                this.CurrentWizardPageVM == this._WizardPage_EnterWebpage ||
                this.CurrentWizardPageVM == this._WizardPage_EnterText
            )
            {
                return this._WizardPage_CompareCards;
            }
            else
            {
                throw new InvalidOperationException("A Next Page should never be requested from the current Page");
            }
        }
                
        /// <summary>
        /// Starts this Wizard over by re-instantiating the critical objects which keep track of state and data.
        /// </summary>
        private void StartOver()
        {
            this._WizardPage_ChooseDeckSourceType = new WizardPage_ChooseDeckSourceType(this);
            this._WizardPage_SelectFile = new WizardPage_SelectFile(this);
            this._WizardPage_EnterWebpage = new WizardPage_EnterWebpage(this);
            this._WizardPage_EnterText = new WizardPage_EnterText(this);
            this._WizardPage_CompareCards = new WizardPage_CompareCards(this);

            this.Converter = new Converter();

            this.SetCurrentPage(this._WizardPage_ChooseDeckSourceType);
        }

        /// <summary>
        /// Sets the current page of the Wizard to page.
        /// </summary>
        /// <param name="page">The page object to set the Wizard to</param>
        private void SetCurrentPage(ImportDeckWizardPageVM page)
        {
            this.CurrentWizardPageVM = page;
            this.NextStepCommand.DisplayName = this.CurrentWizardPageVM == this._WizardPage_CompareCards ? 
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
        /// <returns>The BitmapImage of the Octgn Card with corresponding Guid</returns>
        public static BitmapImage GetCardBitmapImage(Guid cardID)
        {
            var bim = new BitmapImage();
            bim.BeginInit();
            bim.CacheOption = BitmapCacheOption.OnLoad;

            try
            {
                Octgn.DataNew.Entities.Card octgnCard = ConverterDatabase.SingletonInstance.GameDefinition.AllCards().First(c => c.Id == cardID);
                bim.UriSource = new Uri(octgnCard.GetPicture());
                bim.EndInit();
            }
            catch (Exception)
            {
                bim = new BitmapImage();
                bim.CacheOption = BitmapCacheOption.OnLoad;
                bim.BeginInit();
                bim.UriSource = new Uri(ConverterDatabase.SingletonInstance.GameDefinition.CardFront);
                bim.EndInit();
            }

            return bim;
        }

        #endregion Static Helpers
    }
}
