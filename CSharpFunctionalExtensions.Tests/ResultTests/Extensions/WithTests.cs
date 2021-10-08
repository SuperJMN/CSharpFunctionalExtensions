using Xunit;

namespace CSharpFunctionalExtensions.Tests.ResultTests.Extensions
{
    public class WithTests
    {
        [Fact]
        public void Both_results_successful_return_value_from_selector()
        {
            var left = Result.Success(new T1());
            var right = Result.Success(new T2());
            var expected = new E();
            var actual = left.With(right, (l, r) => expected);
            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void Failed_left_returns_a_failure()
        {
            var left = Result.Failure<T1>("First result failed");
            var right = Result.Success(new T2());
            var e = new E();
            var result = left.With(right, (l, r) => e);
            result.IsFailure.Should().BeTrue("The result should be a failure");
            result.Error.Should().Be("First result failed");
        }

        [Fact]
        public void Failed_right_returns_a_failure()
        {
            var left = Result.Success(new T1());
            var right = Result.Failure<T2>("Second result failed");
            var expected = new E();
            var actual = left.With(right, (l, r) => expected);
            actual.IsFailure.Should().BeTrue("The result should be a failure");
            actual.Error.Should().Be("Second result failed");
        }

        [Fact]
        public void Both_results_fail_error_combination_is_returned()
        {
            var left = Result.Failure<T1>("First result failed");
            var right = Result.Failure<T2>("Second result failed");
            var expected = new E();
            var actual = left.With(right, (l, r) => expected);
            actual.Value.Should().Be("First result failed" + Result.ErrorMessagesSeparator + "Second result failed");
        }

        private class T1
        {
        }

        private class T2
        {
        }

        private class E
        {
        }
    }

    public static class WithExtensions
    {
    }
}