public class Goal
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}