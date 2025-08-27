using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [StringLength(100)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Required]
        public bool IsOfficialStore { get; set; } = true;
    }
    
    public class UpdateProductDto
    {
        [StringLength(200)]
        public string? Name { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int? StockQuantity { get; set; }
        
        public int? CategoryId { get; set; }
        
        [StringLength(100)]
        public string? ImageUrl { get; set; }
        
        public bool? IsActive { get; set; }
    }
    
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsOfficialStore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class CreateProductCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string IconUrl { get; set; } = string.Empty;
        
        [Required]
        public int SortOrder { get; set; } = 0;
    }
    
    public class UpdateProductCategoryDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(100)]
        public string? IconUrl { get; set; }
        
        public int? SortOrder { get; set; }
        
        public bool? IsActive { get; set; }
    }
    
    public class ProductCategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public int ProductCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 分頁產品列表 DTO - 新增：支援分頁的產品列表
    /// </summary>
    public class PaginatedProductsDto
    {
        /// <summary>
        /// 產品列表
        /// </summary>
        public List<ProductResponseDto> Products { get; set; } = new();

        /// <summary>
        /// 當前頁碼
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 頁面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 總記錄數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// 搜尋關鍵字（如果有的話）
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// 分類 ID（如果有的話）
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 排序欄位
        /// </summary>
        public string SortBy { get; set; } = "Name";

        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; } = "asc";
    }
}