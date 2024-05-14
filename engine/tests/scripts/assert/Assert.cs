using System;
using System.Linq;
using Godot;

public static class Assert
{
    public static void Equals<T>(T compared, T expected, string message = "Objects are not equal! Expected {0}, received {1}.")
    {
        if (!expected.Equals(compared)) throw new TestException(String.Format(message, expected, compared));
    }
    
    public static void NotEquals<T>(T compared, T expected, string message = "Objects are equal when they should not! Should not equal {0}, but got {1}!")
    {
        if (expected.Equals(compared)) throw new TestException(String.Format(message, expected, compared));
    }

    public static void NearlyEquals(double compared, double expected, double lambda = 1.0e-5, string message = "Numbers are not closely equal! Expected {0} +/- {2}, received {1}.")
    {
        if (Mathf.Abs(expected - compared) > lambda) throw new TestException(String.Format(message, expected, compared, lambda));
    }
    
    public static void IsTrue(bool expression, string message = "Expression is not true! Expected {0}, received {1}.")
    {
        if (expression != true) throw new TestException(String.Format(message, true, expression));
    }
    
    public static void IsFalse(bool expression, string message = "Expression is not false! Expected {0}, received {1}.")
    {
        if (expression != false) throw new TestException(String.Format(message, false, expression));
    }
}