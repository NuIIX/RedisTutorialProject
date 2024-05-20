using Microsoft.EntityFrameworkCore;
using SimpleExampleUsingRedis.Models;
using StackExchange.Redis;


namespace SimpleExampleUsingRedis
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // получаем строку подключени€ из файла конфигурации
            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // добавл€ем контекст ApplicationContext в качестве сервиса в приложение
            builder.Services.AddDbContext<CarContext>(options => options.UseSqlServer(connection));

            builder.Services.AddMvc(); // добавл€ем сервисы MVC

            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();// добавл€ем сервис Redis

            var app = builder.Build();

            // ѕолучаем контекст из приложени€
            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<CarContext>();

            //await CarsGenerator.GenerateRandomCarAsync(context, 10);

            // устанавливаем сопоставление маршрутов с контроллерами
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}