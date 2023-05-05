using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int RecordsTotal { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, int draw)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            RecordsTotal = count;
            RecordsFiltered = count;
            Draw = draw;
            this.AddRange(items);
        }

        public bool HasPreviousPage {
            get {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage {
            get {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, int draw)
        {
            var count = await source.CountAsync().ConfigureAwait(false);
            var items = await source.Skip(pageIndex - 1).Take(pageSize).ToListAsync().ConfigureAwait(false);
            return new PaginatedList<T>(items, count, pageIndex, pageSize, draw);
        }

        public static async Task<PaginatedList<T>> CreateAsyncDataTable(IQueryable<T> source, int pageIndex, int pageSize, int draw, bool isUsingCountAsync = true)
        {
            var count = isUsingCountAsync ? await source.CountAsync().ConfigureAwait(false) : source.ToList().Count();

            var items = await source.Skip(pageIndex).Take(pageSize).ToListAsync().ConfigureAwait(false);
            return new PaginatedList<T>(items, count, pageIndex, pageSize, draw);
        }

        public static async Task<PaginatedList<T>> CreateAllDataAsyncDataTable(IQueryable<T> source, int draw, bool isUsingCountAsync = true)
        {
            var count = isUsingCountAsync ? await source.CountAsync().ConfigureAwait(false) : source.ToList().Count();

            var items = await source.ToListAsync().ConfigureAwait(false);
            return new PaginatedList<T>(items, count, default, default, draw);
        }
        public static async Task<PaginatedList<T>> CreateAllDataAsyncDataTable1(List<T> source, int draw, bool isUsingCountAsync = true)
        {
            var count = source.ToList().Count();

            //var items = await source.ToListAsync().ConfigureAwait(false);
            return new PaginatedList<T>(source, count, default, default, draw);
        }
        public static async Task<PaginatedList<T>> CreateAsyncDataTable(List<T> source,int totalRec, int pageIndex, int pageSize, int draw, bool isUsingCountAsync = true)
        {
            //var count = source.ToList().Count();
            return new PaginatedList<T>(source, totalRec, pageIndex, pageSize, draw);
        }
    }
}
