using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace AutomationExercise.Tests.Infrastructure
{
    /// <summary>
    /// Runs a test up to <c>tryCount</c> times, stopping at the first attempt that does not fail.
    ///
    /// NUnit's own <c>[Retry]</c> repeats a test only when an assertion fails. A Playwright timeout
    /// is an exception, which NUnit reports as an error rather than a failure, so <c>[Retry]</c>
    /// never ran a second attempt for the one thing this suite needs it for: a public demo site
    /// that is slow to answer. This attribute repeats on both.
    ///
    /// SetUp and TearDown run again for every attempt, so each one gets a fresh browser context
    /// and a failed attempt still leaves its own screenshot and trace behind.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RetryOnErrorAttribute : NUnitAttribute, IRepeatTest
    {
        private readonly int _tryCount;

        public RetryOnErrorAttribute(int tryCount)
        {
            _tryCount = tryCount;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new RetryOnErrorCommand(command, _tryCount);
        }

        private sealed class RetryOnErrorCommand : DelegatingTestCommand
        {
            private readonly int _tryCount;

            public RetryOnErrorCommand(TestCommand innerCommand, int tryCount)
                : base(innerCommand)
            {
                _tryCount = tryCount;
            }

            public override TestResult Execute(TestExecutionContext context)
            {
                var attemptsLeft = _tryCount;

                while (attemptsLeft-- > 0)
                {
                    try
                    {
                        context.CurrentResult = innerCommand.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        context.CurrentResult ??= context.CurrentTest.MakeTestResult();
                        context.CurrentResult.RecordException(ex);
                    }

                    // Failed covers both an assertion failure and an unexpected exception.
                    if (context.CurrentResult.ResultState.Status != TestStatus.Failed)
                    {
                        break;
                    }

                    if (attemptsLeft > 0)
                    {
                        TestLog.Warn($"Attempt {context.CurrentRepeatCount + 1} of {_tryCount} failed: {context.CurrentResult.Message}");
                        context.CurrentResult = context.CurrentTest.MakeTestResult();
                        context.CurrentRepeatCount++;
                    }
                }

                return context.CurrentResult;
            }
        }
    }
}
