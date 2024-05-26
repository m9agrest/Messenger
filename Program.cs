using Microsoft.EntityFrameworkCore;
using SimpleMessenger.Data;

namespace SimpleMessenger
{
    public class Program
    {
        public const int Admin = 3;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавление службы DbContext с использованием строки подключения из конфигурации
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();

            // Добавление служб контроллеров с представлениями
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Настройка конвейера HTTP-запросов
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}