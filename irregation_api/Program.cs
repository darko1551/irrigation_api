using irregation_api;
using irregation_api.BackgroundServices;
using irregation_api.Data;
using irregation_api.Socket;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SQLitePCL;
using System.Text.Json;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    //option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("localDb")));
builder.Services.AddScoped<BackgroundServicesDAO>();
builder.Services.AddTransient<Dao>();
builder.Services.AddHostedService<Socket>();
builder.Services.AddHostedService<IrrigationController>();
builder.Services.AddHostedService<CredentialsController>();
builder.Services.AddHttpClient<ValveClient>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(jwtBearerOptions =>
   {
       jwtBearerOptions.MetadataAddress = "https://id.mobilisis.com/auth/realms/mobilisis.global/.well-known/openid-configuration";
       jwtBearerOptions.Authority = "https://id.mobilisis.com/auth/realms/mobilisis.global";
       jwtBearerOptions.Audience = "darko-debeljak-client";
   });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();





app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();



app.MapControllers();

app.Run();
