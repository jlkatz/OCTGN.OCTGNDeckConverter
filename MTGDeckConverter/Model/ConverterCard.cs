using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGDeckConverter.Model
{
    public class ConverterCard
    {
        public ConverterCard(Guid cardID, string name, string set, int multiverseID)
        {
            if (name == null) { throw new ArgumentNullException(); }
            this.CardID = cardID;
            this.Name = name;
            this.Set = set;
            this.MultiverseID = multiverseID;
        }

        public Guid CardID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Set
        {
            get;
            private set;
        }

        public int MultiverseID
        {
            get;
            private set;
        }
    }
}
