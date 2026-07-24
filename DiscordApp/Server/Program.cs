using DiscordApp.Server.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register DatabaseInitializer
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

// Run database initialization
var databaseInitializer =
    app.Services.GetRequiredService<DatabaseInitializer>();

await databaseInitializer.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();