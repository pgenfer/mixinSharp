using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// a mixin class that has an indexer property
    /// </summary>
    public class CollectionWithIndexer
    {
        public string this[int index]
        {
            get
            {
                return index.ToString();
            }
            set
            {
                // do nothing here
            }
        }
    }
}
