public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users");

        users.MapGet("/", async (IUserService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        }).RequireAuthorization();

        users.MapPost("/", async (CreateUserDto dto, IUserService service) =>
        {
            var criado = await service.CreateAsync(dto);
            return Results.Created($"/users/{criado.Id}", criado);
        });

        users.MapPut("/{id}", async (int id, UpdateUserDto dto, IUserService service) =>
            {
                var result = await service.UpdateAsync(id, dto);
                if (result == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(result);
            }).RequireAuthorization();

        users.MapDelete("/{id}", async (int id, IUserService service) =>
        {
            var result = await service.DeleteAsync(id);
            if (result == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(result);
        }).RequireAuthorization();
    }
    }