using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpFunctionalExtensions.Tests.ResultTests.Extensions
{
    public class WithResultOfTAndETests
    {
        [Theory]
        [InlineData("af", "bs", "af")]
        [InlineData("as", "bf", "bf")]
        [InlineData("af", "bf", "af,bf")]
        [InlineData("as", "bs", "asbs")]
        public void Verify(string a, string b, string expected)
        {
            var ea = Emit(a);
            var eb = Emit(b);

            var ec = ea.With<string, ErrorList>(eb, (x, y) => Result.Success<string, ErrorList>(x + y), (e1, e2) => new ErrorList(e1.Concat(e2)));

            var r = ec.Handle(list => string.Join(",", list));
            r.Should().Be(expected);
        }

        private Result<string, ErrorList> Emit(string token)
        {
            if (token.EndsWith("s"))
            {
                return Result.Success<string, ErrorList>(token);
            }

            return Result.Failure<string, ErrorList>(new ErrorList(token));
        }

        private class ErrorList : List<string>
        {
            public ErrorList(IEnumerable<string> items) : base(items)
            {
            }

            public ErrorList(string item) : this(new[] { item })
            {
            }
        }

        [Fact]
        public void Failures_should_combine()
        {
            var a = Result.Failure<int, string>("Error 1");
            var b = Result.Failure<int, string>("Error 2");
            var r = a.With(b, (x, y) => x + y, (s, s1) => "combined");
            r.IsFailure.Should().BeTrue();
            r.Error.Should().Be("combined");
        }
    }
}