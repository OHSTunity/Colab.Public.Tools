using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Starcounter;
using Starcounter.Advanced;


namespace Colab.Public
{
    public static class Counter
    {
        private static int _counter = 0;
        public static int GetNumber()
        {
            return _counter++;
        }
    }

    [Master_json]
    partial class Master : Page
    {
        public int Count = Counter.GetNumber();

        protected Dictionary<Json, CallbackData> _callbacks = new Dictionary<Json, CallbackData>();
        public static ConcurrentDictionary<String, String> UtilsHtmls = new ConcurrentDictionary<String, String>();
        public static ConcurrentDictionary<String, Master> Masters = new ConcurrentDictionary<String, Master>();


        protected override void OnData()
        {
            base.OnData();
        }

        public Boolean IsMobile = false;

        public static Master Current
        {
            get
            {
                Session session = SessionManager.CurrentSession;
                if (!(session.Data is Master))
                {
                    var appname = Starcounter.Internal.StarcounterEnvironment.AppName;
                    var master = new Master() { };
                    if (UtilsHtmls.ContainsKey(appname))
                        master.Utils.Html = UtilsHtmls[appname];
                    session.Data = master;
                }
                return session.Data as Master;
            }
        }

        private static Master FindRecursiveMaster(Json child, int count = 0)
        {
            if (child is Master)
                return child as Master;
            else if ((child != null) && (count < 15))
            {
                count++;
                return FindRecursiveMaster(child.Parent, count);
            }
            return null;
        }

        
        #region Utils/Userspace
        public static T GetPersistentApp<T>() where T : Json
        {
            if (Master.Current != null)
                return Master.Current.Utils.GetPersistentApp<T>();
            else
                return null;
        }

        public static T AddModal<T>(T modal) where T : Json
        {
            return AddModal(modal, null) as T;
        }

        public static Json AddModal(Json modal, Action<ModalResult> callback)
        {
            if (Master.Current != null)
            {
                return Master.Current.Utils.AddModal(modal, callback);
            }
            return modal;
        }

        public static T GetModal<T>() where T : Json
        {
            if (Master.Current != null)
            {
                return Master.Current.Utils.GetModal<T>();
            }
            return null;
        }

        public static void RemoveModal<T>() where T : Json
        {
            var modal = GetModal<T>();
            if (modal != null)
                RemoveModal(modal);
        }

        public static void RemoveModal(Json modal, ModalResult result)
        {
            if (Master.Current != null)
            {
                Master.Current.Utils.RemoveModal(modal, result);
            }
        }

        public static void RemoveModalRecursive(Json modal)
        {
            RemoveModal(modal);
            if (modal.Parent != null)
            {
                RemoveModalRecursive(modal.Parent);
            }
        }

        public static void RemoveModal(Json modal)
        {
            RemoveModal(modal, new ModalResult() { Value = ModalResultType.NORESULT });
        }

        public static void RemoveModal(Json modal, ModalResultType type)
        {
            RemoveModal(modal, new ModalResult() { Value = type, Data = modal.Data });
        }

        public static void SendUserInfo(string message, Action<ModalResult> callback = null)
        {
            if (Master.Current != null)
            {
                Master.Current.Utils.ShowUserMessage(ModalMessageType.INFO, message, callback);
            }
        }

        public static void SendUserWarning(string message, Action<ModalResult> callback = null)
        {
            if (Master.Current != null)
            {
                Master.Current.Utils.ShowUserMessage(ModalMessageType.WARNING, message, callback);
            }
        }

        public static void SendUserError(string message, Action<ModalResult> callback = null)
        {
            if (Master.Current != null)
            {
                Master.Current.Utils.ShowUserMessage(ModalMessageType.ERROR, message, callback);
            }
        }

        public static void SendUserError(Exception e)
        {
            SendUserError(e.Message);
        }

        public static void SendCommand(ColabCommand command, params string[] pars)
        {
            if (Master.Current != null)
            {
                Master.Current.Utils.SendCommand(command, pars);
            }
        }
        #endregion

        #region Applications
        public T GetApplication<T>() where T : Json
        {
            foreach (ApplicationContainer pa in this.Applications)
            {
                if (pa.Application is T)
                    return pa.Application as T;
            }
            return null;
        }

        public T AssureApplication<T>(Func<T> creator, object data = null) where T:Json
        {
            var app = GetApplication<T>();
            if (app == null)
            {
                app = creator();
            }
            app.Data = data;
            SetApplication(app);
            return app;
        }

        public Json SetApplication(Json app)
        {
            if (app == null)
                return app;

            Boolean found = false;
            foreach (ApplicationContainer pa in this.Applications)
            {
                pa.Current = (pa.Application == app);
                pa.Application.AutoRefreshBoundProperties = pa.Current;
                found = found || pa.Current;
            }
            if (!found)
            {
                var container = new ApplicationContainer()
                {
                    Application = app,
                    Current = true,
                };
                app.AutoRefreshBoundProperties = true;
                this.Applications.Add(container);
            }
            return app;
        }


        private int GetApplicationIndex(Json app)
        {
            int index = 0;
            foreach (ApplicationContainer pa in this.Applications)
            {
                if (pa.Application == app)
                    return index;
                index++;
            }
            return -1;
        }

        public Json CurrentApplication
        {
            get
            {
                foreach (ApplicationContainer pa in this.Applications)
                {
                    if (pa.Current)
                        return pa.Application;
                }
                return null;
            }
        }

        public void ClearApplications()
        {
            this.Applications.Clear();
        }

        public static T SimpleApplication<T>(object data = null) where T:Json, new()
        {
            return Master.Current.AssureApplication<T>(() =>
            {
                return Db.Scope(() =>
                {
                    return new T();
                });
            }, data);
        }

        #endregion
    }

    public class CallbackData
    {
        public Action<ModalResult> Action;
        public Transaction Transaction;
    }


    public enum ModalMessageType
    {
        [DisplayValue("info")]
        INFO = 0,
        [DisplayValue("warning")]
        WARNING = 1,
        [DisplayValue("error")]
        ERROR = 2
    }

}
