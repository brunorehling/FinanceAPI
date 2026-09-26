using Microsoft.EntityFrameworkCore;

public record GoalWithProgress(Goal Goal, decimal CurrentAmount, decimal PercentUsed);

public interface IGoalService
{
    Task<List<GoalWithProgress>> GetAllAsync(int userId);
    Task<Goal?> CreateAsync(CreateGoalDto dto, int userId);
    Task<Goal?> UpdateAsync(int id, UpdateGoalDto dto, int userId);
    Task<Goal?> DeleteAsync(int id, int userId);
}

public class GoalService : IGoalService
{
    private readonly AppDbContext _context;

    public GoalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GoalWithProgress>> GetAllAsync(int userId)
    {
        var goals = await _context.Goals
            .Where(g => g.UserId == userId)
            .ToListAsync();

        var result = new List<GoalWithProgress>();

        foreach (var goal in goals)
        {
            var query = _context.Transections
                .Include(t => t.Category)
                .Where(t => t.UserId == userId
                         && t.CreatedAt >= goal.StartDate
                         && t.CreatedAt <= goal.EndDate
                         && t.Category.type == TransectionType.Expense);

            if (goal.CategoryId is not null)
                query = query.Where(t => t.CategoryId == goal.CategoryId);

            var currentAmount = await query.SumAsync(t => t.value);
            var percentUsed = goal.TargetAmount > 0 
                ? Math.Round((currentAmount / goal.TargetAmount) * 100, 1) 
                : 0;

            result.Add(new GoalWithProgress(goal, currentAmount, percentUsed));
        }

        return result;
    }

    public async Task<Goal?> CreateAsync(CreateGoalDto dto, int userId)
    {
        if (dto.CategoryId is not null)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.id == dto.CategoryId && c.UserId == userId);
            if (!categoryExists)
                return null;
        }

        var goal = new Goal
        {
            Name = dto.Name,
            TargetAmount = dto.TargetAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CategoryId = dto.CategoryId,
            UserId = userId
        };

        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal?> UpdateAsync(int id, UpdateGoalDto dto, int userId)
    {
        var goal = await _context.Goals.FindAsync(id);

        if (goal is null || goal.UserId != userId)
            return null;

        goal.Name = dto.Name;
        goal.TargetAmount = dto.TargetAmount;
        goal.StartDate = dto.StartDate;
        goal.EndDate = dto.EndDate;
        goal.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal?> DeleteAsync(int id, int userId)
    {
        var goal = await _context.Goals.FindAsync(id);

        if (goal is null || goal.UserId != userId)
            return null;

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        return goal;
    }
}