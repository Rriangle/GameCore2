using System.Collections.Generic;

namespace GameCore.Application.Common
{
    /// <summary>
    /// 分頁查詢結果
    /// </summary>
    /// <typeparam name="T">資料項目類型</typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PagedResult()
        {
            Items = new List<T>();
        }

        public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            
            CalculatePagination();
        }

        private void CalculatePagination()
        {
            TotalPages = PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < TotalPages;
        }

        /// <summary>
        /// 建立空的分頁結果
        /// </summary>
        /// <param name="pageNumber">頁碼</param>
        /// <param name="pageSize">頁面大小</param>
        /// <returns>空的分頁結果</returns>
        public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 20)
        {
            return new PagedResult<T>(new List<T>(), 0, pageNumber, pageSize);
        }

        /// <summary>
        /// 建立分頁結果
        /// </summary>
        /// <param name="items">資料項目</param>
        /// <param name="totalCount">總數量</param>
        /// <param name="pageNumber">頁碼</param>
        /// <param name="pageSize">頁面大小</param>
        /// <returns>分頁結果</returns>
        public static PagedResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }
} 