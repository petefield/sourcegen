using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace pfie.utilities
{
    public static class ConsoleApplicationBuilder
    {
        public static async Task ConfigureAndRun<T>(string[] args, Action<IServiceCollection>? configureServices = null) where T : class , IApp
        {
            var applicationBuilder = Host.CreateApplicationBuilder();

            configureServices?.Invoke(applicationBuilder.Services);

            applicationBuilder.Services.AddSingleton<T>();
            
            var host = applicationBuilder.Build();

            await host.Services.GetRequiredService<T>().Run();
        }
    }
}
