using System.Security.Claims;

public static class GoalEndpoints
{
    public static void MapGoalEndpoints(this WebApplication app)
    {
        var goals = app.MapGroup("/goals");

        goals.MapGet("/", async (ClaimsPrincipal userClaims, IGoalService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.GetAllAsync(userId);
            return Results.Ok(result);
        }).RequireAuthorization();

        goals.MapPost("/", async (CreateGoalDto dto, ClaimsPrincipal userClaims, IGoalService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.CreateAsync(dto, userId);

            if (result is null)
                return Results.BadRequest("Categoria inválida ou não pertence ao usuário.");

            return Results.Created($"/goals/{result.Id}", result);
        }).RequireAuthorization();

        goals.MapPut("/{id}", async (int id, UpdateGoalDto dto, ClaimsPrincipal userClaims, IGoalService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.UpdateAsync(id, dto, userId);

            if (result is null)
                return Results.NotFound();

            return Results.Ok(result);
        }).RequireAuthorization();

        goals.MapDelete("/{id}", async (int id, ClaimsPrincipal userClaims, IGoalService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.DeleteAsync(id, userId);

            if (result is null)
                return Results.NotFound();

            return Results.Ok(result);
        }).RequireAuthorization();
    }
}