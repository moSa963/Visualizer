using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer.Algorithms
{
    public class PriorityQueue<T>
    {
        private LinkedList<T> Nodes { get; }
        private Func<T, T, int> Compare { get; }
        public int Count => Nodes.Count;

        public PriorityQueue(Func<T, T, int> Compare)
        {
            Nodes = new LinkedList<T>();
            this.Compare = Compare;
        }

        public PriorityQueue(IEnumerable<T> collection, Func<T, T, int> Compare)
        {
            Nodes = new LinkedList<T>();
            this.Compare = Compare;
            foreach (T node in collection)
            {
                Add(node);
            }
        }


        //add new item to the queue
        public void Add(T item)
        {
            for (LinkedListNode<T> c = Nodes.First; c != null; c = c.Next)
            {
                //Add the item before the lesser or equal priority
                if (Compare(item, c.Value) <= 0)
                {
                    _ = Nodes.AddBefore(c, item);
                    return;
                }
            }
            _ = Nodes.AddLast(item);
        }

        //return the item that match or default
        public T Find(Func<T, bool> match)
        {
            for (LinkedListNode<T> c = Nodes.First; c != null; c = c.Next)
            {
                if (match(c.Value))
                {
                    return c.Value;
                }
            }
            return default;
        }


        /// <param name="item">the item that changed</param>
        public void PriorityChanged(T item)
        {
            _ = Nodes.Remove(item);
            Add(item);
        }

        //get the first item in the queue
        public T First()
        {
            return Nodes.First.Value;
        }

        //get the last item in the queue
        public T Last()
        {
            return Nodes.Last.Value;
        }

        //get the first item in the queue and delete it
        public T PopFirst()
        {
            T first = First();
            Nodes.RemoveFirst();
            return first;
        }

        //get the last item in the queue and delete it
        public T PopLast()
        {
            var last = Last();
            Nodes.RemoveLast();
            return last;
        }

        //delete all items from the queue
        public void Clear()
        {
            Nodes.Clear();
        }
    }
}
