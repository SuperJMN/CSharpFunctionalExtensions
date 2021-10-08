using System;

namespace CSharpFunctionalExtensions
{
    public static class CombineExtensions
    {
        public static Either<R, E> Combine<E, T, K, R>(this Either<T, E> a,
            Either<K, E> b,
            Func<T, K, Either<R, E>> map, Func<E, E, E> combineError)
        {
            var mapSuccess = a.MapLeft2(el1 => b
                    .MapLeft1(el2 => combineError(el1, el2))
                    .MapRight2(_ => Either.Error<E, T>(el1)))
                .MapRight2(x => b
                    .MapRight2(y => map(x, y))
                    .MapLeft1(el => el));

            return mapSuccess;
        }

        public static Either<TResult, E> Combine<E, T1, T2, T3, TResult>(this
                Either<T1, E> a,
            Either<T2, E> b,
            Either<T3, E> c,
            Func<T1, T2, T3, Either<TResult, E>> onSuccess, Func<E, E, E> combineError)
        {
            var r = Combine(a, b, (arg1, arg2) => Either.Success<E, (T1, T2)>((arg1, arg2)), combineError);
            return r.Combine(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        }

        public static Either<TResult, E> Combine<E, T1, T2, T3, T4, TResult>(
            Either<T1, E> a,
            Either<T2, E> b,
            Either<T3, E> c,
            Either<T4, E> d,
            Func<T1, T2, T3, T4, Either<TResult, E>> onSuccess, Func<E, E, E> combineError)
        {
            var r = Combine(a, b, c, (x, y, z) => Either.Success<E, (T1, T2, T3)>((x, y, z)), combineError);
            return r.Combine(d, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, cur), combineError);
        }

        public static Either<TResult, E> Combine<E, T1, T2, T3, T4, T5, TResult>(
            Either<T1, E> a,
            Either<T2, E> b,
            Either<T3, E> c,
            Either<T4, E> d,
            Either<T5, E> e,
            Func<T1, T2, T3, T4, T5, Either<TResult, E>> onSuccess, Func<E, E, E> combineError)
        {
            var r = Combine(a, b, c, d, (x1, x2, x3, x4) => Either.Success<E, (T1, T2, T3, T4)>((x1, x2, x3, x4)),
                combineError);
            return r.Combine(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur),
                combineError);
        }

        public static Either<T, E> Combine<E, T>(
            this Either<T, E> ea,
            Either<T, E> eb,
            Func<T, T, Either<T, E>> mapSuccess, Func<E, E, E> combineError)
        {
            return ea
                .MapLeft2(el1 => eb
                    .MapLeft1(el2 => combineError(el1, el2))
                    .MapRight2(_ => Either.Error<E, T>(el1)))
                .MapRight2(x => eb
                    .MapRight2(y => mapSuccess(x, y))
                    .MapLeft1(el => el));
        }
    }

    public static class With
    {
        public static Either<TNewRight, E> MapRight1<E, T, TNewRight>(this Either<T, E> self,
            Func<T, TNewRight> map)
        {
            return self.Value.Match(
                right => Either.Success<E, TNewRight>(map(right)),
                () => Either.Error<E, TNewRight>(self.Error.GetValueOrDefault()));
        }

        public static Either<T, TNewLeft> MapLeft1<E, T, TNewLeft>(this Either<T, E> self,
            Func<E, TNewLeft> map)
        {
            return self.Error.Match(
                left => Either.Error<TNewLeft, T>(map(left)),
                () => Either.Success<TNewLeft, T>(self.Value.GetValueOrDefault()));
        }

        public static Either<TNewRight, E> MapRight2<E, T, TNewRight>(this Either<T, E> self,
            Func<T, Either<TNewRight, E>> map)
        {
            return self.Value.Match(
                right => map(right),
                () => Either.Error<E, TNewRight>(self.Error.GetValueOrDefault()));
        }

        public static Either<T, TNewLeft> MapLeft2<TNewLeft, E, T>(this Either<T, E> self,
            Func<E, Either<T, TNewLeft>> map)
        {
            return self.Error.Match(
                left => map(left),
                () => Either.Success<TNewLeft, T>(self.Value.GetValueOrDefault()));
        }

        public static T Handle<E, T>(this Either<T, E> self, Func<E, T> turnRight)
        {
            return self.Error.Match(turnRight, () => self.Value.GetValueOrDefault());
        }
    }

    public static class Either
    {
        public static Either<T, E> Success<E, T>(T right)
        {
            return new Either<T, E>(right);
        }

        public static Either<T, E> Error<E, T>(E left)
        {
            return new Either<T, E>(left);
        }
    }

    public static class ResultExtensions2
    {
        public static Maybe<T> Value<T, E>(this Result<T, E> result)
        {
            return Maybe.From(result.Value);
        }

        public static Maybe<E> Error<T, E>(this Result<T, E> result)
        {
            return Maybe.From(result.Error);
        }
    }

    public class Either<T, E>
    {
        public Maybe<E> Error { get; }
        public Maybe<T> Value { get; }

        public Either(E error)
        {
            Error = Maybe<E>.From(error);
        }

        public Either(T value)
        {
            Value = Maybe<T>.From(value);
        }
    }
}