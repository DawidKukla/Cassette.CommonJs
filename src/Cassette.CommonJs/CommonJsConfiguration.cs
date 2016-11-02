using Cassette.TinyIoC;

namespace Cassette.CommonJs
{
  public class CommonJsConfiguration : IConfiguration<TinyIoCContainer>
  {
    public void Configure(TinyIoCContainer container)
    {
      container.Register<CommonJsSettings>().AsSingleton();
      container.Register<IExternalModuleResolver, ExternalModuleResolver>().AsSingleton();
    }
  }
}
