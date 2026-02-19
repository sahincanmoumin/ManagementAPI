using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAccessLayer.Context;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.BackgroundServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// log yapilandirmaaa
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// sql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));


var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// interface isterse verilicek olan
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFarmRepository, FarmRepository>();
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();


builder.Services.AddScoped<IAuthService>(provider =>
    new AuthService(provider.GetRequiredService<IUserRepository>(), jwtSecretKey));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFarmService, FarmService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddHostedService<AnimalLifeCycleService>();
builder.Services.AddHostedService<ProductGenerationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Controller sıralaması
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.OrderActionsBy((apiDesc) =>
    {
        var order = new Dictionary<string, int>
        {
            { "Auth", 1 },
            { "User", 2 },
            { "Farm", 3 },
            { "Animal", 4 },
            { "Product", 5 }
        };

        var controller = apiDesc.ActionDescriptor.RouteValues["controller"];
        return order.ContainsKey(controller) ? order[controller].ToString() : "999";
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("Farm Management API started");

app.Run();