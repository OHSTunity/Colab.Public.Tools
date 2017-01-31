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
using Concepts.Ring1;

namespace Colab.Public
{
    internal class UserSessionHook
    {
        private ConcurrentDictionary<ulong, String> _userSessions =
            new ConcurrentDictionary<ulong, String>();
        private ConcurrentDictionary<String, TunityUser> _loggedInUsers =
            new ConcurrentDictionary<String, TunityUser>();

        private Action<TunityUser> _action;

       // private Action<Something> _contextAction;


        public UserSessionHook(Action<TunityUser> action)
        {
            _action = action;
            Hook<UserSession>.CommitInsert += (object sender, UserSession s) =>
            {
                ulong id = s.GetObjectNo();
                if (!_userSessions.ContainsKey(id))
                {
                    _userSessions.TryAdd(id, s.SessionIdString);
                }
                UpdateHookedApps(id, s.User);
            };

            Hook<UserSession>.CommitDelete += (object sender, ulong id) =>
            {
                UpdateHookedApps(id, null);
                String value;
                _userSessions.TryRemove(id, out value);
            };

            Hook<UserSession>.CommitUpdate += (object sender, UserSession s) =>
            {
                UpdateHookedApps(s.GetObjectNo(), s.User);
            };
        }


        private void UpdateHookedApps(ulong id, TunityUser user)
        {
            if (_userSessions.ContainsKey(id) && _action != null)
            {
                String usession = _userSessions[id];
                TunityUser latestUser = null;
                if (_loggedInUsers.ContainsKey(usession))
                {
                    latestUser = _loggedInUsers[usession];
                    _loggedInUsers[usession] = user;
                }
                else
                {
                    _loggedInUsers.TryAdd(usession, user);
                }

                if (!Db.Equals(latestUser, user))
                {
                    try
                    {
                        Session.ScheduleTask(usession, (Session s, String ses) =>
                        {
                            try
                            {
                                if (s != null)
                                {
                                    _action(user);
                                    s.CalculatePatchAndPushOnWebSocket();
                                }
                            }
                            catch
                            {
                            }
                        });
                    }
                    catch
                    {
                    }
                }
            }
        }

    }
}

