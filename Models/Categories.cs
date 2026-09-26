public enum TransectionType
{
    Income,
    Expense
}

public class Category
{
    public int id {get; set;} 
    public required string Name {get; set;}
    public required TransectionType type {get; set;}
    public DateTime CreatedAt {get; set;}
    public int UserId {get; set;}
    public User? User{get; set;}
}