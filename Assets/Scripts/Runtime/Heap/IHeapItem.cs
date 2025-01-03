using System;

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}
