using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using Concepts.Ring1;

namespace Colab.Public
{

    public static class LauncherHandler
    {

        public static void OnConfigurationMenuRequested(Func<Menu> callback)
        {
            var appname = Starcounter.Internal.StarcounterEnvironment.AppName;
            var uri = $"/{appname}/configs_menu";
            Handle.GET(uri, (Request req) =>
            {
                try
                { 
                    return callback();
                }
                catch
                {
                }
                return null;
            });
            UriMapping.Map(uri, UriMapping.MappingUriPrefix + "/configs_menu");
        }
    }
}
