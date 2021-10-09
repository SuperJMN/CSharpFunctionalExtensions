using System;

namespace CSharpFunctionalExtensions
{
    public partial class ResultExtensions
    {
        public static Result<T, E2> BindError<T, E, E2>(this Result<T, E> self,
            Func<E, Result<T, E2>> map)
        {
            if (self.IsSuccess)
            {
                return Result.Success<T, E2>(self.Value);
            }

            return map(self.Error);
        }

        public static Result<T> BindError<T>(this Result<T> self,
            Func<string, Result<T>> map)
        {
            if (self.IsSuccess)
            {
                return Result.Success(self.Value);
            }

            return map(self.Error);
        }
    }
}