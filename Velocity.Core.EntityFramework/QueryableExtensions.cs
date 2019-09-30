using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Velocity.Core.EntityFramework
{
    public static class QueryableExtensions
    {
        public static async Task<Paginated<TSource>> PaginateAsync<TSource>(this IQueryable<TSource> query, int size, int current, StartFrom startFrom = StartFrom.FirstPage)
        {
            var total = await query.CountAsync();

            List<TSource> result;

            if (total > 0)
            {
                var skipped = CalculateSkipped(total, size, current, startFrom);

                result = await query.Skip(skipped)
                    .Take(size)
                    .ToListAsync();
            }
            else
            {
                result = await query.ToListAsync();
            }

            return new Paginated<TSource>(result, current, size, total);
        }

        public static async Task<Paginated<TResult>> PaginateAsync<TSource, TResult>(this IQueryable<TSource> query, int size, int current, Expression<Func<TSource, TResult>> target, StartFrom startFrom = StartFrom.FirstPage)
        {
            var total = await query.CountAsync();

            List<TResult> result;

            if (total > 0)
            {
                var skipped = CalculateSkipped(total, size, current, startFrom);

                result = await query.Skip(skipped)
                    .Take(size)
                    .Select(target)
                    .ToListAsync();
            }
            else
            {
                result = await query.Select(target)
                                .ToListAsync();
            }


            return new Paginated<TResult>(result, current, size, total);
        }


        public static Paginated<TSource> Paginate<TSource>(this IQueryable<TSource> query, int size, int current, StartFrom startFrom = StartFrom.FirstPage)
        {
            var total = query.Count();

            List<TSource> result;

            if (total > 0)
            {
                var skipped = CalculateSkipped(total, size, current, startFrom);

                result = query.Skip(skipped)
                    .Take(size)
                    .ToList();
            }
            else
            {
                result = query.ToList();
            }

            return new Paginated<TSource>(result, current, size, total);
        }


        public static Paginated<TResult> Paginate<TSource, TResult>(this IQueryable<TSource> query, int size, int current, Expression<Func<TSource, TResult>> target, StartFrom startFrom = StartFrom.FirstPage)
        {
            var total = query.Count();

            List<TResult> result;

            if (total > 0)
            {
                var skipped = CalculateSkipped(total, size, current, startFrom);

                result = query.Skip(skipped)
                    .Take(size)
                    .Select(target)
                    .ToList();
            }
            else
            {
                result = query.Select(target).ToList();
            }

            return new Paginated<TResult>(result, current, size, total);
        }


        public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> query, int size, int current, StartFrom startFrom = StartFrom.FirstPage)
        {
            var total = query.Count();

            var skipped = CalculateSkipped(total, size, current, startFrom);

            return query.Skip(skipped)
                .Take(size);
        }


        public static IQueryable<TResult> Limit<TSource, TResult>(this IQueryable<TSource> query, int size, int current, Expression<Func<TSource, TResult>> target, StartFrom startFrom = StartFrom.FirstPage)
        {
            var total = query.Count();

            var skipped = CalculateSkipped(total, size, current, startFrom);

            return query.Skip(skipped)
                .Take(size)
                .Select(target);
        }


        private static int CalculateSkipped(int total, int size, int current, StartFrom startFrom)
        {
            if (startFrom == StartFrom.FirstPage)
                return (current - 1) * size;

            if (size == 0)
                return 0;

            var result = (total / size) - 1;

            var remaining = total % size;

            if (remaining > 0)
                return (result + 1) * size;

            return result * size;
        }
    }
}
