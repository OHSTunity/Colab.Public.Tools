using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.Reflection;
using System.Resources;
using Concepts.Ring8.Tunity;

namespace Colab.Public
{
    public static class ColabX
    {
        public const String ActivityStateChange = "ActivityStateChange";


        #region Messaging
        /// <summary>
        /// Messaging system for now using a hack of urimapping
        /// </summary>
        /// <param name="Message"></param>
        public static void PublishMessage(String Message, String pars)
        {
            try
            {
                Self.GET(UriMapping.MappingUriPrefix + $"/colab/{Message}/{pars}",
                    () =>
                    {
                        return new Page();

                    });
            }
            catch
            {

            }
        }

        public static void PublishMessage(String Message)
        {
            Self.GET(UriMapping.MappingUriPrefix + $"/colab/{Message}",
                () =>
                {
                    return new Page();

                });
        }

        public static void SubscribeMessage(String Message, Action<String> callback)
        {
            var appname = Starcounter.Internal.StarcounterEnvironment.AppName;
            var uri = $"/{appname}/{Message}/{{?}}";
            Handle.GET(uri, (Request req, String data) =>
                {
                    try
                    {
                        callback(data);
                    }
                    catch
                    {
                    }
                    return null;
                });
            UriMapping.Map(uri + "@w", UriMapping.MappingUriPrefix + $"colab/{Message}/@w");
        }

        public static void SubscribeMessage(String Message, Action callback)
        {
            var appname = Starcounter.Internal.StarcounterEnvironment.AppName;
            var uri = $"/{appname}/{Message}";
            Handle.GET(uri, (Request req) =>
            {
                try
                {
                    callback();
                }
                catch
                {
                }
                return null;
            });
            UriMapping.Map(uri, UriMapping.MappingUriPrefix + $"colab/{Message}");
        }
    
    #endregion


    public static dynamic RegisterIndex(String name, String index, Boolean forceRecreate = false)
    {
        var mdi = Db.SQL<Starcounter.Metadata.Index>("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", name).First;
        if (mdi != null && forceRecreate)
        {
            Db.SQL(String.Format("DROP INDEX {0} ON {1}", name, mdi.Table.Name));
            mdi = null;
        }
        if (mdi == null)
        {
            return Db.SQL(String.Format("CREATE INDEX {0} ON {1}", name, index));
        }
        return null;
    }

    /*
    public static dynamic BuildOn<T>(Request req, Func<T, dynamic> func) where T : Json
    {
        Stack<Type> DependencyTree = new Stack<Type>();
        Type dependentOn = typeof(T);
        while (JsonManager.Dependencies.ContainsKey(dependentOn))
        {
            dependentOn = JsonManager.Dependencies[dependentOn];
            DependencyTree.Push(dependentOn);
        }

        //Start at the root
        Type type = DependencyTree.Pop();
        Json data = Session.Current.Data;
        if ((data == null) || (data.GetType() == type))
        {
            Session.Current.Data = JsonManager.Constructors[type].DynamicInvoke(data) as dynamic;
            data =  Session.Current.Data;
        }

        //Create the rest
        while (DependencyTree.Count > 0)
        {
            var nextType = DependencyTree.Pop();
            Json nextData = (data as IJsonParent).GetChild<
            if ((data != null) && (data.GetType() == type)) //Nice, its already there
            {
                if (JsonManager.Constructors.[type]
            }

        }
        //Lets see, find the tree

        return null;
    }
     * */

    public static dynamic BuildOn(Request req, String uri)
    {
        return BuildOn(req, uri, (Json json) => { return json; });
    }


    /// <summary>
    /// For desciption of BuildOn part see private method below. This one only adds authentication to that one.
    /// </summary>
    /// <returns></returns>
    public static dynamic BuildOn<T>(Request req, String uri, Func<T, dynamic> func) where T : Json
    {
        try
        {
            if (SessionManager.CheckAuthentication(req))
            {
                Response response = null;
                dynamic d = BuildOn(uri, func);
                if ((response != null) && !(d is Response))
                {
                    response.Resource = d.Root;
                    return response;
                }
                else
                {
                    return d;
                }
            }
            else
            {
                // SessionData.Current.LastUrl = req.Uri;
                Master m = (Master)Self.GET("/colab/master");
                Master.SendCommand(ColabCommand.MORPH_URL, "/launcher/signin?originurl=" + req.Uri);
                return new NoAccess() { };            }
        }
        catch (TunityException e)
        {
            Master m = (Master)Self.GET("/colab/master");
            Master.SendUserError(e.StackTrace);
            return new NoAccess();
        }

    }





    /// <summary>
    /// Using Self.GET to fetch a response for a certain url. If that url returns a Json object of the right type (first parameter type of the delegate)
    /// the delegate is executed with that object as parameter. Otherwise it returns the object without running the delegate.
    /// 
    /// (It will also find parent JSON objects to the url returned object)
    /// 
    /// </summary>
    /// <typeparam name="T">Type of Json object to return (can be omitted since the delegate gives it)</typeparam>
    /// <param name="uri">url to fetch from</param>
    /// <param name="func">delegate with 1 single parameter of the type you want returned</param>
    /// <returns></returns>
    private static dynamic BuildOn<T>(String uri, Func<T, dynamic> func) where T : Json
    {
        dynamic res = Self.GET<dynamic>(uri);
        Json data = null;
        Boolean response = false;
        if (res is Response)
        {
            data = (res as Response).Resource as Json;
            response = true;
        }
        else if (res is Json)
        {
            data = res;
        }
        if (data != null)
        {
            Json pointer = data;
            while (pointer != null)
            {
                if (pointer is T)
                {
                    dynamic t = func(pointer as T);
                    if (response)
                    {
                        (res as Response).Resource = t;
                        return res;
                    }
                    else return t;
                }
                pointer = pointer.Parent;
            }
        }
        return res;
    }


}
}
