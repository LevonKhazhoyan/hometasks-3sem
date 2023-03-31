namespace MyNUnit.MyNUnitImpl;

using MyNUnitAttributes.Attributes;
using System.Reflection;
using System.Diagnostics;

/// <summary>
/// MyNUnit testing class
/// </summary>
public class MyNUnitClass
{
    public List<TestResult> ResultList { get; } = new();

    /// <summary>
    /// Runs tests in the given directory
    /// </summary>
    /// <param name="path">Directory path</param>
    public void Start(string path)
    {
        var paths = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        var classes = paths.Select(Assembly.LoadFrom)
            .SelectMany(a => a.ExportedTypes)
            .Where(t => t.IsClass);
        var testClasses = classes.Where(c => c.GetMethods().Any(m => m.GetCustomAttributes().Any(a => a is Test)));
        Parallel.ForEach(testClasses, StartTests);
    }

    /// <summary>
    /// Prints report of the testing
    /// </summary>
    public void PrintReport()
    {
        var distinctResult = ResultList.DistinctBy(test => test.Name);
        foreach (var test in distinctResult)
        {
            switch (test.Result)
            {
                case ResultState.Failed:
                    break;
                case ResultState.Passed:
                    Console.WriteLine($"Test named: \"{test.Name}\" {test.Result}. Total time of test session: {test.ElapsedTime.TotalMilliseconds} ms");
                    break;
                case ResultState.Ignored:
                    Console.WriteLine($"Test name: \"{test.Name}\" Ignored. Reason: {test.IgnoreReason}. Total time of test session: {test.ElapsedTime.TotalMilliseconds} ms");
                    break;
            }
        }
    }

    /// <summary>
    /// Runs given tests
    /// </summary>
    private void StartTests(Type testClass)
    {
        var methods = SortByAttributes(testClass);
        if (!RunMethods(methods.BeforeClass, methods.Test))
        {
            return;
        }

        Parallel.ForEach(methods.Test, test => ExecuteTest(methods, testClass, test));

        RunMethods(methods.AfterClass, methods.Test);
    }

    private void ExecuteTest(MethodsList methods, Type testClass, MethodInfo test)
    {
        if (!RunBeforeAfterTestMethod(methods.Before, test))
        {
            ResultList.Add(new TestResult(test.Name, ResultState.Ignored, TimeSpan.Zero, "Before test method failed"));
            return;
        }

        var result = RunTest(test, testClass);
        if (result.Result is ResultState.Ignored or ResultState.Failed)
        {
            ResultList.Add(result);
            return;
        }

        ResultList.Add(!RunBeforeAfterTestMethod(methods.After, test)
            ? new TestResult(test.Name, ResultState.Failed, TimeSpan.Zero, null)
            : result);
    }

    /// <summary>
    /// Runs methods from AfterClass or BeforeClass 
    /// </summary>
    /// <param name="methods">Before or After class methods</param>
    /// <param name="tests">Test methods</param>
    /// <returns>Whether it's failed</returns>
    private bool RunMethods(List<MethodInfo> methods, List<MethodInfo> tests)
    {
        var result = true;
        foreach (var method in methods)
        {
            try
            {
                method.Invoke(null, null);
            }
            catch (TargetInvocationException)
            {
                foreach (var test in tests)
                {
                    ResultList.Add(new TestResult(test.Name, ResultState.Ignored, TimeSpan.Zero, "Before or After class method failed"));
                }
                result = false;
            }
        }
        return result;
    }

    /// <summary>
    /// Runs test
    /// </summary>
    /// <param name="method">Test method</param>
    /// <param name="testClass">Test method class</param>
    private static TestResult RunTest(MethodInfo method, Type testClass)
    {
        var properties = (Test?)method.GetCustomAttribute(typeof(Test));
        if (properties?.Ignore != null)
        {
            return new TestResult(method.Name, ResultState.Ignored, TimeSpan.Zero, properties.Ignore);
        }

        var testClassInstance = Activator.CreateInstance(testClass);
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            method.Invoke(testClassInstance, null);
            stopwatch.Stop();
            if (properties?.Expected != null)
            {
                return new TestResult(method.Name, ResultState.Failed, stopwatch.Elapsed, null);
            }
        }
        catch (TargetInvocationException e)
        {
            stopwatch.Stop();
            if (properties?.Expected != null && e.InnerException?.GetType() != properties.Expected || properties?.Expected == null)
            {
                return new TestResult(method.Name, ResultState.Failed, stopwatch.Elapsed, null);
            }
        }
        return new TestResult(method.Name, ResultState.Passed, stopwatch.Elapsed, null);
    }

    private static bool RunBeforeAfterTestMethod(List<MethodInfo> methods, MethodInfo test)
    {
        foreach (var method in methods)
        {
            try
            {
                method.Invoke(test, null);
            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Sorts methods by attributes
    /// </summary>
    /// <returns>Class containing lists</returns>
    private static MethodsList SortByAttributes(Type testClass)
    {
        var methodsList = new MethodsList();
        foreach (var method in testClass.GetMethods())
        {
            foreach (var attribute in Attribute.GetCustomAttributes(method))
            {
                if (method.GetParameters().Length != 0)
                {
                    throw new InvalidOperationException("Test methods shouldn't have any parameters");
                }
                switch (attribute)
                {
                    case Test:
                        if (method.ReturnType != typeof(void))
                        {
                            throw new InvalidOperationException("Test methods return type should be void");
                        }
                        methodsList.Test.Add(method);
                        break;
                    case After:
                        if (method.ReturnType != typeof(void))
                        {
                            throw new InvalidOperationException("After test methods return type should be void");
                        }
                        methodsList.After.Add(method);
                        break;
                    case Before:
                        if (method.ReturnType != typeof(void))
                        {
                            throw new InvalidOperationException("Before test methods return type should be void");
                        }
                        methodsList.Before.Add(method);
                        break;
                    case AfterClass:
                        if (!method.IsStatic)
                        {
                            throw new InvalidOperationException("After class methods should be static");
                        }
                        methodsList.AfterClass.Add(method);
                        break;
                    case BeforeClass:
                        if (!method.IsStatic)
                        {
                            throw new InvalidOperationException("Before class methods should be static");
                        }
                        methodsList.BeforeClass.Add(method);
                        break;
                }
            }
        }
        return methodsList;
    }
}