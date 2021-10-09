using System;
using System.Threading.Tasks;

namespace CSharpFunctionalExtensions
{
    public static partial class ResultExtensions
    {
        public static Task<Result<T, E>> With<T, E>(
            this Result<T, E> a,
            Result<T, E> b,
            Func<T, T, Task<Result<T, E>>> mapSuccess, Func<E, E, E> combineError)
        {
            return a
                .BindError(el1 => b
                    .MapError(el2 => combineError(el1, el2))
                    .Bind(_ => Result.Failure<T, E>(el1)))
                .Bind(x => b
                    .Bind(y => mapSuccess(x, y))
                    .MapError(el => el));
        }

        public static Task<Result<T, E>> With<T, E>(
            this Result<T, E> a,
            Result<T, E> b,
            Func<T, T, Task<T>> mapSuccess, Func<E, E, E> combineError)
        {
            return a
                .BindError(el1 => b
                    .MapError(el2 => combineError(el1, el2))
                    .Bind(_ => Result.Failure<T, E>(el1)))
                .Bind(x => b
                    .Map(y => mapSuccess(x, y))
                    .MapError(el => el));
        }


        //public static Result<T, E> With<T, E>(
        //    this Result<T, E> a,
        //    Result<T, E> b,
        //    Func<T, T, T> mapSuccess, Func<E, E, E> combineError)
        //{
        //    return a
        //        .BindError(el1 => b
        //            .MapError(el2 => combineError(el1, el2))
        //            .Bind(_ => Result.Failure<T, E>(el1)))
        //        .Bind(x => b
        //            .Map(y => mapSuccess(x, y))
        //            .MapError(el => el));
        //}

        //public static Result<R, E> With<T1, T2, E, R>(this Result<T1, E> a,
        //    Result<T2, E> b,
        //    Func<T1, T2, Result<R, E>> map, Func<E, E, E> combineError)
        //{
        //    var mapSuccess =
        //        a.BindError(el1 => b
        //                .MapError(el2 => combineError(el1, el2))
        //                .Bind(_ => Result.Failure<T1, E>(el1)))
        //            .Bind(x => b
        //                .Bind(y => map(x, y))
        //                .MapError(el => el));

        //    return mapSuccess;
        //}

        //public static Result<TResult, E> With<T1, T2, T3, E, TResult>(this
        //        Result<T1, E> a,
        //    Result<T2, E> b,
        //    Result<T3, E> c,
        //    Func<T1, T2, T3, Result<TResult, E>> onSuccess, Func<E, E, E> combineError)
        //{
        //    var r = With(a, b, (arg1, arg2) => Result.Success<(T1, T2), E>((arg1, arg2)), combineError);
        //    return r.With(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        //}

        //public static Result<TResult, E> With<T1, T2, T3, T4, E, TResult>(
        //    Result<T1, E> a,
        //    Result<T2, E> b,
        //    Result<T3, E> c,
        //    Result<T4, E> d,
        //    Func<T1, T2, T3, T4, Result<TResult, E>> onSuccess, Func<E, E, E> combineError)
        //{
        //    var r = With(a, b, c, (x, y, z) => Result.Success<(T1, T2, T3), E>((x, y, z)), combineError);
        //    return r.With(d, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, cur), combineError);
        //}

        //public static Result<TResult, E> With<T1, T2, T3, T4, T5, E, TResult>(
        //    Result<T1, E> a,
        //    Result<T2, E> b,
        //    Result<T3, E> c,
        //    Result<T4, E> d,
        //    Result<T5, E> e,
        //    Func<T1, T2, T3, T4, T5, Result<TResult, E>> onSuccess, Func<E, E, E> combineError)
        //{
        //    var r = With(a, b, c, d, (x1, x2, x3, x4) => Result.Success<(T1, T2, T3, T4), E>((x1, x2, x3, x4)),
        //        combineError);
        //    return r.With(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur),
        //        combineError);
        //}

        //public static Result<TResult> With<T, K, TResult>(
        //    this Result<T> a,
        //    Result<K> b,
        //    Func<T, K, Result<TResult>> mapSuccess)
        //{
        //    return a
        //        .BindError(e1 => b
        //            .MapError(e2 => string.Join(Result.ErrorMessagesSeparator, e1, e2))
        //            .Bind(_ => Result.Failure<T>(e1))
        //        ).Bind(x => b
        //            .Bind(y => mapSuccess(x, y))
        //            .MapError(e => e));
        //}

        //public static Result<TResult> With<T, K, TResult>(
        //    this Result<T> a,
        //    Result<K> b,
        //    Func<T, K, TResult> mapSuccess)
        //{
        //    return a
        //        .BindError(e1 => b
        //            .MapError(e2 => string.Join(Result.ErrorMessagesSeparator, e1, e2))
        //            .Bind(_ => Result.Failure<T>(e1))
        //        ).Bind(x => b
        //            .Map(y => mapSuccess(x, y))
        //            .MapError(e => e));
        //}
    }
}