namespace CBT.Installers
{
    public static class InstallerExtension
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(x =>
               typeof(IInstaller).IsAssignableFrom(x) && !x.IsAbstract).ToList();

            var instanceOfInstallers = installers.Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            instanceOfInstallers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
