using FluentValidation;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // All /users endpoints require a valid JWT (Activity 3)
        var group = app.MapGroup("/users")
                       .WithTags("Users")
                       .RequireAuthorization();

        // GET /users?page=1&pageSize=20
        group.MapGet("/", GetUsers)
             .WithName("GetUsers")
             .Produces<PagedResult<User>>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status401Unauthorized);

        // GET /users/{id}
        group.MapGet("/{id:int}", GetUserById)
             .WithName("GetUserById")
             .Produces<User>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status401Unauthorized);

        // POST /users
        group.MapPost("/", CreateUser)
             .WithName("CreateUser")
             .Produces<User>(StatusCodes.Status201Created)
             .ProducesValidationProblem()
             .Produces(StatusCodes.Status409Conflict)
             .Produces(StatusCodes.Status401Unauthorized);

        // PUT /users/{id}
        group.MapPut("/{id:int}", UpdateUser)
             .WithName("UpdateUser")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesValidationProblem()
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status409Conflict)
             .Produces(StatusCodes.Status401Unauthorized);

        // DELETE /users/{id}
        group.MapDelete("/{id:int}", DeleteUser)
             .WithName("DeleteUser")
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }

    // ---------- Handlers ----------

    private static IResult GetUsers(IUserStore store, int page = 1, int pageSize = 20)
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
    }

    private static IResult GetUserById(int id, IUserStore store)
    {
        var user = store.GetById(id);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }

    private static IResult CreateUser(
        User input,
        IValidator<User> validator,
        IUserStore store)
    {
        var validation = validator.Validate(input);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        // Domain exception (duplicate email) is translated to 409 by GlobalExceptionHandler
        var created = store.Add(input);
        return Results.Created($"/users/{created.Id}", created);
    }

    private static IResult UpdateUser(
        int id,
        User input,
        IValidator<User> validator,
        IUserStore store)
    {
        if (input.Id != 0 && input.Id != id)
            return Results.BadRequest("Id in URL does not match Id in body.");

        var validation = validator.Validate(input);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        // Domain exception (duplicate email) is translated to 409 by GlobalExceptionHandler
        var updated = store.Update(id, input);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    private static IResult DeleteUser(int id, IUserStore store)
    {
        var deleted = store.Delete(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}