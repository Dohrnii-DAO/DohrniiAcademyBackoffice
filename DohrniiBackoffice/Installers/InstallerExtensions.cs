namespace DohrniiBackoffice.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration Configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(c => typeof(IInstaller).IsAssignableFrom(c) && !c.IsInterface && !c.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();
            installers.ForEach(installer => installer.IntallServices(services, Configuration));

        }
    }
}
