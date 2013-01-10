using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.ViewModel
{
    public class InlineDialogPage_ConvertingCardsVM : InlineDialogPageVM
    {
        public override bool CancelButtonVisible
        { get { return false; } }

        public override bool OkButtonVisible
        { get { return false; } }

        public override string Title
        {
            get { return "Converting Cards"; }
        }
    }
}
