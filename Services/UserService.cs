using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
public interface IUserService
{
    Task<string?> LoginAsync(LoginUserDto dto);
    Task<List<User>> GetAllAsync();

    Task<User> CreateAsync(CreateUserDto dto);

    Task<User?> UpdateAsync(int id, UpdateUserDto dto);

    Task<User?> DeleteAsync(int id);
};

public class UserService : IUserService
{
    
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public UserService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<string?> LoginAsync(LoginUserDto dto)
    {
        var jwtKey = _config["Jwt:Key"]!;
        var jwtIssuer = _config["Jwt:Issuer"];

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
            return null;

        var hasher = new PasswordHasher<User>();
        var resultado = hasher.VerifyHashedPassword(user, user.Password, dto.Password);

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    public async Task<User> CreateAsync(CreateUserDto dto)
    {

        var user = new User { Email = dto.Email, Password = dto.Password, Name = dto.Name};

        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id); 

        if (user is null)
            return null;

        var hasher = new PasswordHasher<User>();

        user.Email = dto.Email;
        user.Password = hasher.HashPassword(user, dto.Password);
        user.Name = dto.Name;

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
            return null; 

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }


}