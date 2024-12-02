
public class Heap<T> where T : IHeapItem<T>
{
    private readonly T[] m_Items;
    public int Count { get; private set; }

    public Heap(int capacity)
    {
        m_Items = new T[capacity];
    }

    public void Push(T item)
    {
        if (Contains(item))
        {
            UpdateItem(item);
            return;
        }
        
        item.HeapIndex = Count;
        m_Items[Count] = item;
        Count++;
        SortUp(item);
    }
    
    public T Pop()
    {
        T firstItem = m_Items[0];
        m_Items[0] = m_Items[Count - 1];
        m_Items[0].HeapIndex = 0;
        Count--;
        SortDown(m_Items[0]);
        
        return firstItem;
    }

    public T Peek()
    {
        return m_Items[0];
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int leftIndex = item.HeapIndex * 2 + 1;
            int rightIndex = item.HeapIndex * 2 + 2;
            
            if (leftIndex < Count && item.CompareTo(m_Items[leftIndex]) > 0)
            {
                Swap(item, m_Items[leftIndex]);
            }
            else if (rightIndex < Count && item.CompareTo(m_Items[rightIndex]) > 0)
            {
                Swap(item, m_Items[rightIndex]);
            }
            else
            {
                break;
            }
        }
    }

    private void SortUp(T current)
    {
        while (true)
        {
            T parent = m_Items[(current.HeapIndex - 1) / 2];
            if (current.CompareTo(parent) < 0)
            {
                Swap(current, parent);
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(T current, T with)
    {
        m_Items[with.HeapIndex] = current;
        m_Items[current.HeapIndex] = with;

        int tempParentIndex = with.HeapIndex;
                
        with.HeapIndex = current.HeapIndex;
        current.HeapIndex = tempParentIndex;
    }

    private void UpdateItem(T item)
    {
        SortUp(item);
    }

    private bool Contains(T item)
    {
        return Equals(m_Items[item.HeapIndex], item);
    }
}
