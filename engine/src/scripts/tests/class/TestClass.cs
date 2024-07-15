
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public abstract class TestClass<T> : Singleton<T> where T : TestClass<T>
{
    public void Run()
    {
        foreach (MethodInfo method in typeof(T).GetMethods())
        {
            bool isTestMethod = method.GetCustomAttributes(typeof(TestAttribute), false).Any();
            if (!isTestMethod) continue;

            bool isSlowTest = method.GetCustomAttributes(typeof(TestSlowAttribute), false).Any();
            if (!Config.Get().Tests.RunSlowTests && isSlowTest) continue;

            try
            {
                method.Invoke(this, null);
                this.LogPassed($"{typeof(T).FullName} | {method.Name}");
            }
            catch (TargetInvocationException exception)
            {
                bool isUncertainTest = method.GetCustomAttributes(typeof(TestUncertainAttribute), false).Any();
                if (Config.Get().Tests.PassUncertainTestsWhenFailed && isUncertainTest)
                    this.LogWarning($"{typeof(T).FullName} | {method.Name} :: [TestUncertain] {exception.GetBaseException().Message}");
                else
                    this.LogFailed($"{typeof(T).FullName} | {method.Name} :: {exception.GetBaseException().Message}");
            }
        }
    }

    public List<MethodInfo> GetTests() => typeof(T).GetMethods().Where(method => method.GetCustomAttributes(typeof(TestAttribute), false).Any()).ToList();

    public int GetTestCount() => this.GetTests().Count;

    public List<MethodInfo> GetSuitableTests()
    {
        return this.GetTests().Where(method => Config.Get().Tests.RunSlowTests || !method.GetCustomAttributes(typeof(TestSlowAttribute), false).Any()).ToList();
    }

    public int GetSuitableTestCount() => this.GetSuitableTests().Count;

    protected void LogPrint(string message)
    {
        TestRunner.Get().LogInfo("LOGS : " + message);
    }

    private void LogPassed(string message)
    {
        TestRunner.Get().LogResult(TestResult.Passed, message);
    }

    private void LogWarning(string message)
    {
        TestRunner.Get().LogResult(TestResult.Warning, message);
    }

    private void LogFailed(string message)
    {
        TestRunner.Get().LogResult(TestResult.Failed, message);
    }
}