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
    [NotFound_json]
    partial class NotFound : Page, IBound<Something>, IContextApp
    {
        public String ContextId
        {
            get
            {
                return Data != null ? Data.DbIDString : "";
            }
        }
    }
}
