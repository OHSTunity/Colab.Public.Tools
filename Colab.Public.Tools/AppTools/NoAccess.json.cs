using System.Linq;
using System;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Advanced;

using Concepts.Ring1;
using Concepts.Ring3;
using Concepts.Ring2;
using Concepts.Ring8.Tunity;


namespace Colab.Public
{
    [NoAccess_json]
    partial class NoAccess : Page, IBound<Something>, IContextApp
    {
        public String ContextId
        {
            get
            {
                return Data != null ? Data.DbIDString : "";
            }
        }

        public static NoAccess Show(String message = "No access", Something context = null, String loginurl = "")
        {
            var app = Master.Current.GetApplication<NoAccess>();
            if (app == null)
            {
                app = new NoAccess();
            }
            app.Data = context;
            app.Message = message;
            app.LoginUrl = loginurl;
            Master.Current.SetApplication(app);
            return app;
        }
    }
}
