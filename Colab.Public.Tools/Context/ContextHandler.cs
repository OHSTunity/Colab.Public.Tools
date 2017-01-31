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

    public static class ContextHandler
    {
        public static T GetContext<T>(String id) where T:class
        {
            try
            {
                return DbHelper.FromID(Convert.ToUInt64(id)) as T;
            }
            catch
            {
                return null;
            }
        }
        
        public static void SetContext(String contextId, String appname)
        {
            var dummy = Self.GET(UriMapping.MappingUriPrefix + String.Format("/context/id={0}&app={1}", contextId, appname), () =>
                {
                    return new Page();
                });
        }


        public static void SetNonContext(String uri) //Always save uri for last call without context
        {
            var dummy = Self.GET(UriMapping.MappingUriPrefix + String.Format("/context/id=None&closeuri={0}", uri), () =>
            {
                return new Page();
            });
        }

        public static void OnContextMenuRequested(Func<Something, Json> callback)
        {
            var appname = Starcounter.Internal.StarcounterEnvironment.AppName;
            var uri = "/" + appname + "/context_menu/";
            Handle.GET(uri + "{?}", (Request req, String id) =>
            {
                try
                { 
                    var context = GetContext<Something>(id);
                    if (context != null)
                        return callback(context);
                }
                catch
                {
                }
                return null;
            });
            UriMapping.Map(uri + "@w", UriMapping.MappingUriPrefix + "/context_menu/@w");
        }


       
        /// <summary>
        /// Just get called if app is current active and have a IContextApp present
        /// </summary>
        /// <param name="callback"></param>
        public static void OnContextSelected(Action<Something> callback)
        {
            var appname = Starcounter.Internal.StarcounterEnvironment.AppName;
            var uri = "/" + appname + "/oncontextselected/";
            Handle.GET(uri + "{?}", (Request req, String data) =>
            {
                try
                {
                    var parts = data.Split('&');
                    var id = parts[0].Split('=')[1];
                    var app = parts[1].Split('=')[1];
                    if (String.Equals(app, appname))
                    {
                        var activity = GetContext<Something>(id);
                        callback(activity);
                    }
                }
                catch
                {
                }
                return null;
            });
            UriMapping.Map(uri + "@w", UriMapping.MappingUriPrefix + "/oncontextselected/@w");
        }

     
    }
}
