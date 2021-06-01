using System.Collections.Generic;
using System.Linq;

namespace uwpPlatformer.Utilities
{
    public class HistoryStack<T>
    {
        private LinkedList<T> items = new LinkedList<T>();

        public HistoryStack(int capacity)
        {
            Capacity = capacity;
        }

        public List<T> Items => items.ToList();
        public int Capacity { get; }

        public void Push(T item)
        {
            if (items.Count == Capacity)
            {
                items.RemoveFirst();
            }

            items.AddLast(item);
        }

        public T Pop()
        {
            if (items.Count == 0) return default;

            var lastItem = items.Last;
            items.RemoveLast();
            return lastItem == null ? default : lastItem.Value;
        }
    }
}