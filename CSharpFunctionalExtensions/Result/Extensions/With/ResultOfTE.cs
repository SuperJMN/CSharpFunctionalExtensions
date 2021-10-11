﻿using System;

namespace CSharpFunctionalExtensions
{
    public static partial class ResultExtensions
    {
        public static Result<T, E> WithBind<T, E>(
            this Result<T, E> a,
            Result<T, E> b,
            Func<T, T, Result<T, E>> func, Func<E, E, E> combineError)
        {
            return a
                .BindError(el1 => b
                    .MapError(el2 => combineError(el1, el2))
                    .Bind(_ => Result.Failure<T, E>(el1)))
                .Bind(x => b
                    .Bind(y => func(x, y))
                    .MapError(el => el));
        }


        public static Result<R, E> WithMap<T1, T2, E, R>(this Result<T1, E> a,
            Result<T2, E> b,
            Func<T1, T2, R> func, Func<E, E, E> combineError)
        {
            var mapSuccess =
                a.BindError(el1 => b
                        .MapError(el2 => combineError(el1, el2))
                        .Bind(_ => Result.Failure<T1, E>(el1)))
                    .Bind(x => b
                        .Map(y => func(x, y))
                        .MapError(el => el));

            return mapSuccess;
        }

        public static Result<R, E> WithBind<T1, T2, E, R>(this Result<T1, E> a,
            Result<T2, E> b,
            Func<T1, T2, Result<R, E>> func, Func<E, E, E> combineError)
        {
            var mapSuccess =
                a.BindError(el1 => b
                        .MapError(el2 => combineError(el1, el2))
                        .Bind(_ => Result.Failure<T1, E>(el1)))
                    .Bind(x => b
                        .Bind(y => func(x, y))
                        .MapError(el => el));

            return mapSuccess;
        }
    }
}