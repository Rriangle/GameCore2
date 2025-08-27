using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.DTOs;

/// <summary>
/// 遊戲資訊 DTO
/// </summary>
public class GameDto
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 創建遊戲請求 DTO
/// </summary>
public class CreateGameRequestDto
{
    [Required(ErrorMessage = "遊戲名稱為必填項目")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "遊戲名稱長度必須在 1-100 個字元之間")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "遊戲描述為必填項目")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "遊戲描述長度必須在 10-1000 個字元之間")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "遊戲分類為必填項目")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "遊戲分類長度必須在 1-50 個字元之間")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "遊戲價格為必填項目")]
    [Range(0, 10000, ErrorMessage = "遊戲價格必須在 0-10000 之間")]
    public decimal Price { get; set; }
}

/// <summary>
/// 更新遊戲請求 DTO
/// </summary>
public class UpdateGameRequestDto
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "遊戲名稱長度必須在 1-100 個字元之間")]
    public string? Name { get; set; }

    [StringLength(1000, MinimumLength = 10, ErrorMessage = "遊戲描述長度必須在 10-1000 個字元之間")]
    public string? Description { get; set; }

    [StringLength(50, MinimumLength = 1, ErrorMessage = "遊戲分類長度必須在 1-50 個字元之間")]
    public string? Category { get; set; }

    [Range(0, 10000, ErrorMessage = "遊戲價格必須在 0-10000 之間")]
    public decimal? Price { get; set; }

    public bool? IsActive { get; set; }
}

/// <summary>
/// 遊戲列表查詢 DTO
/// </summary>
public class GameListQueryDto
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
}

/// <summary>
/// 遊戲列表回應 DTO
/// </summary>
public class GameListResponseDto
{
    public List<GameDto> Games { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}