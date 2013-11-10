using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ordliste
{
    /// <summary>
    /// Represents a collection of objects that have a common key.
    /// </summary>
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> elements;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            this.Key = key;
            this.elements = elements;
        }

        public TKey Key { get; private set; }

        #region IEnumerable<TElement> Members

        public IEnumerator<TElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}