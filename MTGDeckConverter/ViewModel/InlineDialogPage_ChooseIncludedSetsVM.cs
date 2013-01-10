namespace MTGDeckConverter.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MTGDeckConverter.Model;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InlineDialogPage_ChooseIncludedSetsVM : InlineDialogPageVM
    {
        public List<ConverterSet> Sets
        {
            get
            {
                return (
                    from set in ConverterDatabase.SingletonInstance.Sets.Values
                    orderby set.MaxMultiverseID descending
                    select set
                    ).ToList();
            }
        }

        public override bool CancelButtonVisible
        { get { return false; } }

        public override string Title
        {
            get { return "Choose Included Sets When Searching for Cards"; }
        }
    }
}
