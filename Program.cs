
using FYP.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace rjf.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(name: MyAllowSpecificOrigins, builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<LaundaryDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("LaundryDb"));
            });



            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                     policy => policy.RequireRole("Admin"));
                options.AddPolicy("BranchManager",
                     policy => policy.RequireRole("BranchManager"));
                options.AddPolicy("User",
                     policy => policy.RequireRole("User"));
            });

            builder.Services.AddScoped<Custom>();





            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = builder.Configuration["Jwt:Issuer"],
             ValidAudience = builder.Configuration["Jwt:Issuer"],
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
         };
     });


            var app = builder.Build();

         
                app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseStaticFiles(); // Enable serving static files from wwwroot

            // Serve files from the custom directory (e.g., Images/Products)
            var staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Products");
            if (Directory.Exists(staticFilesPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(staticFilesPath),
                    RequestPath = "/Images/Products"
                });
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
