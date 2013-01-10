using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MTGDeckConverter.ViewModel
{
    public class InlineDialogVM : ViewModelBase
    {
        #region Constructor

        public InlineDialogVM(InlineDialogPageVM inlineDialogPageVM)
        {
            if (inlineDialogPageVM == null)
            { throw new ArgumentNullException(); }

            this.InlineDialogPage = inlineDialogPageVM;
        }

        #endregion // Constructor

        #region Public Properties

        public CallWhenCompleted CallWhenCompletedMethod
        {
            get;
            set;
        }
        
        internal const string CompletedPropertyName = "Completed";
        private bool _Completed = false;
        public bool Completed
        {
            get { return _Completed; }
            protected set 
            { 
                _Completed = value;
                RaisePropertyChanged(CompletedPropertyName); 
            } 
        }

        internal const string CompletedSuccessfullyPropertyName = "CompletedSuccessfully";
        private bool _CompletedSuccessfully = false;
        public bool CompletedSuccessfully
        {
            get { return _CompletedSuccessfully; }
            protected set
            {
                _CompletedSuccessfully = value;
                RaisePropertyChanged(CompletedSuccessfullyPropertyName);
            }
        }

        public InlineDialogPageVM InlineDialogPage
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Commands

        private CommandViewModel _CancelButtonCommand;
        public CommandViewModel CancelButtonCommand
        {
            get
            {
                if (_CancelButtonCommand == null)
                {
                    _CancelButtonCommand = new CommandViewModel
                    (
                        this.InlineDialogPage.CancelButtonText,
                        new RelayCommand
                        (
                            () => this.CloseCommand(false),
                            () => this.InlineDialogPage.CancelButtonEnabled
                        )
                    );
                }
                return _CancelButtonCommand;
            }
        }

        private CommandViewModel _OkButtonCommand;
        public CommandViewModel OkButtonCommand
        {
            get
            {
                if (_OkButtonCommand == null)
                {
                    _OkButtonCommand = new CommandViewModel
                    (
                        this.InlineDialogPage.OkButtonText,
                        new RelayCommand
                        (
                            () => this.CloseCommand(true),
                            () => this.InlineDialogPage.OkButtonEnabled
                        )
                    );
                }
                return _OkButtonCommand;
            }
        }

        #endregion Commands

        #region Public Methods

        public delegate void CallWhenCompleted(InlineDialogVM inlineDialogVM);

        public void CloseCommand(bool completedSuccessfully)
        {
            this.CompletedSuccessfully = completedSuccessfully;
            this.Close();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Closes the Inline Dialog, and executes the code specified to run after closing
        /// </summary>
        private void Close()
        {
            if (this.CallWhenCompletedMethod != null)
            {
                this.CallWhenCompletedMethod(this);
            }
            this.Completed = true;
        }

        #endregion Private
    }
}
