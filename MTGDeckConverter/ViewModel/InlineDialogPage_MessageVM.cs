using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.ViewModel
{
    public class InlineDialogPage_MessageVM : InlineDialogPageVM
    {
        public InlineDialogPage_MessageVM(string message)
            : this(message, "")
        { }

        public InlineDialogPage_MessageVM(string message, string title)
        {
            this.Message = message;
            _Title = title;
        }

        public string Message
        {
            get;
            private set;
        }
        
        private string _Title;
        public override string Title
        {
            get { return _Title; }
        }

        public override bool CancelButtonVisible
        { get { return false; } }
    }
}
