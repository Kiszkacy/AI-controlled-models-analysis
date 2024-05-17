
using System;

/// <summary>
/// The <c>TestAfterAttribute</c> specifies that this test must run after another class tests or a group of tests.
/// </summary>
/// <remarks>
/// The argument passed to <c>TestAfterAttribute</c> constructor should be the name of a test class type without the "Test" suffix.
/// For example, if the test class is named <c>MyClassTest</c>, then the argument should be <c>MyClass</c>.
/// </remarks>
public class TestAfterAttribute : Attribute
{
    public string[] After { get; }

    public TestAfterAttribute(params string[] after)
    {
        this.After = after;
    }
}