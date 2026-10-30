using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
                       .WithTags("Users");

        // GET /users
        group.MapGet("/", (IUserStore store) => Results.Ok(store.GetAll()));

        // GET /users/{id}
        group.MapGet("/{id:int}", (int id, IUserStore store) =>
        {
            var user = store.GetById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        // POST /users
        group.MapPost("/", (User input, IUserStore store) =>
        {
            if (!IsValid(input, out var error))
                return Results.BadRequest(error);

            var created = store.Add(input);
            return Results.Created($"/users/{created.Id}", created);
        });

        // PUT /users/{id}
        group.MapPut("/{id:int}", (int id, User input, IUserStore store) =>
        {
            if (!IsValid(input, out var error))
                return Results.BadRequest(error);

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

    private static bool IsValid(User user, out string error)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName) ||
            string.IsNullOrWhiteSpace(user.LastName) ||
            string.IsNullOrWhiteSpace(user.Email))
        {
            error = "FirstName, LastName and Email are required.";
            return false;
        }

        if (!user.Email.Contains('@'))
        {
            error = "Email is invalid.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}