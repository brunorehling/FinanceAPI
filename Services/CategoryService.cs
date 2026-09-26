using Microsoft.EntityFrameworkCore;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync(int UserId);
    Task<Category> CreateAsync(CreateCategoryDto dto, int UserId);
    Task<Category?> UpdateAsync(int id, UpdateCategoryDto dto, int UserId);
    Task<Category?> DeleteAsync(int id, int UserId);

}

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Category>> GetAllAsync(int userId)
    {
        return await _context.Categories.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<Category> CreateAsync(CreateCategoryDto dto, int userId)
    {
        var category = new Category { Name = dto.Name, type = dto.type, UserId = userId };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(int id, UpdateCategoryDto dto, int userId)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null || category.UserId != userId)
        return null;

        category.Name = dto.Name;
        category.type = dto.type;

        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> DeleteAsync(int id, int userId)
    {
        var category = await _context.Categories.FindAsync(id);

       if (category is null || category.UserId != userId)
        return null;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return category;
    }
}