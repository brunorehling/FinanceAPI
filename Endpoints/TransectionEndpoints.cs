using System.Security.Claims;

public static class TransectionEndpoints
{
    public static void MapTransectionEndpoints(this WebApplication app)
    {
        var transections = app.MapGroup("/transections");

        transections.MapGet("/", async (ClaimsPrincipal userClaims, ITransectionService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.GetAllAsync(userId);
            return Results.Ok(result);
        }).RequireAuthorization();

        transections.MapGet("/summary", async (int? year, int? month, ClaimsPrincipal userClaims, ITransectionService service) =>
        {
            var now = DateTime.UtcNow;
            var y = year ?? now.Year;
            var m = month ?? now.Month;

            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var (income, expenses) = await service.GetMonthlySummaryAsync(userId, y, m);
            return Results.Ok(new { year = y, month = m, income, expenses, balance = income - expenses });
        }).RequireAuthorization();

        transections.MapPost("/", async (CreateTransectionDto dto, ClaimsPrincipal userClaims, ITransectionService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.CreateAsync(dto, userId);

            if (result is null)
                return Results.BadRequest("Categoria inválida ou não pertence ao usuário.");

            return Results.Created($"/transections/{result.id}", result);
        }).RequireAuthorization();

        transections.MapPut("/{id}", async (int id, UpdateTransectionDto dto, ClaimsPrincipal userClaims, ITransectionService service) =>
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await service.UpdateAsync(id, dto, userId);
                if (result == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(result);
        }).RequireAuthorization();

        transections.MapDelete("/{id}", async (int id, ClaimsPrincipal userClaims, ITransectionService service) =>
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