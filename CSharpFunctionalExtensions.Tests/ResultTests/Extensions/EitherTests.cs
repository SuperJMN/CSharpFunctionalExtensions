﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CSharpFunctionalExtensions.Tests.ResultTests.Extensions
{
    public class EitherTests
    {
        private class Error
        {
            public string Message { get; }

            public Error(string message)
            {
                Message = message;
            }
        }

        public class ErrorList : List<string>
        {
            public ErrorList(IEnumerable<string> items) : base(items)
            {
            }

            public ErrorList(string item) : this(new[] { item })
            {
            }
        }

        [Fact]
        public void When_mapping_success_to_same_type_successful_value_is_returned()
        {
            var result = Either.Success<Error, int>(2);
            var a = result
                .MapRight1(i => 5 * i)
                .Handle(error => -1);

            a.Should().Be(10);
        }

        [Fact]
        public void When_mapping_success_to_another_type_failure_value_is_returned()
        {
            var result = Either.Failure<Error, int>(new Error("error"));
            var a = result
                .MapRight1(i => i.ToString())
                .Handle(error => error.Message);

            a.Should().Be("error");
        }

        [Fact]
        public void When_mapping_failure_to_same_type_failure_value_is_returned()
        {
            var result = Either.Failure<Error, int>(new Error("error"));
            var a = result
                .MapRight1(i => 5 * i)
                .Handle(error => -1);

            a.Should().Be(-1);
        }

        [Fact]
        public void When_mapping_failure_to_another_type_successful_value_is_returned()
        {
            var result = Either.Success<Error, int>(2);
            var a = result
                .MapRight1(i => i.ToString())
                .Handle(error => error.Message);

            a.Should().Be("2");
        }

        [Theory]
        [InlineData("af", "bs", "af")]
        [InlineData("as", "bf", "bf")]
        [InlineData("af", "bf", "af,bf")]
        [InlineData("as", "bs", "asbs")]
        public void Verify(string a, string b, string expected)
        {
            var ea = Emit(a);
            var eb = Emit(b);

            var ec = ea.Combine<ErrorList, string>(eb, (x, y) => Either.Success<ErrorList, string>(x + y), (e1, e2) => new ErrorList(e1.Concat(e2)));

            var r = ec.Handle(list => string.Join(",", list));
            r.Should().Be(expected);
        }

        private Either<string, ErrorList> Emit(string token)
        {
            if (token.EndsWith("s"))
            {
                return Either.Success<ErrorList, string>(token);
            }

            return Either.Failure<ErrorList, string>(new ErrorList(token));
        }
    }
}