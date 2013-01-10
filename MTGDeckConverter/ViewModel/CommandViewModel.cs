using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MTGDeckConverter.ViewModel
{

    public abstract class CommandViewModelBase : ViewModelBase
    {
        #region Constructor

        public CommandViewModelBase(string displayName, ICommand command)
        //    : this(displayName, command, null)
        //{ }

        //public CommandViewModelBase(string displayName, ICommand command, object commandParameter)
        {
            if (command == null)
            {
                //throw new ArgumentNullException("command");
            }

            this.DisplayName = displayName;
            this.Command = command;
            //this.CommandParameter = commandParameter;
            this.CommandList = new ObservableCollection<CommandViewModelBase>();
        }

        #endregion Constructor

        #region Public Properties

        private string _DisplayName;
        public string DisplayName 
        {
            get { return _DisplayName; }
            set
            {
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }

        public ICommand Command { get; protected set; }

        public object CommandParameter { get; protected set; }

        ObservableCollection<CommandViewModelBase> _commandList;
        public ObservableCollection<CommandViewModelBase> CommandList
        {
            get { return _commandList; }
            set
            {
                _commandList = value;
                RaisePropertyChanged("CommandList");
            }
        }

        #endregion Public Properties
    }

    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel : CommandViewModelBase
    {
        public CommandViewModel(string displayName, RelayCommand command)
            : base(displayName, command)
        //: this(displayName, command, null)
        { }
        //public CommandViewModel(string displayName, RelayCommand command, object commandParameter)
        //    : base(displayName, command, commandParameter)
        //{ }
    }

    /// <summary>
    /// Represents an actionable item displayed by a View.
    /// </summary>
    public class CommandViewModel<T> : CommandViewModelBase
    {
        public CommandViewModel(string displayName, RelayCommand<T> command)
            : base(displayName, command)
        //: this(displayName, command, null)
        { }
        //public CommandViewModel(string displayName, RelayCommand<T> command, object commandParameter)
        //    : base(displayName, command, commandParameter)
        //{ }
    }
}
