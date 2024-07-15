using System.Collections;

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
}