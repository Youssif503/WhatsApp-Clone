using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.Services;
using Whatsapp.DAL.data;
using Whatsapp.DAL.models;
using Whatsapp.DAL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

#region infrastructure

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JWToptions>(builder.Configuration.GetSection("JWT"));

Batteries.Init();
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<RefreshTokenRepository>();
builder.Services.AddMemoryCache();

builder.Services.AddIdentity<User,IdentityRole>(op =>
    {
        op.User.RequireUniqueEmail = true;
        op.Password.RequiredLength = 5;
        op.Password.RequireNonAlphanumeric = false;
        op.Password.RequireLowercase = false;
        op.Password.RequireUppercase = false;
        op.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
    op.RequireHttpsMetadata = false;
    op.SaveToken = true;
    op.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateIssuerSigningKey =  true,
        ValidIssuer =  builder.Configuration["JWT:Issuer"],
        
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        
        ValidateLifetime = true,
        
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();


