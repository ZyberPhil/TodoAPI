
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Services;

namespace TodoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            builder.Services.AddControllers();
            DiscordNotifier.SendNotification($"```Controller Geladen```");
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            DiscordNotifier.SendNotification($"```Swagger Geladen```");
            builder.Services.AddDbContext<TodoDbContext>(opt =>
            opt.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")
                )
             ));
            DiscordNotifier.SendNotification($"```DBContext Geladen```");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
