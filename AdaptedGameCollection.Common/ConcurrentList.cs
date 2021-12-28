using System.Collections;
using System.Collections.Generic;

namespace AdaptedGameCollection.Common
{
    public class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly List<T> _list;

        public ConcurrentList()
        {
            _list = new List<T>();
        }

        public void Add(T item)
        {
            lock (_list)
            {
                _list.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (_list)
            {
                _list.Remove(item);
            }
        }

        public bool Contains(T item)
        {
            lock (_list)
            {
                return _list.Contains(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_list)
            {
                foreach (T item in _list)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}