using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PalmParadiseHotelApp.Data;
using PalmParadiseHotelApp.Menus;

namespace PalmParadiseHotelApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = builder.Build();

            var options = new DbContextOptionsBuilder<HotelDbContext>();
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            using (var dbContext = new HotelDbContext(options.Options))
            {
                var initializer = new DataInitializer();
                initializer.MigrateAndSeed(dbContext);
            }

            var mainMenu = new MainMenu();
            mainMenu.DisplayMenu();



        }
    }
}
