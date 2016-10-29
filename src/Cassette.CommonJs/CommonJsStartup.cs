using Cassette.TinyIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  public class CommonJsStartup : IConfiguration<TinyIoCContainer>
  {
    public void Configure(TinyIoCContainer container)
    {
      container.Register<CommonJsSettings>().AsSingleton();
    }
  }
}
