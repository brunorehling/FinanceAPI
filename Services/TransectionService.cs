using Microsoft.EntityFrameworkCore;

public interface ITransectionService
{
    Task<List<Transection>> GetAllAsync(int userId);
    Task<(decimal income, decimal expenses)> GetMonthlySummaryAsync(int userId, int year, int month);
    Task<Transection?> CreateAsync(CreateTransectionDto dto, int userId);
    Task<Transection?> UpdateAsync(int id, UpdateTransectionDto dto, int userId);
    Task<Transection?> DeleteAsync(int id, int userId);
}

public class TransectionService : ITransectionService
{
    private readonly AppDbContext _context;

    public TransectionService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Transection>> GetAllAsync(int userId)
    {
        return await _context.Transections.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<(decimal income, decimal expenses)> GetMonthlySummaryAsync(int userId, int year, int month)
    {
        var transactions = await _context.Transections
            .Include(t => t.Category)
            .Where(t => t.UserId == userId
                    && t.CreatedAt.Year == year
                    && t.CreatedAt.Month == month)
            .ToListAsync();

        var income = transactions.Where(t => t.Category.type == TransectionType.Income).Sum(t => t.value);
        var expenses = transactions.Where(t => t.Category.type == TransectionType.Expense).Sum(t => t.value);

        return (income, expenses);
    }

    public async Task<Transection> CreateAsync(CreateTransectionDto dto, int userId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.id == dto.categoryId && c.UserId == userId);

        if (category is null)
            return null;

        var transection = new Transection { value = dto.value,
        description = dto.description,
        CategoryId = dto.categoryId,
        UserId = userId,
        CreatedAt = DateTime.UtcNow  
        };

        _context.Transections.Add(transection);
        await _context.SaveChangesAsync();
        return transection;
    }

    public async Task<Transection?> UpdateAsync(int id, UpdateTransectionDto dto, int userId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.id == dto.categoryId && c.UserId == userId);

        if (category is null)
            return null;

        var transection = await _context.Transections.FindAsync(id);

        if (transection is null || transection.UserId != userId)
            return null; 

        transection.value = dto.value;
        transection.description = dto.description;
        transection.CategoryId = dto.categoryId;

        await _context.SaveChangesAsync();
        return transection;
    }

   public async Task<Transection?> DeleteAsync(int id, int userId)
{
    var transection = await _context.Transections.FindAsync(id);

    if (transection is null || transection.UserId != userId)
        return null;

    _context.Transections.Remove(transection);
    await _context.SaveChangesAsync();
    return transection;
}
}