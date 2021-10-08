using System;

namespace CSharpFunctionalExtensions
{
    public static class CombineExtensions
    {
        public static Result<R, E> Combine<E, T, K, R>(this Result<T, E> a,
            Result<K, E> b,
            Func<T, K, Result<R, E>> map, Func<E, E, E> combineError)
        {
            var mapSuccess = a.MapLeft2(el1 => b
                    .MapLeft1(el2 => combineError(el1, el2))
                    .MapRight2(_ => Result.Failure<T, E>(el1)))
                .MapRight2(x => b
                    .MapRight2(y => map(x, y))
                    .MapLeft1(el => el));

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

        public static Result<T, E> Combine<E, T>(
            this Result<T, E> ea,
            Result<T, E> eb,
            Func<T, T, Result<T, E>> mapSuccess, Func<E, E, E> combineError)
        {
            return ea
                .MapLeft2(el1 => eb
                    .MapLeft1(el2 => combineError(el1, el2))
                    .MapRight2(_ => Result.Failure<T, E>(el1)))
                .MapRight2(x => eb
                    .MapRight2(y => mapSuccess(x, y))
                    .MapLeft1(el => el));
        }
    }

    public static class With
    {
        public static Result<TNewRight, E> MapRight1<E, T, TNewRight>(this Result<T, E> self,
            Func<T, TNewRight> map)
        {
            return self.Value().Match(
                right => Result.Success<TNewRight, E>(map(right)),
                () => Result.Failure<TNewRight, E>(self.Error().GetValueOrDefault()));
        }

        public static Result<T, TNewLeft> MapLeft1<E, T, TNewLeft>(this Result<T, E> self,
            Func<E, TNewLeft> map)
        {
            return self.Error().Match(
                left => Result.Failure<T, TNewLeft>(map(left)),
                () => Result.Success<T, TNewLeft>(self.Value().GetValueOrDefault()));
        }

        public static Result<TNewRight, E> MapRight2<E, T, TNewRight>(this Result<T, E> self,
            Func<T, Result<TNewRight, E>> map)
        {
            return self.Value().Match(
                right => map(right),
                () => Result.Failure<TNewRight, E>(self.Error().GetValueOrDefault()));
        }

        public static Result<T, TNewLeft> MapLeft2<TNewLeft, E, T>(this Result<T, E> self,
            Func<E, Result<T, TNewLeft>> map)
        {
            return self.Error().Match(
                left => map(left),
                () => Result.Success<T, TNewLeft>(self.Value().GetValueOrDefault()));
        }

        public static T Handle<E, T>(this Result<T, E> self, Func<E, T> turnRight)
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