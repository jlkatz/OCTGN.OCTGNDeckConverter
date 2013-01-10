using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MTGDeckConverter.Model;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Media.Imaging;

namespace MTGDeckConverter.ViewModel
{
    public class ImportDeckWizardVM : ViewModelBase
    {
        #region Fields

        private WizardPage_ChooseDeckSourceType _WizardPage_ChooseDeckSourceType;
        private WizardPage_SelectFile _WizardPage_SelectFile;
        private WizardPage_EnterWebpage _WizardPage_EnterWebpage;
        private WizardPage_EnterText _WizardPage_EnterText;
        private WizardPage_CompareCards _WizardPage_CompareCards;

        #endregion Fields

        #region Constructor

        public ImportDeckWizardVM()
        {
            //Call Start Over, since it will reset everything and set the first page
            this.StartOver();
        }

        #endregion Constructor

        #region Public Properties

        internal const string CurrentWizardPageVMPropertyName = "CurrentWizardPageVM";
        private ImportDeckWizardPageVM _CurrentWizardPageVM;
        public ImportDeckWizardPageVM CurrentWizardPageVM
        {
            get { return _CurrentWizardPageVM; }
            private set { this.SetValue(ref _CurrentWizardPageVM, value, CurrentWizardPageVMPropertyName); }
        }

        internal const string InlineDialogPropertyName = "InlineDialog";
        private InlineDialogVM _InlineDialog;
        public InlineDialogVM InlineDialog
        {
            get { return _InlineDialog; }
            set { this.SetValue(ref _InlineDialog, value, InlineDialogPropertyName); }
        }

        internal const string ConverterPropertyName = "Converter";
        private Converter _Converter;
        public Converter Converter
        {
            get { return _Converter; }
            private set { this.SetValue(ref _Converter, value, ConverterPropertyName); }
        }

        #endregion Public Properties

        #region Commands

        #region Choose Another Card Command
        CommandViewModel<ConverterMapping> _ChooseAnotherCardCommand;
        public CommandViewModel<ConverterMapping> ChooseAnotherCardCommand
        {
            get
            {
                if (_ChooseAnotherCardCommand == null)
                {
                    _ChooseAnotherCardCommand = new CommandViewModel<ConverterMapping>
                    (
                        "...",
                        new RelayCommand<ConverterMapping>
                        (
                            (cm) => 
                            {
                                this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ChooseAnotherCardVM(cm));
                                
                                this.InlineDialog.CallWhenCompletedMethod = delegate(InlineDialogVM inlineDialogVM)
                                {
                                    if (inlineDialogVM.CompletedSuccessfully)
                                    {
                                        InlineDialogPage_ChooseAnotherCardVM page = inlineDialogVM.InlineDialogPage as InlineDialogPage_ChooseAnotherCardVM;
                                        if(page.SelectedConverterCard != null)
                                        {
                                            cm.AddPotentialOCTGNCard(page.SelectedConverterCard);
                                            cm.SelectedOCTGNCard = page.SelectedConverterCard;
                                            this.Converter.UpdateCardCounts();
                                        }
                                    }
                                };
                            }
                        )
                    );
                }
                return _ChooseAnotherCardCommand;
            }
        }
        #endregion Choose Another Card Command

        #region Choose Included Sets Command
        CommandViewModel _ChooseIncludedSetsCommand;
        public CommandViewModel ChooseIncludedSetsCommand
        {
            get
            {
                if (_ChooseIncludedSetsCommand == null)
                {
                    _ChooseIncludedSetsCommand = new CommandViewModel
                    (
                        "Choose Included Sets...",
                        new RelayCommand
                        (
                            () =>
                            {
                                this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ChooseIncludedSetsVM());
                            }
                        )
                    );
                }
                return _ChooseIncludedSetsCommand;
            }
        }
        #endregion Choose Included Sets Command

        #region Command Buttons

        #region Next Step Command
        CommandViewModel _NextStepCommand;
        public CommandViewModel NextStepCommand
        {
            get
            {
                if (_NextStepCommand == null)
                {
                    _NextStepCommand = new CommandViewModel
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
                return _NextStepCommand;
            }
        }
        #endregion Next Step Command

        #region StartOver Command
        CommandViewModel _StartOverCommand;
        public CommandViewModel StartOverCommand
        {
            get
            {
                if (_StartOverCommand == null)
                {
                    _StartOverCommand = new CommandViewModel
                    (
                        "Start Over",
                        new RelayCommand
                        (
                            () => this.StartOver()
                        )
                    );
                }
                return _StartOverCommand;
            }
        }
        #endregion StartOver Command

        #endregion Command Buttons

        #endregion Commands

        #region Public Methods

        public bool IsCurrentWizardPageTheLastStep()
        {
            return this.CurrentWizardPageVM == _WizardPage_CompareCards;
        }

        public void MoveToNextStep()
        {
            if (this.IsCurrentWizardPageTheLastStep())
            {
                if (this.SaveDeck())
                {
                    this.InlineDialog = new InlineDialogVM
                    (
                        new InlineDialogPage_MessageVM
                        (
                            "Congratulations, you successfully converted your Deck to OCTGN!  Click Ok to start over.",
                            "Conversion Complete"
                        )
                    );
                    this.InlineDialog.CallWhenCompletedMethod = delegate(InlineDialogVM inlineDialogVM)
                    {
                        this.StartOver();
                    };
                }
            }
            else
            {
                ImportDeckWizardPageVM nextPage = this.DetermineNextPage();

                if (nextPage == _WizardPage_CompareCards)
                {
                    HandleConversionAndShowCompareCardsPage();
                }
                else
                {
                    this.SetCurrentPage(nextPage);
                }
            }
        }

        #endregion Public Methods
        
        #region Private Methods

        //If the next page is CompareCards, then that means it is time to attempt conversion
        //If the conversion is successful, advance to CompareCards
        //If the conversion fails, show a (hopefully) helpful message and do not advance
        //While converting, show an in-progress dialog
        private void HandleConversionAndShowCompareCardsPage()
        {
            //Show the dialog that conversion is in progress
            this.InlineDialog = new InlineDialogVM(new InlineDialogPage_ConvertingCardsVM());

            System.Threading.Tasks.Task<Tuple<bool, string>> conversionTask =
                System.Threading.Tasks.Task<Tuple<bool, string>>.Factory.StartNew(() =>
                {
                    DateTime startConversionTime = DateTime.Now;

                    Tuple<bool, string> result = this.Converter.Convert();
                    
                    TimeSpan timeSinceStart = DateTime.Now - startConversionTime;
                    double minSeconds = 0.5;  //Wait at least this long so the conversion process is convincing
                    if (timeSinceStart.TotalSeconds < minSeconds)
                    {
                        TimeSpan leftover = TimeSpan.FromSeconds(minSeconds - timeSinceStart.TotalSeconds);
                        Console.WriteLine("Finished! Waiting to be convincing for " + leftover);
                        System.Threading.Thread.Sleep(leftover);
                    }
                    return result;
                });

            //Continue with this if conversion completes successfully
            conversionTask.ContinueWith((t) =>
            {
                if (t.Result.Item1)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(
                        () =>
                        {
                            this.InlineDialog = null;
                            this.SetCurrentPage(_WizardPage_CompareCards);
                        });
                }
                else
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("An error occurred while trying to convert the deck.  Please try again.");
                    message.AppendLine();
                    message.AppendLine("Details:");
                    message.AppendLine(t.Result.Item2);
                    DispatcherHelper.CheckBeginInvokeOnUI(
                        () => 
                        {
                            this.InlineDialog = new InlineDialogVM(new InlineDialogPage_MessageVM(message.ToString(), "Error While Converting Deck"));
                        }
                    );
                }
            }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion);

            //Or continue with this if importing threw an unexpected exception
            conversionTask.ContinueWith((t) =>
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
                System.Diagnostics.Debug.WriteLine(message.ToString());

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.InlineDialog = new InlineDialogVM(new InlineDialogPage_MessageVM(message.ToString(), "Unexpected Exception While Converting Deck"));
                });
            }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
        }

        private ImportDeckWizardPageVM DetermineNextPage()
        {
            if (this.CurrentWizardPageVM == _WizardPage_ChooseDeckSourceType)
            {
                switch (this.Converter.DeckSourceType)
                {
                    case DeckSourceTypes.File:
                        return _WizardPage_SelectFile;

                    case DeckSourceTypes.Webpage:
                        return _WizardPage_EnterWebpage;

                    case DeckSourceTypes.Text:
                        return _WizardPage_EnterText;

                    default:
                        throw new NotImplementedException();
                }
            }

            else if 
            (
                this.CurrentWizardPageVM == _WizardPage_SelectFile ||
                this.CurrentWizardPageVM == _WizardPage_EnterWebpage ||
                this.CurrentWizardPageVM == _WizardPage_EnterText
            )
            {
                return _WizardPage_CompareCards;
            }

            else
            {
                throw new InvalidOperationException("A Next Page should never be requested from the current Page");
            }
        }

        private bool SaveDeck()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
            {
                AddExtension = true,
                Filter = "Octgn decks|*.o8d",
                FileName = this.Converter.DeckName + ".o8d",
            };

            //Attempt to set the initial directory if it exists
            //(It might not exist if the last location was a USB stick for example)
            if (System.IO.Directory.Exists(SettingsManager.SingletonInstance.SaveFileDirectory))
            {
                sfd.InitialDirectory = SettingsManager.SingletonInstance.SaveFileDirectory;
            }
            else
            {
                //If it doesn't exist, use the OCTGN Game Definition's default location
                sfd.InitialDirectory = ConverterDatabase.SingletonInstance.GameDefinition.DefaultDecksPath;
            }

            if (!sfd.ShowDialog().GetValueOrDefault())
            {
                return false;
            }
            else
            {
                try
                {
                    SettingsManager.SingletonInstance.SaveFileDirectory = System.IO.Path.GetDirectoryName(sfd.FileName);
                    this.Converter.SaveDeck(sfd.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show
                    (
                        "An error occured while trying to save the deck:\n" + ex.Message, "Error",
                        System.Windows.MessageBoxButton.OK, 
                        System.Windows.MessageBoxImage.Error
                    );
                    return false;
                }
            }
        }
        
        private void StartOver()
        {
            _WizardPage_ChooseDeckSourceType = new WizardPage_ChooseDeckSourceType(this);
            _WizardPage_SelectFile = new WizardPage_SelectFile(this);
            _WizardPage_EnterWebpage = new WizardPage_EnterWebpage(this);
            _WizardPage_EnterText = new WizardPage_EnterText(this);
            _WizardPage_CompareCards = new WizardPage_CompareCards(this);

            this.Converter = new Converter();

            this.SetCurrentPage(_WizardPage_ChooseDeckSourceType);
        }

        private void SetCurrentPage(ImportDeckWizardPageVM page)
        {
            this.CurrentWizardPageVM = page;
            this.NextStepCommand.DisplayName = this.CurrentWizardPageVM == _WizardPage_CompareCards ? 
                "Save Deck..." : 
                "Next >";
        }

        #endregion Private Methods

        #region ViewModelBase Helpers

        //http://www.pochet.net/blog/2010/06/25/inotifypropertychanged-implementations-an-overview/
        /// <summary>
        /// Returns True if the property was changed, false if no change
        /// </summary>
        protected bool SetValue<T>(ref T property, T value, string propertyName, bool broadcast = false)
        {
            //TODO: Make this method an extension of ViewModelBase
            if (Object.Equals(property, value))
            {
                return false;
            }
            var oldValue = property;
            property = value;

            this.RaisePropertyChanged<T>(propertyName, oldValue, value, broadcast);

            return true;
        }

        #endregion ViewModelBase Helpers

        #region Static Helpers

        public static BitmapImage GetCardBitmapImage(Guid cardID)
        {
            var bim = new BitmapImage();
            bim.BeginInit();
            bim.CacheOption = BitmapCacheOption.OnLoad;

            try
            {
                Octgn.Data.CardModel octgnCardModel = ConverterDatabase.SingletonInstance.GameDefinition.GetCardById(cardID);
                bim.UriSource = Octgn.Data.CardModel.GetPictureUri(ConverterDatabase.SingletonInstance.GameDefinition, octgnCardModel.Set.Id, octgnCardModel.ImageUri);
                bim.EndInit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error loading picture uri from game pack: " + ex.ToString());
                bim = new BitmapImage();
                bim.CacheOption = BitmapCacheOption.OnLoad;
                bim.BeginInit();
                bim.UriSource = new Uri(@"pack://application:,,,/Octgn;component/Resources/Front.jpg");
                bim.EndInit();

            }
            return bim;
        }

        #endregion Static Helpers
    }
}
