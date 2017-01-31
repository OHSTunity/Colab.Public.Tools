using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using Concepts.Ring3;
using Concepts.Ring1;
using Concepts.Ring8.Tunity;

namespace Colab.Public
{
    /// <summary>
    /// </summary>
    public class SessionData 
    {
        private Master _master;
        private String _culture;
        private Boolean _scSessionOwned;

        public String LastUrl;

        public Boolean CookieSet = false;
        public Boolean PermanentCookie = false;

        public SessionData()
        {
        }


        public static SessionData Current
        {
            get
            {
                return SessionManager.CurrentData;
            }
        }

        public Master Master
        {
            get { return _master; }
            set { _master = value; }
        }

        public Boolean Authenticated
        {
            get
            {

                return User != null;
            }
        }

        public String Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                _culture = value;
            }
        }

        public String Avatar
        {
            get
            {
                return Concepts.Ring8.Tunity.Avatar.GetValueString(Person);
            }
        }

        public TunityUser User
        {
            get
            {
                var userSession = Db.SQL<UserSession>("SELECT o FROM Concepts.Ring8.Tunity.UserSession o WHERE o.SessionIdString=?", 
                    Session.Current.SessionId).First;
                if (userSession != null)
                {
                    return userSession.User;
                }
                return null;
            }
        }

        public Boolean IsSimulatedUser
        {
            get
            {
                var userSession = Db.SQL<UserSession>("SELECT o FROM Concepts.Ring8.Tunity.UserSession o WHERE o.SessionIdString=?",
                    Session.Current.SessionId).First;
                if (userSession != null)
                {
                    return userSession.SimulatedUser != null;
                }
                return false;
            }
        }

        public Person Person
        {
            get
            {
                if (User != null)
                {
                    return User.WhoIs as Person;
                }
                else
                    return null;
            }
        }

        public Boolean ScSessionOwned
        {
            get 
            {
                return _scSessionOwned;
            }
            set 
            {
                _scSessionOwned = value;
            }
        }
    }
}

