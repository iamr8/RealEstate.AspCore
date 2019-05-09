using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RealEstate.Services.Extensions
{
    public static class SearchExtensions
    {
        public static bool Any<T>(this ICollection<T> collection, SearchStringOperand operand, Func<T, string> predicate, string value)
        {
            if (value == null)
                return false;

            switch (operand)
            {
                case SearchStringOperand.Like:
                    if (string.IsNullOrEmpty(value))
                        return false;

                    return collection.Any(x => EF.Functions.Like(predicate.Invoke(x), value.Like()));

                case SearchStringOperand.Equal:
                default:
                    if (string.IsNullOrEmpty(value))
                        return false;

                    return collection.Any(x => predicate.Invoke(x) == value);
            }
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, SearchStringOperand operand, Func<T, string> predicate, string value)
        {
            if (value == null)
                return query;

            switch (operand)
            {
                case SearchStringOperand.Like:
                    query = query.Where(x => EF.Functions.Like(predicate.Invoke(x), value.Like()));
                    break;

                case SearchStringOperand.Equal:
                default:
                    query = query.Where(x => predicate.Invoke(x) == value);

                    break;
            }

            return query;
        }

        public static IQueryable<T> Where<T, TT>(this IQueryable<T> query, SearchNumericOperand operand, Func<T, int> key, TT value) where TT : struct
        {
            switch (operand)
            {
                case SearchNumericOperand.Greater:
                    switch (value)
                    {
                        case int INT:
                            query = query.Where(x => key(x) > INT);
                            break;

                        case decimal DECIMAL:
                            query = query.Where(x => key(x) > DECIMAL);
                            break;

                        case long LONG:
                            query = query.Where(x => key(x) > LONG);
                            break;

                        case double DOUBLE:
                            query = query.Where(x => key(x) > DOUBLE);
                            break;
                    }

                    break;

                case SearchNumericOperand.GreaterEqual:
                    switch (value)
                    {
                        case int INT:
                            query = query.Where(x => key(x) >= INT);
                            break;

                        case decimal DECIMAL:
                            query = query.Where(x => key(x) >= DECIMAL);
                            break;

                        case long LONG:
                            query = query.Where(x => key(x) >= LONG);
                            break;

                        case double DOUBLE:
                            query = query.Where(x => key(x) >= DOUBLE);
                            break;
                    }
                    break;

                case SearchNumericOperand.Less:
                    switch (value)
                    {
                        case int INT:
                            query = query.Where(x => key(x) < INT);
                            break;

                        case decimal DECIMAL:
                            query = query.Where(x => key(x) < DECIMAL);
                            break;

                        case long LONG:
                            query = query.Where(x => key(x) < LONG);
                            break;

                        case double DOUBLE:
                            query = query.Where(x => key(x) < DOUBLE);
                            break;
                    }
                    break;

                case SearchNumericOperand.LessEqual:
                    switch (value)
                    {
                        case int INT:
                            query = query.Where(x => key(x) <= INT);
                            break;

                        case decimal DECIMAL:
                            query = query.Where(x => key(x) <= DECIMAL);
                            break;

                        case long LONG:
                            query = query.Where(x => key(x) <= LONG);
                            break;

                        case double DOUBLE:
                            query = query.Where(x => key(x) <= DOUBLE);
                            break;
                    }
                    break;

                case SearchNumericOperand.Equal:
                default:
                    switch (value)
                    {
                        case int INT:
                            query = query.Where(x => key(x) == INT);
                            break;

                        case decimal DECIMAL:
                            query = query.Where(x => key(x) == DECIMAL);
                            break;

                        case long LONG:
                            query = query.Where(x => key(x) == LONG);
                            break;

                        case double DOUBLE:
                            query = query.Where(x => key(x) == DOUBLE);
                            break;
                    }
                    break;
            }

            return query;
        }

        public enum SearchStringOperand
        {
            Equal, Like
        }

        public enum SearchNumericOperand
        {
            Equal, Greater, GreaterEqual, Less, LessEqual
        }

        public enum SearchComparisonOperand
        {
            And, Or
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, SearchComparisonOperand operand, params Expression<Func<T, bool>>[] conditions)
        {
            var predicate = PredicateBuilder.New<T>();
            if (conditions?.Any() != true)
                return query;

            if (conditions?.Any() == true)
            {
                foreach (var condition in conditions)
                {
                    if (condition == null)
                        continue;

                    switch (operand)
                    {
                        case SearchComparisonOperand.And:
                            predicate = predicate.And(condition);
                            break;

                        case SearchComparisonOperand.Or:
                        default:
                            predicate = predicate.Or(condition);
                            break;
                    }
                }
            }

            query = query.Where(x => predicate.Compile().Invoke(x));
            return query;
        }
    }
}