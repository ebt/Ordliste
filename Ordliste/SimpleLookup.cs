using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ordliste
{
    [Serializable]
    public class SimpleLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly IDictionary<TKey, HashSet<TElement>> dictionary = new ConcurrentDictionary<TKey, HashSet<TElement>>();

        public int Count
        {
            get { return this.dictionary.Keys.Count; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get { return this.dictionary[key]; }
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            var enumerable = this.dictionary.Keys.Select(k => new Grouping<TKey, TElement>(k, this.dictionary[k]));
            return enumerable.GetEnumerator();
        }

        private readonly object dictionaryLockObj = new object();
        public void Add(TKey key, TElement element)
        {
            lock (this.dictionaryLockObj)
            {
                if (this.dictionary.ContainsKey(key))
                {
                    this.dictionary[key].Add(element);
                }
                else
                {
                    this.dictionary.Add(key, new HashSet<TElement> { element });
                }
            }
        }

        public void Add(TKey key, params TElement[] elements)
        {
            foreach (var element in elements)
            {
                this.Add(key, element);
            }
        }

        public bool Contains(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        public void Remove(TKey key, TElement element)
        {
            lock (this.dictionaryLockObj)
            {
                if (this.dictionary.ContainsKey(key))
                    this.dictionary[key].Remove(element);
            }
        }

        public void Remove(TKey key)
        {
            lock (this.dictionaryLockObj)
            {
                if (this.dictionary.ContainsKey(key))
                    this.dictionary.Remove(key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}