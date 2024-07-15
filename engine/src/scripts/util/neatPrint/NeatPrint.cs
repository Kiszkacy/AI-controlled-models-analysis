using System;

using Godot;

public static class NeatPrint
{
    public static void Print(string text = null) => GD.Print(text);

    public static void PrintRaw(string text = null) => GD.PrintRaw(text);

    public static void SetColor(ConsoleColor color) => Console.ForegroundColor = color;

    public static void ResetColor() => Console.ResetColor();

    public static void ColorPrint(ConsoleColor color, string text = null)
    {
        SetColor(color);
        Print(text);
        ResetColor();
    }

    public static void ColorPrintRaw(ConsoleColor color, string text = null)
    {
        SetColor(color);
        PrintRaw(text);
        ResetColor();
    }
}