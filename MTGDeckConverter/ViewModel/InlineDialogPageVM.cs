using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace MTGDeckConverter.ViewModel
{
    public abstract class InlineDialogPageVM : ViewModelBase
    {
        internal virtual string CancelButtonText { get { return "Cancel"; } }
        public virtual bool CancelButtonEnabled { get { return true; } }
        public virtual bool CancelButtonVisible { get { return true; } }

        internal virtual string OkButtonText { get { return "Ok"; } }
        public virtual bool OkButtonEnabled { get { return true; } }
        public virtual bool OkButtonVisible { get { return true; } }

        public abstract string Title { get; }
    }
}
