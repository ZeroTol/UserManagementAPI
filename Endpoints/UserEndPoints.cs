using FluentValidation;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        // GET /users?page=1&pageSize=20
        group.MapGet("/", (IUserStore store, int page = 1, int pageSize = 20) =>
        {
            if (page < 1)
                return Results.BadRequest("page must be >= 1.");
            if (pageSize < 1 || pageSize > 100)
                return Results.BadRequest("pageSize must be between 1 and 100.");

            var all = store.GetAll().ToList();
            var total = all.Count;
            var items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Results.Ok(new PagedResult<User>(items, page, pageSize, total));
        });

        // GET /users/{id}
        group.MapGet("/{id:int}", (int id, IUserStore store) =>
        {
            var user = store.GetById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        // POST /users
        group.MapPost("/", (User input, IValidator<User> validator, IUserStore store) =>
        {
            var validation = validator.Validate(input);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var created = store.Add(input);
            return Results.Created($"/users/{created.Id}", created);
        });

        // PUT /users/{id}
        group.MapPut("/{id:int}", (int id, User input, IValidator<User> validator, IUserStore store) =>
        {
            if (input.Id != 0 && input.Id != id)
                return Results.BadRequest("Id in URL does not match Id in body.");

            var validation = validator.Validate(input);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var updated = store.Update(id, input);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        // DELETE /users/{id}
        group.MapDelete("/{id:int}", (int id, IUserStore store) =>
        {
            var deleted = store.Delete(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}