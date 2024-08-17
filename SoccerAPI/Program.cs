using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using SoccerAPI.Configuration;

// Generate and print a secure key
void GenerateAndPrintKey()
{
    var key = GenerateKey();
    Console.WriteLine($"Generated Key (Base64): {key}");
    VerifyKeySize(key);
}


string GenerateKey()
{
    using (var rng = new RNGCryptoServiceProvider())
    {
        var key = new byte[32]; // 32 bytes = 256 bits
        rng.GetBytes(key);
        return Convert.ToBase64String(key);
    }
}

void VerifyKeySize(string base64Key)
{
    var keyBytes = Convert.FromBase64String(base64Key);
    Console.WriteLine($"Key size in bits: {keyBytes.Length * 8}");
}


// Build the application
var builder = WebApplication.CreateBuilder(args);

// Generate and print a secure key (for testing purposes)
// Comment this out or remove it when deploying to production
GenerateAndPrintKey();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SoccerAPI", Version = "v1" });

    // Add JWT Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
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
            new List<string>()
        }
    });
});
// Testing configuration values
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

Console.WriteLine($"Issuer: {issuer}");
Console.WriteLine($"Audience: {audience}");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Optional: Use this if you want to keep the original property names
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
   
    c.OperationFilter<JsonPatchOperationFilter>(); // Register the operation filter
});

var app = builder.Build();




// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles(); // html, images

app.UseCors("MyPolicy"); // policy block and allow

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
