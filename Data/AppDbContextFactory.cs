using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace UserManagement.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1) BaseDirectory ≃ ...\UserManagement.Api\bin\Debug\net6.0\
            var basePath = AppContext.BaseDirectory;

            // 2) Subimos 3 niveles: net6.0 → Debug → bin → UserManagement.Api
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));

            // (Opcional: te ayuda a diagnosticar)
            Console.WriteLine($"[DesignTime] Cargando appsettings.json desde: {projectPath}");

            // 3) Cargamos el JSON desde la carpeta del proyecto
            var config = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            // 4) Preparamos las opciones de SQL Server con tu cadena
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}