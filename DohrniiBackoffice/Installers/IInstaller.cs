namespace DohrniiBackoffice.Installers
{
    public interface IInstaller
    {
        void IntallServices(IServiceCollection services, IConfiguration Configuration);
    }
}
