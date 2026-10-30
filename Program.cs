using UserManagementAPI.Endpoints;
using UserManagementAPI.Services;
    
var builder = WebApplication.CreateBuilder(args);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory user store
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register endpoint groups
app.MapUserEndpoints();

app.Run();