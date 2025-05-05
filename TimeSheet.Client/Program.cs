using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Timesheet.BLL.Middleware;
using Timesheet.Helper;

namespace TimeSheet.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient("TimeSheet", httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:5267/api/");

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

            // Add authorization services
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();

             var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


app.UseMiddleware<TokenMiddleware>(); 

app.UseAuthentication();
app.UseAuthorization();

            

            app.MapControllerRoute(
                name: "default",
              pattern: "{controller=Auth}/{action=Login}");

            app.Run();
        }
    }
}
