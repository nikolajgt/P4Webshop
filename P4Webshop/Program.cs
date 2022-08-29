using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using P4Webshop.Extensions.HealthChecks;
using P4Webshop.Extensions.JWT;
using P4Webshop.Interface;
using P4Webshop.Interface.Generic;
using P4Webshop.Models;
using P4Webshop.Models.HealthChecks;
using P4Webshop.Models.POCO;
using P4Webshop.Repository;
using P4Webshop.Repositorys;
using P4Webshop.Services;
using P4Webshop.Workers;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentityCore<Customer>(i =>
{
    //i.User.RequireUniqueEmail = true;
    i.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    i.Lockout.AllowedForNewUsers = true;
    i.Password.RequireDigit = false;
    i.Password.RequireNonAlphanumeric = false;
    i.Password.RequireUppercase = false;
    i.Password.RequiredLength = 2;
}).AddEntityFrameworkStores<MyDbContext>();

builder.Services.AddIdentityCore<Admin>(i =>
{
    i.Password.RequireDigit = false;
    i.Password.RequireNonAlphanumeric = false;
    i.Password.RequireUppercase = false;
    i.Password.RequiredLength = 2;
}).AddEntityFrameworkStores<MyDbContext>();

builder.Services.AddIdentityCore<Employee>(i =>
{
    i.Password.RequireDigit = false;
    i.Password.RequireNonAlphanumeric = false;
    i.Password.RequireUppercase = false;
    i.Password.RequiredLength = 2;
}).AddEntityFrameworkStores<MyDbContext>();

builder.Services.AddDbContextFactory<MyDbContext>(opt => opt
    .EnableDetailedErrors()
    .UseMySql(config.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(config.GetConnectionString("mysql")), mysqlOpt =>
                mysqlOpt.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)
                    )
    );


///This is for our generic repo DI
builder.Services.AddScoped(typeof(IGenericUserRepository<>), typeof(GenericUserRepository<>));
builder.Services.AddScoped(typeof(IGenericProductRepository<>), typeof(GenericProductRepository<>));

/// Standard DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IShoppingService, ShoppingService>();
builder.Services.AddScoped<IShoppingRepository, ShoppingRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();


/// These is a part of our JWT securirty and accessing HttpReqeust on controllers
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthenticationHandler>();
builder.Services.AddSingleton<IWorkerControlService, WorkerControlService>();
builder.Services.AddSingleton<IHttpClientService, HttpClientService>();

builder.Services.AddSingleton(config.GetSection("SecretKeys").Get<SecretKeys>());

///Hosted services
builder.Services.AddHostedService<Worker1>();
builder.Services.AddHostedService<Worker2>();


///This for health check on our services and system. Will be used on admin page to see services/workers uptime
builder.Services.AddHealthChecks()
                    .AddDbContextCheck<MyDbContext>("Database health check", null, new[] { "database" })
                    .AddCheck<AdminServiceHealthCheck>("Admin service health check", null, new[] { "service" })
                    .AddCheck<UserServiceHealthCheck>("User service health check", null, new[] { "service" })
                    .AddCheck<JWTServiceHealthCheck>("Jwt service health check", null, new[] { "service" });

/// HTTP Client for checking health by make a get request on the service
builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(Program));

var key = config.GetSection("Secretkeys").GetRequiredSection("JwtKey").Value;
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        /// i need to check if this in only for SignalR JWT validation

        //x.Events = new JwtBearerEvents
        //{
        //    OnMessageReceived = context =>
        //    {
        //        var accessTokenResponse = context.HttpContext.Request.Headers["Authorization"].ToString();
        //        if (String.IsNullOrEmpty(accessTokenResponse))
        //            return Task.CompletedTask;

        //        var accessTokenSkippedFirst = string.Join(string.Empty, accessTokenResponse.Skip(8));
        //        var finalAccessToken = accessTokenSkippedFirst.Remove(accessTokenSkippedFirst.Length - 1, 1);

        //        //var test = context.HttpContext.User;
        //        //var path = context.HttpContext.Request.Path;
        //        if (!string.IsNullOrEmpty(finalAccessToken))
        //        {
        //            context.Token = finalAccessToken;
        //        }
        //        return Task.CompletedTask;
        //    }
        //};
    });
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Beaerer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Type = SecuritySchemeType.ApiKey,
    });


    opt.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new HealthCheck
            {
                Component = x.Key,
                Status = x.Value.ToString(),
                Description = x.Value.Description
            }),
            Duration = report.TotalDuration,
        };
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
});

app.Run();
