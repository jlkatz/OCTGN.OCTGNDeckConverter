using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace MTGDeckConverter.ViewModel
{
    public abstract class ImportDeckWizardPageVM : ViewModelBase
    {
        public ImportDeckWizardPageVM(ImportDeckWizardVM importDeckWizardVM)
        {
            if (importDeckWizardVM == null)
            {
                throw new ArgumentNullException();
            }
            this.ImportDeckWizardVM = importDeckWizardVM;
        }

        public ImportDeckWizardVM ImportDeckWizardVM
        {
            get;
            private set;
        }

        internal const string CanMoveToNextStepPropertyName = "CanMoveToNextStep";
        private bool _CanMoveToNextStep = true;
        public bool CanMoveToNextStep
        {
            get { return _CanMoveToNextStep; }
            protected set { this.SetValue(ref _CanMoveToNextStep, value, CanMoveToNextStepPropertyName); }
        }

        public abstract bool ShowNextStepCommand { get; }
        public abstract bool ShowStartOverCommand { get; }
        public virtual string Title { get { return ""; } }

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
    }
}
