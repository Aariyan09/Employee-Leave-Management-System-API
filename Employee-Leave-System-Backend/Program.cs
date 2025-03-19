using Employee_Leave_System_Backend.Data;
using Employee_Leave_System_Backend.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Employee_Leave_System_Backend.Entities.DbModels;
using System.Security.Claims;




var builder = WebApplication.CreateBuilder(args);

IdentityModelEventSource.ShowPII = true;

// Add services to the container.

builder.Services.AddControllers();

#region Register services in dependency injection container
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
#endregion


builder.Services.AddDbContext<SQLDBContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region Policy for Admin Authorization

// Retrieve JWT settings from configuration

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])
            ),

            ValidateLifetime = false, // Disable expiration check
        };

        // Capture JWT authentication errors
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine($"Received Token: '{context.Token}'");
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("JWT Challenge triggered.");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                Console.WriteLine("JWT Forbidden error.");
                return Task.CompletedTask;
            }
        };
    });



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin")); // Only Admin can access

    options.AddPolicy("UserPolicy", policy =>
    policy.RequireRole("User")); // Only Employee can access

});
#endregion

#region CORS
// Load Allowed Origins from Configuration
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors();
#endregion

#region Configure Swagger to Accept JWT Tokens

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Leave Management API", Version = "v1" });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Allow any origin, method, and header 
app.UseCors(builder =>
    builder.WithOrigins(allowedOrigins)
           .AllowAnyMethod()
           .AllowAnyHeader());


// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leave Management API v1");
    c.RoutePrefix = string.Empty; // Swagger at root (localhost:5000/)
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
