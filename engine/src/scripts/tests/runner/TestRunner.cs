
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class TestRunner : Singleton<TestRunner>
{
    private readonly string testsPath = "./src/scripts";

    private int testNumber = 0; // test number iterator
    private int testsPassed = 0;
    private int testsWarnings = 0;
    private readonly TestLayerer testLayerer = new();

    public void LogResult(TestResult result, string message)
    {
        this.testNumber += 1;

        ConsoleColor resultColor;
        switch (result)
        {
            case TestResult.Passed:
                this.testsPassed += 1;
                resultColor = ConsoleColor.Green;
                break;
            case TestResult.Warning:
                this.testsPassed += 1;
                this.testsWarnings += 1;
                resultColor = ConsoleColor.Yellow;
                break;
            case TestResult.Failed:
                resultColor = ConsoleColor.Red;
                break;
            case TestResult.None:
                throw new Exception("Received invalid enum type after test has completed.");
            default:
                throw new Exception("Received unknown enum type after test has completed.");
        }

        NeatPrinter.Start()
            .Print($"TEST {this.testNumber:D3} | {DateTime.Now:HH:mm:ss.fff} | ")
            .ColorPrint(resultColor, Enum.GetName(typeof(TestResult), result)!.ToUpper())
            .Print($" | {message}")
            .End();
    }

    public void LogInfo(string message)
    {
        if (Config.Get().Tests.PrintAdditionalLogs)
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Blue, "[TESTS]")
                .Print($"[TESTS]  | {message}")
                .End();
    }

    public void Run()
    {
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[TESTS]")
            .Print("  | STARTING")
            .End();

        int testCount = this.GetTestCount();
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[TESTS]")
            .Print($"  | DETECTED {testCount} TESTS")
            .End();

        int suitableTestCount = this.GetSuitableTestCount();
        if (suitableTestCount != testCount)
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Blue, "[TESTS]")
                .Print($"  | REJECTED {testCount - suitableTestCount} TESTS DUE TO THE TEST RUN SETTINGS")
                .End();

        List<Type> testClasses = this.GetTestClasses();
        List<List<Type>> layers = this.testLayerer.GetTestLayers(testClasses);
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[TESTS]")
            .Print($"  | CREATED {layers.Count} TEST LAYERS OUT OF {testClasses.Count} TEST CLASSES")
            .End();

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[TESTS]")
            .Print("  | STARTING")
            .End();
        DateTime startTime = DateTime.Now;

        foreach (Type testClass in layers.SelectMany(layer => layer))
        {
            MethodInfo method = testClass.GetMethod("Run");
            method.Invoke(Activator.CreateInstance(testClass), Array.Empty<object>());
        }

        DateTime endTime = DateTime.Now;
        TimeSpan timeDifference = endTime - startTime;
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[TESTS]")
            .Print("  | PASSED (")
            .ColorPrint(this.testsPassed == this.testNumber ? ConsoleColor.Green : ConsoleColor.Red, $"{this.testsPassed}")
            .Print("/")
            .ColorPrint(ConsoleColor.Cyan, $"{this.testNumber}")
            .Print(") TESTS")
            .ColorPrint(ConsoleColor.Yellow, this.testsWarnings != 0 ? $" ({this.testsWarnings} warnings!)" : "")
            .Print($" | DONE IN {(timeDifference.TotalSeconds < 1 ? $"{timeDifference.Milliseconds}ms" : $"{(int)timeDifference.TotalSeconds}.{timeDifference.Milliseconds}")}")
            .End();
    }

    private int GetTestCount()
    {
        return GetTestFiles()
            .Select(file => Type.GetType(this.GetClassNameFromTestFilePath(file)))
            .Sum(testClass => (int)testClass!.GetMethod("GetTestCount")!.Invoke(Activator.CreateInstance(testClass), Array.Empty<object>())!);
    }

    private int GetSuitableTestCount()
    {
        return GetTestFiles()
            .Select(file => Type.GetType(this.GetClassNameFromTestFilePath(file)))
            .Sum(testClass => (int)testClass!.GetMethod("GetSuitableTestCount")!.Invoke(Activator.CreateInstance(testClass), Array.Empty<object>())!);
    }

    private string[] GetTestFiles()
    {
        return Directory.GetFiles(testsPath, "*", SearchOption.AllDirectories)
            .Where(fileName => fileName.ToLower().EndsWith(".test.cs"))
            .ToArray();
    }

    private string GetClassNameFromTestFilePath(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath).Replace(".", "", StringComparison.OrdinalIgnoreCase);
    }

    private List<Type> GetTestClasses() => this.GetTestFiles().Select(file => Type.GetType(this.GetClassNameFromTestFilePath(file))).ToList();
}