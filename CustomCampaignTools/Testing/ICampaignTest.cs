#if DEBUG
using System;
using System.Diagnostics;

namespace CustomCampaignTools.Testing;

public interface ICampaignTest
{
    string Name { get; }
    TestResult Run();
}

public abstract class CampaignTest : ICampaignTest
{
    public abstract string Name { get; }

    public TestResult Run()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Execute();
            stopwatch.Stop();

            return new TestResult
            {
                TestName = Name,
                Passed = true,
                DurationMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return new TestResult
            {
                TestName = Name,
                Passed = false,
                Message = ex.Message,
                Exception = ex,
                DurationMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }
    }

    protected abstract void Execute();
}

public class TestResult
{
    public string TestName;
    public bool Passed;
    public string Message;
    public Exception Exception;
    public double DurationMs;

    public static TestResult Pass(string testName, string message = "")
    {
        return new TestResult
        {
            TestName = testName,
            Passed = true,
            Message = message
        };
    }

    public static TestResult Fail(string testName, string message, Exception exception = null)
    {
        return new TestResult
        {
            TestName = testName,
            Passed = false,
            Message = message,
            Exception = exception
        };
    }
}

public static class TestAssert
{
    public static void IsTrue(bool condition, string message = "Expected condition to be true.")
    {
        if (!condition)
            throw new TestAssertionException(message);
    }

    public static void IsFalse(bool condition, string message = "Expected condition to be false.")
    {
        if (condition)
            throw new TestAssertionException(message);
    }

    public static void AreEqual<T>(T expected, T actual, string message = null)
    {
        if (!Equals(expected, actual))
        {
            throw new TestAssertionException(
                message ?? $"Expected <{expected}>, but got <{actual}>.");
        }
    }

    public static void IsNotNull(object value, string message = "Expected value to not be null.")
    {
        if (value == null)
            throw new TestAssertionException(message);
    }

    public static void Throws<TException>(Action action, string message = null)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        catch (Exception ex)
        {
            throw new TestAssertionException(
                message ?? $"Expected exception {typeof(TException).Name}, but got {ex.GetType().Name}.");
        }

        throw new TestAssertionException(
            message ?? $"Expected exception {typeof(TException).Name}, but no exception was thrown.");
    }
}

public class TestAssertionException : Exception
{
    public TestAssertionException(string message) : base(message) { }
}
#endif