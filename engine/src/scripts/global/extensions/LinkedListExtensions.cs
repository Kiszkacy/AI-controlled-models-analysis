
using System.Collections.Generic;

public static class LinkedListExtensions
{
    public static void Extend<T>(this LinkedList<T> first, LinkedList<T> second)
    {
        foreach (var entry in second)
        {
            first.AddLast(entry);
        }
    }
}