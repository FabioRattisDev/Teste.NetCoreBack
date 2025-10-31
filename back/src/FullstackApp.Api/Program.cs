using FullstackApp.Application;
using FullstackApp.Domain.Services.Interface;
using FullstackApp.Infrastructure;
using FullstackApp.Infrastructure.Repositories;
using FullstackApp.Infrastructure.Repositories.Interface;
using FullstackApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FullstackApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "FullstackApp.Api", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Digite: Bearer {seu token JWT}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });


            builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IJwtService, JwtService>();

            // MediatR
            //builder.Services.AddMediatR(typeof(ApplicationAssemblyMarker));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());

            // --- JWT CONFIG ---
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT__Key env var missing");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? "default_issuer";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_Audience") ?? "default_audience";
            var jwtExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_ExpireMinutes") ?? "60");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });


            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default") ??
                                    Environment.GetEnvironmentVariable("DATABASE_CONNECTION") ??
                                    throw new Exception("Database connection string not found. Set DATABASE_CONNECTION or ConnectionStrings__Default env var.");
            //var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)

            );

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments("/swagger"))
                    {
                        var user = Environment.GetEnvironmentVariable("SWAGGERSECURITY_USER") ?? "";
                        var password = Environment.GetEnvironmentVariable("SWAGGERSECURITY_PASSWORD") ?? "";

                        if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(password))
                        {
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
                            {
                                context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Swagger Area\"");
                                context.Response.StatusCode = 401;
                                return;
                            }

                            try
                            {
                                var authValue = authHeader.Substring("Basic ".Length).Trim();
                                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authValue));
                                var parts = decoded.Split(':');

                                if (parts.Length < 2 || parts[0] != user || parts[1] != password)
                                {
                                    context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Swagger Area\"");
                                    context.Response.StatusCode = 401;
                                    return;
                                }
                            }
                            catch
                            {
                                context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Swagger Area\"");
                                context.Response.StatusCode = 401;
                                return;
                            }
                        }
                    }
                    await next();
                });
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            if (args.Contains("--ef-migrations") == false)
            {
                app.Run();
            }
        }
    }
}