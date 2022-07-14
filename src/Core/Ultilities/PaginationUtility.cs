using Microsoft.EntityFrameworkCore;
//using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace Core.Utilities
{
    public class PaginationUtility<T> : List<T>
    {
        public long Current { get; private set; }
        public long TotalPages { get; private set; }
        public long PageSize { get; private set; }
        public long Total { get; private set; }
        public bool HasNext => Current < TotalPages;
        public bool HasPrevious => Current > 1;
        public PageInfo PageInfo { get; private set; }

        public PaginationUtility(List<T> items, long count, long pageNumber, long pageSize)
        {
            Total = count;
            PageSize = pageSize;
            Current = pageNumber;
            TotalPages = (long)Math.Ceiling(count / (double)pageSize);
            PageInfo = new PageInfo(Total, PageSize, Current, TotalPages, HasNext, HasPrevious);

            AddRange(items);
        }

        public static async Task<PaginationUtility<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginationUtility<T>(items, source.Count(), pageNumber, pageSize);
        }
        /*
        public static async Task<PaginationUtility<T>> ToPagedListAsync(IFindFluent<T, T> source, int pageNumber, int pageSize)
        {
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Limit(pageSize).ToListAsync();
            return new PaginationUtility<T>(items, (int)source.CountDocuments(), pageNumber, pageSize);
        }
        */
    }

    public class PaginationRequest
    {
        [TypeInt32MinValueValidation(1, ErrorMessage = "PageSizeGreaterThanOrEqual1")]
        public int PageSize { get; set; } = int.MaxValue / 2;
        [TypeInt32MinValueValidation(1, ErrorMessage = "PageNumberGreaterThanOrEqual1")]
        public int Current { get; set; } = 1;
        public string? OrderByQuery { get; set; } //Sample: "customerName desc, customerBirthday"
        public string? Search { get; set; }
    }

    public class TypeInt32MinValueValidationAttribute : ValidationAttribute
    {
        private readonly int MinValue;
        public TypeInt32MinValueValidationAttribute(int minValue)
        {
            MinValue = minValue;
        }
        public override bool IsValid(object? value)
        {
            return Convert.ToInt32(value) >= MinValue;
        }
    }

    public class PaginationResponse<T>
    {
        public IEnumerable<T> PagedData { get; set; }
        public PageInfo PageInfo { get; set; }

        private PaginationResponse(IEnumerable<T> items, PageInfo pageInfo)
        {
            PagedData = items;
            PageInfo = pageInfo;
        }

        public static PaginationResponse<T> PaginationInfo(IEnumerable<T> items, PageInfo pageInfo)
        {
            return new PaginationResponse<T>(items, pageInfo);
        }
    }

    public class PageInfo
    {
        public long Total { get; set; }
        public long PageSize { get; set; }
        public long Current { get; set; }
        public long TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }

        public PageInfo() { }

        public PageInfo(long totalCount, long pageSize, long currentPage, long totalPages, bool hasNext, bool hasPrevious)
        {
            Total = totalCount;
            PageSize = pageSize;
            Current = currentPage;
            TotalPages = totalPages;
            HasNext = hasNext;
            HasPrevious = hasPrevious;
        }
    }
}
