using Starcounter;
using Starcounter.Advanced;
using System;
using System.Collections.Generic;
using Concepts.Ring1;
using Concepts.Ring3;
using Concepts.Ring2;
using Concepts.Ring8.Tunity;


namespace Colab.Public
{
    [SearchItem_json]
    partial class SearchItem : Json, IBound<object>
    {
        protected override void OnData()
        {
            base.OnData();
            if (String.IsNullOrEmpty(Html))
            {
                Html = "/common/search/default.html";
            }
        }

        public virtual String ActionCB
        {
            get
            {
               return "";
            }
        }
    }
}
