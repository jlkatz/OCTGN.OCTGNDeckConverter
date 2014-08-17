using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCTGNDeckConverterTest
{
    public class ExpectedDeckSection
    {
        public ExpectedDeckSection(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new ArgumentException("sectionName cannot be null or blank");
            }

            this.SectionName = sectionName;
        }

        public string SectionName
        {
            get;
            private set;
        }

        public int TotalCardCount
        {
            get;
            set;
        }

        public int UniqueCardCount
        {
            get;
            set;
        }
    }
}
