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

        public static Either<TResult, TLeft> Combine<TLeft, T1, T2, T3, TResult>(this
                Either<T1, TLeft> a,
            Either<T2, TLeft> b,
            Either<T3, TLeft> c,
            Func<T1, T2, T3, Either<TResult, TLeft>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, (arg1, arg2) => Either.Success<TLeft, (T1, T2)>((arg1, arg2)), combineError);
            return r.Combine(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        }

        public static Either<TResult, TLeft> Combine<TLeft, T1, T2, T3, T4, TResult>(
            Either<T1, TLeft> a,
            Either<T2, TLeft> b,
            Either<T3, TLeft> c,
            Either<T4, TLeft> d,
            Func<T1, T2, T3, T4, Either<TResult, TLeft>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, (x, y, z) => Either.Success<TLeft, (T1, T2, T3)>((x, y, z)), combineError);
            return r.Combine(d, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, cur), combineError);
        }

        public static Either<TResult, TLeft> Combine<TLeft, T1, T2, T3, T4, T5, TResult>(
            Either<T1, TLeft> a,
            Either<T2, TLeft> b,
            Either<T3, TLeft> c,
            Either<T4, TLeft> d,
            Either<T5, TLeft> e,
            Func<T1, T2, T3, T4, T5, Either<TResult, TLeft>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, d, (x1, x2, x3, x4) => Either.Success<TLeft, (T1, T2, T3, T4)>((x1, x2, x3, x4)),
                combineError);
            return r.Combine(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur),
                combineError);
        }

        public static Either<TRight, TLeft> Combine<TLeft, TRight>(
            this Either<TRight, TLeft> ea,
            Either<TRight, TLeft> eb,
            Func<TRight, TRight, Either<TRight, TLeft>> mapSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            return ea
                .MapLeft2(el1 => eb
                    .MapLeft1(el2 => combineError(el1, el2))
                    .MapRight2(_ => Either.Error<TLeft, TRight>(el1)))
                .MapRight2(x => eb
                    .MapRight2(y => mapSuccess(x, y))
                    .MapLeft1(el => el));
        }
    }

    public static class With
    {
        public static Either<TNewRight, TLeft> MapRight1<TLeft, TRight, TNewRight>(this Either<TRight, TLeft> self,
            Func<TRight, TNewRight> map)
        {
            return self.Right.Match(
                right => new Either<TNewRight, TLeft>(map(right)),
                () => new Either<TNewRight, TLeft>(self.Left.GetValueOrDefault()));
        }

        public static Either<TRight, TNewLeft> MapLeft1<TLeft, TRight, TNewLeft>(this Either<TRight, TLeft> self,
            Func<TLeft, TNewLeft> map)
        {
            return self.Left.Match(
                left => new Either<TRight, TNewLeft>(map(left)),
                () => new Either<TRight, TNewLeft>(self.Right.GetValueOrDefault()));
        }

        public static Either<TNewRight, TLeft> MapRight2<TLeft, TRight, TNewRight>(this Either<TRight, TLeft> self,
            Func<TRight, Either<TNewRight, TLeft>> map)
        {
            return self.Right.Match(
                right => map(right),
                () => new Either<TNewRight, TLeft>(self.Left.GetValueOrDefault()));
        }

        public static Either<TRight, TNewLeft> MapLeft2<TNewLeft, TLeft, TRight>(this Either<TRight, TLeft> self,
            Func<TLeft, Either<TRight, TNewLeft>> map)
        {
            return self.Left.Match(
                left => map(left),
                () => new Either<TRight, TNewLeft>(self.Right.GetValueOrDefault()));
        }

        public static Either<TRight, TLeft> Error<TLeft, TRight>(TLeft left)
        {
            return new Either<TRight, TLeft>(left);
        }

        public static TRight Handle<TLeft, TRight>(this Either<TRight, TLeft> self, Func<TLeft, TRight> turnRight)
        {
            return self.Left.Match(turnRight, () => self.Right.GetValueOrDefault());
        }
    }

    public static class Either
    {
        public static Either<TRight, TLeft> Success<TLeft, TRight>(TRight right)
        {
            return new Either<TRight, TLeft>(right);
        }

        public static Either<TRight, TLeft> Error<TLeft, TRight>(TLeft left)
        {
            return new Either<TRight, TLeft>(left);
        }
    }

    public class Either<TRight, TLeft>
    {
        public Maybe<TLeft> Left { get; }
        public Maybe<TRight> Right { get; }

        public Either(TLeft left)
        {
            Left = Maybe<TLeft>.From(left);
        }

        public Either(TRight right)
        {
            Right = Maybe<TRight>.From(right);
        }
    }
}