using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 寵物 Repository 介面
/// </summary>
public interface IPetRepository
{
    /// <summary>
    /// 根據用戶ID獲取寵物
    /// </summary>
    Task<Pet?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 根據寵物ID獲取寵物
    /// </summary>
    Task<Pet?> GetByPetIdAsync(int petId);

    /// <summary>
    /// 新增寵物
    /// </summary>
    Task<Pet> AddAsync(Pet pet);

    /// <summary>
    /// 更新寵物
    /// </summary>
    Task<bool> UpdateAsync(Pet pet);

    /// <summary>
    /// 刪除寵物
    /// </summary>
    Task<bool> DeleteAsync(int id);
} 