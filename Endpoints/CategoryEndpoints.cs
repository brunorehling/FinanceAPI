using System.Security.Claims;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var categories = app.MapGroup("/categories");

        categories.MapGet("/", async (ClaimsPrincipal userClaims, ICategoryService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.GetAllAsync(userId);
            return Results.Ok(result);
        }).RequireAuthorization();

        categories.MapPost("/", async (CreateCategoryDto dto, ClaimsPrincipal userClaims, ICategoryService service) =>
        {
            var userIdString = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdString!);
            var result = await service.CreateAsync(dto, userId);

            return Results.Created();
        }).RequireAuthorization();

        categories.MapPut("/{id}", async (int id, UpdateCategoryDto dto,ClaimsPrincipal userClaims, ICategoryService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.UpdateAsync(id, dto, userId);
                if (result == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(result);
        }).RequireAuthorization();

        categories.MapDelete("/{id}", async (int id, ClaimsPrincipal userClaims, ICategoryService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.DeleteAsync(id, userId);
            if (result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(result);
        }).RequireAuthorization();
    }
} 