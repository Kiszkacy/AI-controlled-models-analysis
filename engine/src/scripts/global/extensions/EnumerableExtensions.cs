using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static void NeatPrint(this IEnumerable enumerable)
    {
        NeatPrinter printer = NeatPrinter.Start().Print("[\n");
        foreach (var item in enumerable)
        {
            printer.Print("\t").Print((item ?? "null").ToString()).Print(",\n");
        }
        printer.Print("]").End();
    }
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}