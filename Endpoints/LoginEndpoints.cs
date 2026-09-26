public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
        var login = app.MapGroup("/login");

        login.MapPost("/", async (LoginUserDto dto, IUserService service) =>
        {
            var logado = await service.LoginAsync(dto);
            if (logado is null)
                return Results.Unauthorized();

            return Results.Ok(logado);
        });
    }
}