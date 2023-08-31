using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace UrbanHelp
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            // создаем хост приложения
            var host = Host.CreateDefaultBuilder()
                // внедряем сервисы
                .ConfigureServices(services =>
                {
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<Countries>();
                    services.AddDbContext<AppContext>();
                    services.AddHttpClient();
                })
                .Build();
            // получаем сервис - объект класса App
            var app = host.Services.GetService<App>();
            // запускаем приложения
            app?.Run();
        }
        
    }
    public class Countries
    {
        public Countries()
        {
            using (StreamReader reader = new StreamReader("Data/Countries.txt"))
            {
                string line = reader.ReadToEnd();
                Country.AddRange(line.Split(Environment.NewLine));
            }
        }
        public List<string> Country { get; set; } = new();
    }
}
