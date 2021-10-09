using System;

namespace CSharpFunctionalExtensions
{
    public partial class ResultExtensions
    {
        public static Result<T, K> BindError<T, K, E>(this Result<T, E> self,
            Func<E, Result<T, K>> map)
        {
            if (self.IsFailure)
            {
                return map(self.Error);
            }

            return Result.Success<T, K>(self.Value);
        }
    }
}