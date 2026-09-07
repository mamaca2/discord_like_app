using DiscordApp.Application.contracts;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Services;
using DiscordApp.Application.Services.AuthenticationServices;
using DiscordApp.Application.Services.UserSearchServices;
using DiscordApp.Application.Validators;
using DiscordApp.Domain.Models;
using DiscordApp.Infrastructure.DB;
using DiscordApp.Infrastructure.Email;
using DiscordApp.Server.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =============================
// Controllers & OpenAPI
// =============================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// =============================
// Database
// =============================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Application sees only IAppDbContext
builder.Services.AddScoped<IAppDbContext>(
    sp => sp.GetRequiredService<AppDbContext>());

// =============================
// Database Initializer
// =============================
builder.Services.AddSingleton<DatabaseInitializer>();

// =============================
// CORS
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =============================
// ASP.NET Identity
// =============================
builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#";
        options.SignIn.RequireConfirmedEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

// =============================
// JWT Authentication
// =============================
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("=================================");
                Console.WriteLine("JWT AUTHENTICATION FAILED");
                Console.WriteLine(context.Exception.Message);
                Console.WriteLine("=================================");

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Console.WriteLine("=================================");
                Console.WriteLine("JWT VALIDATED");

                foreach (var claim in context.Principal!.Claims)
                {
                    Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
                }

                Console.WriteLine("=================================");

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:Key"]
                    ?? throw new InvalidOperationException("JwtSettings:Key is missing.")
                )
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

// =============================
// Application Services
// =============================
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IFriendRequestService, FriendRequestService>();
builder.Services.AddScoped<IFriendsService, FriendsService>();
builder.Services.AddScoped<IUserSearchService, UserSearchService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

var app = builder.Build();

// Enable Developer Exception Page in local dev to see actual exception messages
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

// =============================
// Database Initialization
// =============================
using (var scope = app.Services.CreateScope())
{
    var databaseInitializer =
        scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

    await databaseInitializer.InitializeAsync();
}

// =============================
// Roles Initialization
// =============================
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "User", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// =============================
// HTTP Pipeline
// =============================
app.UseCors("ReactClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();