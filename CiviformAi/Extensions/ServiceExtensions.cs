using Contracts;
using LoggingService;
using Repository;

namespace CiviformAi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerManager, LoggerManager>();
        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
         services.AddScoped<IRepositoryManager, RepositoryManager>();
        public static void ConfigureAccessSchemaReader(this IServiceCollection services) =>
         services.AddScoped<IAccessSchemaReader<TableSchema>, AccessSchemaReader>();
        public static void ConfigureTempAccessSchemaStore(this IServiceCollection services) =>
            services.AddSingleton<TempAccessSchemaStore>();
        public static void ConfigureAccessImportService(this IServiceCollection services) =>
            services.AddScoped<IAccessImportService<TableSchema>, AccessImportService>();

    }
}
