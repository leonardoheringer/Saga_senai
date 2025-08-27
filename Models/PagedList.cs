using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SistemaPetrobras.Models
{
    public class PagedList<T> : List<T>
    {
    // A propriedade Items é esperada pelas views (compatibilidade com outros PagedList implementations)
    public List<T> Items => this;

        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int PageCount => TotalPages;
        public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;
        public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}

