using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.Resources;
using Concepts.Ring8.Tunity;
using System.Globalization;

namespace Colab.Public
{
    public static class SessionManager
    {
        private static ConcurrentDictionary<String, SessionData> Datas = new ConcurrentDictionary<String, SessionData>();

        public static Session CurrentSession
        {
            get
            {
                if (Session.Current == null)
                {
                    var session = new Session(SessionOptions.PatchVersioning);
                    Session.Current = session;
                }
                return Session.Current;
            }
        }


        public static IEnumerable<String> ActiveSessionIds(TunityUser user)
        {
            foreach (String key in Datas.Keys)
            {
                var su = Db.SQL<UserSession>("SELECT o FROM Concepts.Ring8.Tunity.UserSession o WHERE o.SessionIdString=?", key).First;
                if ((su != null) && Db.Equals(su.User, user))
                {
                    yield return key;
                }
            }
        }

        public static IEnumerable<String> ActiveSessionIds()
        {
            return Datas.Keys;
        }

        public static SessionData CurrentData
        {
            get
            {
                if (Datas.ContainsKey(CurrentSession.SessionId))
                {
                    return Datas[CurrentSession.SessionId];
                }
                else
                {
                    var data = new SessionData();
                    data.ScSessionOwned = true;
                    Datas.TryAdd(CurrentSession.SessionId, data);
                    CurrentSession.AddDestroyDelegate((Session s) =>
                    {
                        SessionData sd;
                        Datas.TryRemove(s.SessionId, out sd);
                    });
                    return data;
                }
            }
        }


        private static string FindCookie(List<string> cookies, string key)
        {
            for (int i = 0; i < cookies.Count; i++)
            {
                if (cookies[i].IndexOf(key + "=") > -1)
                {
                    return cookies[i];
                }
            }
            return null;
        }

        public static Boolean CheckAuthentication(Request req)
        {
            return CheckAuthentication(req, true);
        }
        public static Boolean CheckAuthentication(Request req, bool deleteCookie)
        {
            var currentData = CurrentData;
            return currentData.Authenticated;
        }

        private static Dictionary<String, ResourceManager> resources = new Dictionary<string, ResourceManager>();
        private static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

        private static Assembly GetAssembly(String assemblyname)
        {
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.FullName == assemblyname)
                    return ass;
            }
            return null;
        }

        private static ResourceManager GetRM(String resource)
        {
            if (resources.ContainsKey(resource))
                return resources[resource];
            else
            {
                Assembly ass = GetAssembly(resource);
                if (ass != null)
                {
                    ResourceManager rm = new ResourceManager(resource, ass);
                    resources.Add(resource, rm);
                    return rm;
                }
                else
                {
                    throw new Exception("No assembly with name: " + resource);
                }
            }
        }

        public static string GetString(String resource, String value)
        {
            return GetRM(resource).GetString(value, culture);
        }

        #region UserHooks
        private static ConcurrentBag<UserSessionHook> _userSessionHooks = new ConcurrentBag<UserSessionHook>();
        public static void OnUserChange(Action<TunityUser> action)
        {
            _userSessionHooks.Add(new UserSessionHook(action));
        }
        #endregion

    }
}

