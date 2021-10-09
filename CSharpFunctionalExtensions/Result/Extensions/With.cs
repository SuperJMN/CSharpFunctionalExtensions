using System;

namespace CSharpFunctionalExtensions
{
    public static class CombineExtensions
    {
        public static Result<T, E> Combine<E, T>(
            this Result<T, E> ea,
            Result<T, E> eb,
            Func<T, T, Result<T, E>> mapSuccess, Func<E, E, E> combineError)
        {
            return ea
                .BindError(el1 => eb
                    .MapError(el2 => combineError(el1, el2))
                    .Bind(_ => Result.Failure<T, E>(el1)))
                .Bind(x => eb
                    .Bind(y => mapSuccess(x, y))
                    .MapError(el => el));
        }

        public static Result<R, E> Combine<E, T1, T2, R>(this Result<T1, E> a,
            Result<T2, E> b,
            Func<T1, T2, Result<R, E>> map, Func<E, E, E> combineError)
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

        public static Result<TResult, E> Combine<E, T1, T2, T3, TResult>(this
                Result<T1, E> a,
            Result<T2, E> b,
            Result<T3, E> c,
            Func<T1, T2, T3, Result<TResult, E>> onSuccess, Func<E, E, E> combineError)
        {
            var r = Combine(a, b, (arg1, arg2) => Result.Success<(T1, T2), E>((arg1, arg2)), combineError);
            return r.Combine(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        }

        public static Result<TResult, E> Combine<E, T1, T2, T3, T4, TResult>(
            Result<T1, E> a,
            Result<T2, E> b,
            Result<T3, E> c,
            Result<T4, E> d,
            Func<T1, T2, T3, T4, Result<TResult, E>> onSuccess, Func<E, E, E> combineError)
        {
            var r = Combine(a, b, c, (x, y, z) => Result.Success<(T1, T2, T3), E>((x, y, z)), combineError);
            return r.Combine(d, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, cur), combineError);
        }

        public static Result<TResult, E> Combine<E, T1, T2, T3, T4, T5, TResult>(
            Result<T1, E> a,
            Result<T2, E> b,
            Result<T3, E> c,
            Result<T4, E> d,
            Result<T5, E> e,
            Func<T1, T2, T3, T4, T5, Result<TResult, E>> onSuccess, Func<E, E, E> combineError)
        {
            var r = Combine(a, b, c, d, (x1, x2, x3, x4) => Result.Success<(T1, T2, T3, T4), E>((x1, x2, x3, x4)),
                combineError);
            return r.Combine(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur),
                combineError);
        }
    }

    public static class With
    {
        public static Result<T, K> MapLeft1<T, K, E>(this Result<T, E> self,
            Func<E, K> map)
        {
            return self.MapError(map);
        }

        public static Result<K, E> MapRight2<T, K, E>(this Result<T, E> self,
            Func<T, Result<K, E>> map)
        {
            return self.Bind(map);
        }

        public static Result<T, K> BindError<T, K, E>(this Result<T, E> self,
            Func<E, Result<T, K>> map)
        {
            return self.Error().Match(
                left => map(left),
                () => Result.Success<T, K>(self.Value().GetValueOrDefault()));
        }

        public static T Handle<T, E>(this Result<T, E> self, Func<E, T> turnRight)
        {
            return self.Error().Match(turnRight, () => self.Value().GetValueOrDefault());
        }
    }

    public static class ResultExtensions2
    {
        public static Maybe<T> Value<T, E>(this Result<T, E> result)
        {
            if (result.IsSuccess)
            {
                return Maybe<T>.From(result.Value);
            }

            return Maybe<T>.None;
        }

        public static Maybe<E> Error<T, E>(this Result<T, E> result)
        {
            if (result.IsFailure)
            {
                return Maybe<E>.From(result.Error);
            }

            return Maybe<E>.None;
        }
    }
}