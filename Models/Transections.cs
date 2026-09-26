public class Transection
{
    public int id {get; set;}
    public required decimal value {get; set;}
    public string? description {get; set;}
    public DateTime CreatedAt {get; set;}
    public int UserId {get; set;}
    public int CategoryId {get; set;}
    public User? User{get; set;}
    public Category? Category{get; set;}
}