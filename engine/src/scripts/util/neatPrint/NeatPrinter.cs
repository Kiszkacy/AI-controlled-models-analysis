using System;

public class NeatPrinter
{
    public static NeatPrinter Start() => new();

    public NeatPrinter Print(string text)
    {
        NeatPrint.PrintRaw(text);
        return this;
    }

    public NeatPrinter SetColor(ConsoleColor color)
    {
        NeatPrint.SetColor(color);
        return this;
    }

    public NeatPrinter ResetColor()
    {
        NeatPrint.ResetColor();
        return this;
    }

    public NeatPrinter ColorPrint(ConsoleColor color, string text)
    {
        NeatPrint.ColorPrintRaw(color, text);
        return this;
    }

    public void End() => NeatPrint.Print();

    public NeatPrinter NewLine()
    {
        NeatPrint.Print();
        return this;
    }

    private NeatPrinter() { }
}