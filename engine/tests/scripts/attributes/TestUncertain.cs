
/// <summary>
/// Use the <c>TestUncertainAttribute</c> to label tests that are non-deterministic and may fail in certain edge cases. 
/// </summary>
/// <remarks>
/// This tag should be used sparingly, aim to minimize non-deterministic behavior in tests.
/// </remarks>
public class TestUncertainAttribute : TestAttribute { }