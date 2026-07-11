using NUnit.Framework;
using System;

namespace AutomationExercise.Tests.Infrastructure
{
    public static class TestLog
    {
        public static void Debug(string message)
        {
            TestContext.Out.WriteLine($"[DEBUG] {message}");
        }

        public static void Warn(string message)
        {
            TestContext.Out.WriteLine($"[WARN] {message}");
        }
    }
}
