using System;

namespace CSharpFunctionalExtensions
{
    public static class CombineExtensions
    {
        public static Either<E, R> Combine<E, T, K, R>(this Either<E, T> a,
            Either<E, K> b,
            Func<T, K, Either<E, R>> map, Func<E, E, E> combineError)
        {
            var mapSuccess = a.MapLeft2(el1 => b
                    .MapLeft1(el2 => combineError(el1, el2))
                    .MapRight2(_ => Either.Error<E, T>(el1)))
                .MapRight2(x => b
                    .MapRight2(y => map(x, y))
                    .MapLeft1(el => el));

            return mapSuccess;
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, TResult>(this
                Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Func<T1, T2, T3, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, (arg1, arg2) => Either.Success<TLeft, (T1, T2)>((arg1, arg2)), combineError);
            return r.Combine(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, T4, TResult>(
            Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Either<TLeft, T4> d,
            Func<T1, T2, T3, T4, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, (x, y, z) => Either.Success<TLeft, (T1, T2, T3)>((x, y, z)), combineError);
            return r.Combine(d, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, cur), combineError);
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, T4, T5, TResult>(
            Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Either<TLeft, T4> d,
            Either<TLeft, T5> e,
            Func<T1, T2, T3, T4, T5, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, d, (x1, x2, x3, x4) => Either.Success<TLeft, (T1, T2, T3, T4)>((x1, x2, x3, x4)),
                combineError);
            return r.Combine(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur),
                combineError);
        }

        public static Either<TLeft, TRight> Combine<TLeft, TRight>(
            this Either<TLeft, TRight> ea,
            Either<TLeft, TRight> eb,
            Func<TRight, TRight, Either<TLeft, TRight>> mapSuccess, Func<TLeft, TLeft, TLeft> combineError)
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
        public static Either<TLeft, TNewRight> MapRight1<TLeft, TRight, TNewRight>(this Either<TLeft, TRight> self,
            Func<TRight, TNewRight> map)
        {
            return self.Right.Match(
                right => new Either<TLeft, TNewRight>(map(right)),
                () => new Either<TLeft, TNewRight>(self.Left.GetValueOrDefault()));
        }

        public static Either<TNewLeft, TRight> MapLeft1<TLeft, TRight, TNewLeft>(this Either<TLeft, TRight> self,
            Func<TLeft, TNewLeft> map)
        {
            return self.Left.Match(
                left => new Either<TNewLeft, TRight>(map(left)),
                () => new Either<TNewLeft, TRight>(self.Right.GetValueOrDefault()));
        }

        public static Either<TLeft, TNewRight> MapRight2<TLeft, TRight, TNewRight>(this Either<TLeft, TRight> self,
            Func<TRight, Either<TLeft, TNewRight>> map)
        {
            return self.Right.Match(
                right => map(right),
                () => new Either<TLeft, TNewRight>(self.Left.GetValueOrDefault()));
        }

        public static Either<TNewLeft, TRight> MapLeft2<TNewLeft, TLeft, TRight>(this Either<TLeft, TRight> self,
            Func<TLeft, Either<TNewLeft, TRight>> map)
        {
            return self.Left.Match(
                left => map(left),
                () => new Either<TNewLeft, TRight>(self.Right.GetValueOrDefault()));
        }

        public static Either<TLeft, TRight> Error<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }

        public static TRight Handle<TLeft, TRight>(this Either<TLeft, TRight> self, Func<TLeft, TRight> turnRight)
        {
            return self.Left.Match(turnRight, () => self.Right.GetValueOrDefault());
        }
    }

    public static class Either
    {
        public static Either<TLeft, TRight> Success<TLeft, TRight>(TRight right)
        {
            return new Either<TLeft, TRight>(right);
        }

        public static Either<TLeft, TRight> Error<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }
    }

    public class Either<TLeft, TRight>
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