using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 寵物 Repository
/// </summary>
public class PetRepository : IPetRepository
{
    private readonly GameCoreDbContext _context;

    public PetRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID獲取寵物
    /// </summary>
    public async Task<Pet?> GetByUserIdAsync(int userId)
    {
        return await _context.Pets
            .Where(p => p.UserID == userId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 根據寵物ID獲取寵物
    /// </summary>
    public async Task<Pet?> GetByPetIdAsync(int petId)
    {
        return await _context.Pets
            .Where(p => p.PetID == petId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 新增寵物
    /// </summary>
    public async Task<Pet> AddAsync(Pet pet)
    {
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        return pet;
    }

    /// <summary>
    /// 更新寵物
    /// </summary>
    public async Task<bool> UpdateAsync(Pet pet)
    {
        _context.Pets.Update(pet);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除寵物
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var pet = await _context.Pets.FindAsync(id);
        if (pet == null)
            return false;

        _context.Pets.Remove(pet);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
} 