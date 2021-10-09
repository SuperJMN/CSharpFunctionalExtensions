using System;

namespace CSharpFunctionalExtensions
{
    public partial class ResultExtensions
    {
        public static Result<K, E> BindError<T, K, E>(this Result<T, E> self,
            Func<T, Result<K, E>> map)
        {
            return self.Value().Match(
                right => map(right),
                () => Result.Failure<K, E>(self.Error().GetValueOrDefault()));
        }
    }
}