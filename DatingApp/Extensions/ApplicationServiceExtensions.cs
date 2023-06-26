using DatingApp.Data;
using DatingApp.Interface;
using DatingApp.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {
            services.AddDbContext<DataContext>(optionsAction: options => options.UseSqlServer(config.GetConnectionString(name: "SQLConnection")));
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            return services;
        }
    }
}
