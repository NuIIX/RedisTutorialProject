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

            // �������� ������ ����������� �� ����� ������������
            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // ��������� �������� ApplicationContext � �������� ������� � ����������
            builder.Services.AddDbContext<CarContext>(options => options.UseSqlServer(connection));

            builder.Services.AddMvc(); // ��������� ������� MVC

            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();// ��������� ������ Redis

            var app = builder.Build();

            // �������� �������� �� ����������
            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<CarContext>();

            //await CarsGenerator.GenerateRandomCarAsync(context, 10);

            // ������������� ������������� ��������� � �������������
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}