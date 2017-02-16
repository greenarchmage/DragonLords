using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utility
{
  public class PriorityQueueMin<T> : IEnumerable<T> where T : IComparable<T>
  {
    private T[] priorityHeap;
    public int Count { get; private set; }

    #region Constructors
    public PriorityQueueMin()
    {
      priorityHeap = new T[1];
      Count = 0;
    }
    public PriorityQueueMin(int size)
    {
      priorityHeap = new T[size + 1];
      Count = 0;
    }

    public PriorityQueueMin(T[] keys)
    {
      Count = keys.Length;
      priorityHeap = new T[keys.Length + 1];
      for (int i = 0; i < Count; i++)
      { priorityHeap[i + 1] = keys[i]; }
      for (int k = Count / 2; k >= 1; k--)
      { sink(k); }

      // isMinHeap(); check if min heap?
    }

    #endregion
    public bool IsEmpty() { return Count == 0; }

    public void Insert(T x)
    {
      // double size of array if necessary
      if (Count == priorityHeap.Length - 1) resize(2 * priorityHeap.Length);

      // add x, and percolate it up to maintain heap invariant
      priorityHeap[++Count] = x;
      swim(Count);
    }

    public void insertRange(List<T> list)
    {
      foreach (T ele in list)
      {
        Insert(ele);
      }
    }

    public T Min()
    {
      return priorityHeap[1];
    }

    private void resize(int capacity)
    {
      //capacity > n;
      T[] temp = new T[capacity];
      for (int i = 1; i <= Count; i++)
      {
        temp[i] = priorityHeap[i];
      }
      priorityHeap = temp;
    }

    public T DelMin()
    {
      T min = priorityHeap[1];
      exch(1, Count--);
      sink(1);
      priorityHeap[Count + 1] = default(T);         // avoid loitering and help with garbage collection
      if ((Count > 0) && (Count == (priorityHeap.Length - 1) / 4)) resize(priorityHeap.Length / 2);
      //assert isMinHeap();
      return min;
    }
    /***************************************************************************
     * Helper functions.
     ***************************************************************************/
    private bool greater(int i, int j)
    {
      return priorityHeap[i].CompareTo(priorityHeap[j]) > 0;
    }

    private void exch(int i, int j)
    {
      T swap = priorityHeap[i];
      priorityHeap[i] = priorityHeap[j];
      priorityHeap[j] = swap;
    }

    /***************************************************************************
     * Heap functions
     ***************************************************************************/
    /// <summary>
    /// Restructures the heap if the child element has become higher than its parent
    /// </summary>
    /// <param name="k">index of priority queue object</param>
    private void swim(int k)
    {
      while (k > 1 && greater(k / 2, k))
      {
        exch(k, k / 2);
        k = k / 2;
      }
    }

    private void sink(int k)
    {
      while (2 * k <= Count)
      {
        int j = 2 * k;
        if (j < Count && greater(j, j + 1)) j++;
        if (!greater(k, j)) break;
        exch(k, j);
        k = j;
      }
    }

    // is pq[1..N] a min heap?
    private bool isMinHeap()
    {
      return isMinHeap(1);
    }

    // is subtree of pq[1..n] rooted at k a min heap?
    private bool isMinHeap(int k)
    {
      if (k > Count) return true;
      int left = 2 * k;
      int right = 2 * k + 1;
      if (left <= Count && greater(k, left)) return false;
      if (right <= Count && greater(k, right)) return false;
      return isMinHeap(left) && isMinHeap(right);
    }

    // Enumerator functions
    public IEnumerator<T> GetEnumerator()
    {
      PriorityQueueMin<T> copy = new PriorityQueueMin<T>();
      for (int i = 0; i < Count; i++)
      {
        copy.Insert(priorityHeap[i + 1]);
      }
      while (!copy.IsEmpty())
      {
        yield return copy.DelMin();
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Contains(T val)
    {
      return priorityHeap.Contains(val);
    }

    public void Clear()
    {
      priorityHeap = new T[1];
      Count = 0;
    }
  }
}
