using System.Text;
using CarGleam.Data;
using CarGleam.Service;
using CarGleam.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args); // object 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
});

//dbcontext class is added to the program.cs file
builder.Services.AddDbContext<EFCoreDBContext>(item =>
     item.UseSqlServer(builder.Configuration.GetConnectionString("dbconnection"))); // name of the connection string

//----------------------------- JWT AUTHENTICATION -----------------------
var jwtSettings = builder.Configuration.GetSection("JwtSetting"); // Get the JwtSetting section from appsettings.json
var key = Encoding.ASCII.GetBytes(jwtSettings["key"]); 
var issuer = jwtSettings["issuer"];
var audience = jwtSettings["audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default authentication scheme to JWT
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters // Configure the JwtBearerOptions
    {
        ValidateIssuer = true, // Validate the server that created the token
        ValidateAudience = true,
        ValidateLifetime = true, // Validate that the token is still valid
        ValidateIssuerSigningKey = true, 

        ValidIssuer = jwtSettings["issuer"], // The server that created the token
        ValidAudience = jwtSettings["audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key) // The key we use to sign the token
    };
});

//builder.Services.AddAuthorization(); 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin")); 
});

//-----services being defined here------------
builder.Services.AddHostedService<UserInactivityService>(); //hosted service is used to run background tasks

builder.Services.AddScoped<EmailNotificationService>(); // Register EmailNotificationService / injection, scoped is used for the lifetime of the request
//--------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapControllers(); // Map the controllers / routes

app.Run();
