
using System;
using System.Text;
using BLL.Auth;
using Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositroy;
using Timesheet.BLL.Mapping;
using Timesheet.BLL.TimesheetLog;
using Timesheet.Core.Entites;
using Timesheet.Helper;
using Timesheet.Repositroy.Infrastructure;

namespace Timesheet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.Configure<AuthSetting>(builder.Configuration.GetSection(nameof(AuthSetting)));
            builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

            builder.Services.AddDbContext<TimesheetContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(ctf => { ctf.AddProfile<MapperProfile>();  });
            builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IAuthBLL, AuthBLL>();
            builder.Services.AddScoped<ITimesheetAttendanceBLL, TimesheetAttendanceBLL>();
            // Add services to the container.

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TimesSheet Api",
                    Description = ".Net 8"
                });

                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration[$"{nameof(AuthSetting)}:{nameof(AuthSetting.Jwt)}:{nameof(AuthSetting.Jwt.Secret)}"])),
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration[$"{nameof(AuthSetting)}:{nameof(AuthSetting.Jwt)}:{nameof(AuthSetting.Jwt.Issuer)}"],
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero 
            };

            builder.Services.AddSingleton(tokenValidationParams);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(jwt =>
                    {
                        jwt.SaveToken = true;
                        jwt.TokenValidationParameters = tokenValidationParams;
                    });



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //log4net
               builder.Logging.ClearProviders();
                var log4netPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
                builder.Logging.AddLog4Net(log4netPath);



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
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

            app.Run();
        }
    }
}
