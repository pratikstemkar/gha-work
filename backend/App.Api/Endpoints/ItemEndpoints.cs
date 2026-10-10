using App.Api.Models;
using App.Api.Services;

namespace App.Api.Endpoints;

public static class ItemEndpoints
{
    public static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/items").WithTags("Items").RequireAuthorization();

        group.MapGet("/", (ItemStore items) =>
            Results.Ok(items.All().Select(ItemDto.From)));

        group.MapPost("/", (CreateItemRequest request, ItemStore items) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { message = "Title is required." });
            }

            var item = items.Add(request.Title.Trim());
            return Results.Created($"/api/items/{item.Id}", ItemDto.From(item));
        });

        group.MapPatch("/{id:int}", (int id, UpdateItemRequest request, ItemStore items) =>
        {
            var item = items.SetDone(id, request.Done);
            return item is null ? Results.NotFound() : Results.Ok(ItemDto.From(item));
        });

        group.MapDelete("/{id:int}", (int id, ItemStore items) =>
            items.Remove(id) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
