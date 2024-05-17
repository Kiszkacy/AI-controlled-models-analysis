using System;

/// <summary>
/// The <c>TestBeforeAttribute</c> specifies that this test must run before another class tests or a group of tests.
/// </summary>
/// <remarks>
/// The argument passed to <c>TestBeforeAttribute</c> constructor should be the name of a test class type without the "Test" prefix.
/// For example, if the test class is named <c>TestMyClass</c>, then the argument should be <c>MyClass</c>.
/// </remarks>
public class TestBeforeAttribute : Attribute
{
    public string[] Before { get; }

    public TestBeforeAttribute(params string[] before)
    {
        this.Before = before;
    }
}