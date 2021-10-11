using System;
using System.Threading.Tasks;

namespace CSharpFunctionalExtensions
{
    public static partial class ResultExtensions
    {
        public static Task<Result<TResult, E>> WithBind<T1, T2, E, TResult>(this Result<T1, E> a,
            Result<T2, E> b,
            Func<T1, T2, Task<Result<TResult, E>>> map, Func<E, E, E> combineError)
        {
            var mapSuccess =
                a.BindError(el1 => b
                        .MapError(el2 => combineError(el1, el2))
                        .Bind(_ => Result.Failure<T1, E>(el1)))
                    .Bind(x => b
                        .Bind(y => map(x, y))
                        .MapError(el => el));

            return mapSuccess;
        }

        public static Task<Result<TResult, E>> WithMap<T1, T2, E, TResult>(this Result<T1, E> a,
            Result<T2, E> b,
            Func<T1, T2, Task<TResult>> map, Func<E, E, E> combineError)
        {
            var mapSuccess =
                a.BindError(el1 => b
                        .MapError(el2 => combineError(el1, el2))
                        .Bind(_ => Result.Failure<T1, E>(el1)))
                    .Bind(x => b
                        .Map(y => map(x, y))
                        .MapError(el => el));

            return mapSuccess;
        }
    }
}