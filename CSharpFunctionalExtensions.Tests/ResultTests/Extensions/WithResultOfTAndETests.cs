using FluentAssertions;
using Xunit;

namespace CSharpFunctionalExtensions.Tests.ResultTests.Extensions
{
    public class WithResultOfTAndETests
    {
        [Fact]
        public void Success_should_combine()
        {
            var a = Result.Success<int, string>(2);
            var b = Result.Success<int, string>(3);

            var r = a.With(b, (x, y) => x + y, (e1, e2) => "both failed");

            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be(5);
        }

        [Fact]
        public void Failures_should_combine()
        {
            var a = Result.Failure<int, string>("Error 1");
            var b = Result.Failure<int, string>("Error 2");

            var r = a.With(b, (x, y) => x + y, (e1, e2) => "both failed");

            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("both failed");
        }

        [Fact]
        public void Failure_in_first_should_fail_with_first_message()
        {
            var a = Result.Failure<int, string>("Error 1");
            var b = Result.Success<int, string>(2);

            var r = a.With(b, (x, y) => x + y, (e1, e2) => "both failed");

            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("Error 1");
        }

        [Fact]
        public void Failure_in_second_should_fail_with_second_message()
        {
            var a = Result.Success<int, string>(2);
            var b = Result.Failure<int, string>("Error 2");

            var r = a.With(b, (x, y) => x + y, (e1, e2) => "both failed");

            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("Error 2");
        }
    }
}