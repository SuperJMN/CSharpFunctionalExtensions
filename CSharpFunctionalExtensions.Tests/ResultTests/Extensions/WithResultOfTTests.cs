using FluentAssertions;
using Xunit;

namespace CSharpFunctionalExtensions.Tests.ResultTests.Extensions
{
    public class WithResultOfTTests
    {
        [Fact]
        public void Success_should_combine()
        {
            var a = Result.Success(2);
            var b = Result.Success(3);

            var r = a.With(b, (x, y) => x + y);

            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be(5);
        }

        [Fact]
        public void Failures_should_combine()
        {
            var a = Result.Failure<int>("Error 1");
            var b = Result.Failure<int>("Error 2");

            var r = a.With(b, (x, y) => x + y);

            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("Error 1, Error 2");
        }

        [Fact]
        public void Failure_in_first_should_fail_with_first_message()
        {
            var a = Result.Failure<int>("Error 1");
            var b = Result.Success(2);

            var r = a.With(b, (x, y) => x + y);

            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("Error 1");
        }

        [Fact]
        public void Failure_in_second_should_fail_with_second_message()
        {
            var a = Result.Success(2);
            var b = Result.Failure<int>("Error 2");

            var r = a.With(b, (x, y) => x + y);

            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("Error 2");
        }
    }
}